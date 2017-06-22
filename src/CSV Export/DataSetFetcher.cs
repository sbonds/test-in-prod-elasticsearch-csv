using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.Elasticsearch.CSVExport
{
    /// <summary>
    /// Fetches dataset from Elasticsearch, providing progress reporting, et. al
    /// </summary>
    public class DataSetFetcher : IDisposable
    {

        public const int MaxSampleSize = 500;
        public const int MaxBatchSize = 2000;


        /// <summary>
        /// Gets or sets ES cluster URI for making requests
        /// </summary>
        public Uri ClusterUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets index pattern (or specific index to query)
        /// </summary>
        public string IndexPattern
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an instance of JsonSerializer for flattening structured documents
        /// </summary>
        public JsonSerializer Serializer
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of DataSetFetcher
        /// </summary>
        public DataSetFetcher()
        {
            Serializer = new JsonSerializer();
            Serializer.DateParseHandling = DateParseHandling.DateTimeOffset;
        }

        /// <summary>
        /// Returns a new instance of ElasticLowLevelClient configured
        /// with DataSetFetcher instance settings
        /// </summary>
        /// <returns></returns>
        protected ElasticLowLevelClient getClient()
        {
            var conf = new ConnectionConfiguration(ClusterUri);
            var client = new ElasticLowLevelClient(conf);
            return client;
        }

        /// <summary>
        /// Prepares query parameters for /search
        /// </summary>
        /// <param name="x"></param>
        /// <param name="luceneQuery"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        protected SearchRequestParameters getSearchParameters(SearchRequestParameters x, string luceneQuery, int maxSize, bool scroll = false)
        {
            if (string.IsNullOrEmpty(luceneQuery))
                throw new ArgumentNullException("luceneQuery");
            if (maxSize > 0 && maxSize < int.MaxValue)
                x = x.AddQueryString("size", maxSize.ToString());
            if (scroll)
                x = x.AddQueryString("scroll", "1m");
            x = x.AddQueryString("q", luceneQuery);
            return x;
        }

        /// <summary>
        /// Flattens document structure into dictionary key-value pairs
        /// </summary>
        /// <param name="documentJson"></param>
        /// <returns></returns>
        protected Dictionary<string, string> flattenDocumentStructure(string documentJson)
        {
            using (var sr = new StringReader(documentJson))
            {
                using (var jr = new JsonTextReader(sr))
                {
                    JObject document = Serializer.Deserialize<JObject>(new JsonTextReader(sr));
                    var jTokens = document.Descendants().Where(p => p.Count() == 0);
                    var results = jTokens.Aggregate(new Dictionary<string, string>(), (properties, jToken) =>
                    {
                        properties.Add(jToken.Path, jToken.ToString());
                        return properties;
                    });
                    return results;
                }
            }
        }


        protected IEnumerable<dynamic> elasticsearchGetSearch(DataFetchContext context)
        {
            var client = getClient();
            var response = client.SearchGet<dynamic>(IndexPattern, x => getSearchParameters(x, context.LuceneQuery, context.MaxBatchSize, context.Scroll));
            if (response.Success)
            {
                context.TotalHits = Convert.ToInt64(response.Body.hits.total);
                context.TimeTaken = TimeSpan.FromMilliseconds(Convert.ToInt32(response.Body.took));
                if (context.Scroll)
                {
                    context.ScrollToken = response.Body._scroll_id;
                }

                foreach (var hit in response.Body.hits.hits)
                {
                    yield return hit;
                }
            }
            else
            {
                throw response.OriginalException;
            }
        }
        protected IEnumerable<dynamic> elasticsearchGetScroll(DataFetchContext context)
        {
            var client = getClient();
            var response = client.Scroll<dynamic>(new { scroll = "1m", scroll_id = context.ScrollToken });
            if (response.Success)
            {
                context.TimeTaken = TimeSpan.FromMilliseconds(Convert.ToInt32(response.Body.took));
                context.Scroll = response.Body.hits.hits.Count > 0;
                foreach (var hit in response.Body.hits.hits)
                {
                    yield return hit;
                }
            }
            else
            {
                throw response.OriginalException;
            }
        }

        protected void flattenDocumentToRow(DataTable table, dynamic hit)
        {
            Dictionary<string, string> rowSource = flattenDocumentStructure(hit._source.ToString());

            // create row for each document in this table
            var row = table.NewRow();
            foreach (var kvp in rowSource)
            {
                if (!table.Columns.Contains(kvp.Key))
                    table.Columns.Add(kvp.Key);                
                row[kvp.Key] = kvp.Value;
            }

            table.Rows.Add(row);
        }

        /// <summary>
        /// Fetches a dataset from ElasticSearch, flattening documents into a DataTable
        /// </summary>
        /// <param name="luceneQuery">Specifies Lucene query to Search documents</param>
        /// <param name="maxSize">Maximum number of results that will be returned. Specify int.MaxValue to disable this limit</param>
        /// <returns></returns>
        public async Task<DataFetchResult> FetchDataSet(string luceneQuery = "*")
        {
            return await FetchDataSet(CancellationToken.None, luceneQuery);
        }

        /// <summary>
        /// Fetches a dataset from ElasticSearch, flattening documents into a DataTable
        /// </summary>
        /// <param name="token">Cancellation token to cancel request and processing</param>
        /// <param name="luceneQuery">Specifies Lucene query to Search documents</param>
        /// <param name="maxSize">Maximum number of results that will be returned. Specify int.MaxValue to disable this limit</param>
        /// <returns></returns>
        public async Task<DataFetchResult> FetchDataSet(CancellationToken token, string luceneQuery = "*")
        {
            return await Task.Run(() =>
            {
                var context = new DataFetchContext();
                context.LuceneQuery = luceneQuery;
                context.MaxBatchSize = MaxSampleSize;
                context.Scroll = false;

                var table = new DataTable();

                try
                {
                    foreach (var hit in elasticsearchGetSearch(context))
                    {
                        token.ThrowIfCancellationRequested();
                        flattenDocumentToRow(table, hit);
                    }
                }
                catch
                {
                    table.Dispose();
                    throw;
                }

                // add columns to context
                foreach(DataColumn col in table.Columns)
                {
                    if (!context.Columns.Contains(col.ColumnName))
                        context.Columns.Add(col.ColumnName);
                }

                return new DataFetchResult(table, context);
            });
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
