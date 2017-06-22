using CsvHelper;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypton.Elasticsearch.CSVExport
{
    public partial class ExportDataForm : Form
    {

        Task fetchTask = null;
        CancellationTokenSource cancellationToken = null;

        JsonSerializer serializer = new JsonSerializer()
        {
            DateParseHandling = DateParseHandling.DateTimeOffset
        };

        string ClusterUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["clusterUrl"];
            }
        }

        string IndexPattern
        {
            get
            {
                return ConfigurationManager.AppSettings["indexPattern"];
            }
        }

        public ExportDataForm()
        {
            InitializeComponent();
        }

        private void btnPreviewRows_Click(object sender, EventArgs e)
        {
            toggleBusyState(true);

            if (dgvPreview.DataSource != null && dgvPreview.DataSource is DataTable)
            {
                (dgvPreview.DataSource as DataTable).Dispose();
                dgvPreview.DataSource = null;
            }
            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
            }

            cancellationToken = new CancellationTokenSource();
            fetchTask = Task.Factory.StartNew(fetchSampleDataSet, cancellationToken.Token, cancellationToken.Token).ContinueWith(processSampleDataSet, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void toggleBusyState(bool busy)
        {
            pgQueryProgress.Style = ProgressBarStyle.Marquee;
            if (busy)
            {
                pgQueryProgress.Visible = true;
                txtLuceneQuery.ReadOnly = true;
                btnPreviewRows.Enabled = false;
                btnExportDataSet.Enabled = false;
                btnCancel.Enabled = true;
            }
            else
            {
                pgQueryProgress.Visible = false;
                txtLuceneQuery.ReadOnly = false;
                btnPreviewRows.Enabled = true;
                btnExportDataSet.Enabled = true;
                btnCancel.Enabled = false;
            }
        }

        private void processSampleDataSet(Task<DataFetchResult> result)
        {
            if (result.Status == TaskStatus.RanToCompletion)
            {
                dgvPreview.DataSource = result.Result.Result;
                lblTimeTook.Text = result.Result.Context.TimeTaken.TotalMilliseconds.ToString();
                lblTotalHits.Text = result.Result.Context.TotalHits.ToString();
                DataFetchResult r;
            }
            else if (result.Status == TaskStatus.Faulted)
            {
                MessageBox.Show($"REST request to elasticsearch failed\r\n{result.Exception.Message}\r\n{result.Exception.InnerException?.Message}", "Search Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            toggleBusyState(false);
        }

        private DataFetchResult fetchSampleDataSet(object tokenObj)
        {
            var token = (CancellationToken)tokenObj;

            var conf = new ConnectionConfiguration(new Uri(ClusterUrl));
            var client = new ElasticLowLevelClient(conf);

            JsonSerializer serializer = new JsonSerializer();
            serializer.DateParseHandling = DateParseHandling.DateTimeOffset;


            var response = client.SearchGet<dynamic>(IndexPattern, x => x.IgnoreUnavailable(true).AddQueryString("q", txtLuceneQuery.Text).AddQueryString("size", "500"));
            if (response.Success)
            {
                token.ThrowIfCancellationRequested();

                var table = new DataTable();
                var result = new DataFetchResult(table, new DataFetchContext() { TimeTaken = TimeSpan.FromMilliseconds(Convert.ToInt32(response.Body.took)), TotalHits = Convert.ToInt64(response.Body.hits.total) });

                foreach (var hit in response.Body.hits.hits)
                {
                    token.ThrowIfCancellationRequested();

                    Dictionary<string, string> rowSource = flattenDocumentStructure(hit._source.ToString());
                    var row = table.NewRow();
                    foreach (var item in rowSource)
                    {
                        if (!table.Columns.Contains(item.Key))
                            table.Columns.Add(item.Key);
                        row[item.Key] = item.Value;
                        token.ThrowIfCancellationRequested();
                    }
                    table.Rows.Add(row);
                }

                return result;
            }
            else
            {
                throw response.OriginalException;
            }
        }



        private void ExportDataForm_Shown(object sender, EventArgs e)
        {
            txtLuceneQuery.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            lblOperation.Text = "Cancelling...";
            if (cancellationToken != null && !cancellationToken.IsCancellationRequested)
            {
                cancellationToken.Cancel(true);
                btnCancel.Enabled = false;
            }
        }

        private void btnExportDataSet_Click(object sender, EventArgs e)
        {
            toggleBusyState(true);

            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
            }
            cancellationToken = new CancellationTokenSource();
            var fetchContext = new FetchFullDataSetContext()
            {
                Query = txtLuceneQuery.Text,
                Token = cancellationToken.Token
            };

            fetchTask = Task.Factory.StartNew(fetchEntireDataSet, fetchContext, cancellationToken.Token).ContinueWith(fetchEntireDataSetCompleted, TaskScheduler.FromCurrentSynchronizationContext());
            lblOperation.Text = "Downloading...";
        }

        private void reportProgress(int current, int max)
        {
            pgQueryProgress.Invoke(new MethodInvoker(delegate
            {
                pgQueryProgress.Style = ProgressBarStyle.Continuous;
                pgQueryProgress.Maximum = max;
                pgQueryProgress.Value = current;
                lblExportProgress.Text = $"{current} / {max} {((double)current / (double)max).ToString("P")}";
            }));
        }

        class FetchFullDataSetContext
        {
            public string Query
            {
                get;
                set;
            }

            public string CSVFileName
            {
                get;
                set;
            }

            public CancellationToken Token
            {
                get;
                set;
            }

            public List<string> TempFiles
            {
                get;
                set;
            }

            public List<string> Columns
            {
                get;
                set;
            }

            public string ScrollToken
            {
                get;
                set;
            }

            public int TotalDocuments
            {
                get;
                set;
            }

            public int ProcessedDocuments
            {
                get;
                set;
            }

            public FetchFullDataSetContext()
            {
                TempFiles = new List<string>();
                Columns = new List<string>();
            }
        }

        private void fetchEntireDataSet(object contextObj)
        {
            var context = (FetchFullDataSetContext)contextObj;

            var conf = new ConnectionConfiguration(new Uri(ClusterUrl));
            var client = new ElasticLowLevelClient(conf);

            // First establish initial search request to get a scroll token
            var searchResponse = client.SearchGet<dynamic>(IndexPattern, x => x
                .IgnoreUnavailable(true)
                .AddQueryString("scroll", "1m")
                .AddQueryString("q", context.Query)
                .AddQueryString("size", "2000"));

            if (searchResponse.Success)
            {
                context.Token.ThrowIfCancellationRequested();

                context.ScrollToken = searchResponse.Body._scroll_id;
                context.TotalDocuments = Convert.ToInt32(searchResponse.Body.hits.total);

                reportProgress(context.ProcessedDocuments, context.TotalDocuments);

                var table = new DataTable();
                bool keepRequesting = searchResponse.Body.hits.hits.Count > 0;

                using (table)
                {
                    foreach (var hit in searchResponse.Body.hits.hits)
                    {
                        context.Token.ThrowIfCancellationRequested();

                        Dictionary<string, string> rowSource = flattenDocumentStructure(hit._source.ToString());
                        var row = table.NewRow();
                        foreach (var item in rowSource)
                        {
                            if (!table.Columns.Contains(item.Key))
                                table.Columns.Add(item.Key);
                            if (!context.Columns.Contains(item.Key))
                                context.Columns.Add(item.Key);
                            row[item.Key] = item.Value;
                            context.Token.ThrowIfCancellationRequested();
                        }
                        table.Rows.Add(row);
                    }

                    string tempFilePath = Path.GetTempFileName();
                    context.TempFiles.Add(tempFilePath);
                    using (var fs = File.OpenWrite(tempFilePath))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                        bf.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
                        bf.Serialize(fs, table);
                    }

                    context.ProcessedDocuments += table.Rows.Count;
                    reportProgress(context.ProcessedDocuments, context.TotalDocuments);
                }


                // now subsequent request for another search
                do
                {
                    context.Token.ThrowIfCancellationRequested();
                    
                    var scrollRequest = client.Scroll<dynamic>(new { scroll = "1m", scroll_id = context.ScrollToken });
                    if (scrollRequest.Success)
                    {
                        // delete last scroll token
                        //var clearRequest = client.ClearScroll<dynamic>(new { scroll_id = new string[] { scrollIdToken } });
                        // if (!clearRequest.Success)
                        //    throw clearRequest.OriginalException;

                        keepRequesting = scrollRequest.Body.hits.hits.Count > 0;

                        using (table = new DataTable())
                        {
                            foreach (var hit in scrollRequest.Body.hits.hits)
                            {
                                context.Token.ThrowIfCancellationRequested();

                                Dictionary<string, string> rowSource = flattenDocumentStructure(hit._source.ToString());
                                var row = table.NewRow();
                                foreach (var item in rowSource)
                                {
                                    if (!table.Columns.Contains(item.Key))
                                        table.Columns.Add(item.Key);
                                    if (!context.Columns.Contains(item.Key))
                                        context.Columns.Add(item.Key);
                                    row[item.Key] = item.Value;

                                    context.Token.ThrowIfCancellationRequested();
                                }
                                table.Rows.Add(row);
                            }

                            string tempFilePath = Path.GetTempFileName();
                            context.TempFiles.Add(tempFilePath);
                            using (var fs = File.OpenWrite(tempFilePath))
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                                bf.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
                                bf.Serialize(fs, table);
                            }

                            context.ProcessedDocuments += table.Rows.Count;
                            reportProgress(context.ProcessedDocuments, context.TotalDocuments);
                        }
                    }
                    else
                    {
                        throw scrollRequest.OriginalException;
                    }
                } while (keepRequesting);
            }
            else
            {
                throw searchResponse.OriginalException;
            }
        }

        private Dictionary<string, string> flattenDocumentStructure(string documentJson)
        {
            using (var sr = new StringReader(documentJson))
            {
                using (var jr = new JsonTextReader(sr))
                {
                    JObject document = serializer.Deserialize<JObject>(new JsonTextReader(sr));
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

        private void fetchEntireDataSetCompleted(Task task)
        {
            toggleBusyState(false);
            lblExportProgress.Text = "Download complete";

            FetchFullDataSetContext context = (FetchFullDataSetContext)task.AsyncState;

            // cancel the scroll token
            Task.Factory.StartNew(() =>
            {
                var conf = new ConnectionConfiguration(new Uri(ClusterUrl));
                var client = new ElasticLowLevelClient(conf);
                var clearRequest = client.ClearScroll<dynamic>(new { scroll_id = new string[] { context.ScrollToken } });
            });


            bool process = task.Status == TaskStatus.RanToCompletion;

            if (task.Status == TaskStatus.Faulted)
            {
                MessageBox.Show($"Export has failed\r\n{task.Exception.Message}\r\n{task.Exception.InnerException?.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Let user process data that has already been downloaded
            if ((task.Status == TaskStatus.Faulted || task.Status == TaskStatus.Canceled) && context.ProcessedDocuments > 0)
            {
                if (MessageBox.Show($"{context.ProcessedDocuments} documents were downloaded, would you like to still process those even though this is an incomplete export?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    process = true;
                }
            }

            if (process && sfdExportCSV.ShowDialog() == DialogResult.OK)
            {
                context.CSVFileName = sfdExportCSV.FileName;
                context.TotalDocuments = context.ProcessedDocuments;
                context.ProcessedDocuments = 0;

                if (cancellationToken != null)
                {
                    cancellationToken.Dispose();
                }
                cancellationToken = new CancellationTokenSource();
                toggleBusyState(true);
                lblOperation.Text = "Merging...";
                Task.Factory.StartNew(mergeDataSet, context, cancellationToken.Token).ContinueWith(mergeDataSetCompleted, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                cleanContext(context);
            }
        }

        private void cleanContext(FetchFullDataSetContext context)
        {
            foreach (var tempFilePath in context.TempFiles)
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        private void mergeDataSet(object contextObj)
        {
            FetchFullDataSetContext context = (FetchFullDataSetContext)contextObj;

            using (var sw = new StreamWriter(context.CSVFileName, false, Encoding.UTF8))
            {
                sw.AutoFlush = true;
                using (var csv = new CsvWriter(sw))
                {
                    // write columns
                    foreach (string column in context.Columns)
                    {
                        csv.WriteField(column);
                    }
                    csv.NextRecord();

                    foreach (var tempFile in context.TempFiles)
                    {
                        using (var fs = File.OpenRead(tempFile))
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            DataTable table = (DataTable)bf.Deserialize(fs);

                            foreach (DataRow row in table.Rows)
                            {
                                foreach (var columnName in context.Columns)
                                {
                                    csv.WriteField(table.Columns.Contains(columnName) ? row[columnName] : string.Empty);
                                }
                                csv.NextRecord();
                                context.ProcessedDocuments++;

                                if (context.ProcessedDocuments % 500 == 0)
                                    reportProgress(context.ProcessedDocuments, context.TotalDocuments);
                            }
                        }
                        reportProgress(context.ProcessedDocuments, context.TotalDocuments);
                        File.Delete(tempFile);
                    }
                }
            }
        }

        private void mergeDataSetCompleted(Task task)
        {
            toggleBusyState(false);
            lblOperation.Text = "Export complete";

            FetchFullDataSetContext context = (FetchFullDataSetContext)task.AsyncState;
            cleanContext(context);

            MessageBox.Show("Processing completed", Text, MessageBoxButtons.OK);
        }
    }
}
