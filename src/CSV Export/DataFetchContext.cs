using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.Elasticsearch.CSVExport
{
    public class DataFetchContext
    {

        public string LuceneQuery
        {
            get;
            set;
        }

        public int MaxBatchSize
        {
            get;
            set;
        }

        public bool Scroll
        {
            get;
            set;
        }

        public string ScrollToken
        {
            get;
            set;
        }
        
        public long TotalHits
        {
            get;
            set;
        }

        public TimeSpan TimeTaken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of all known columns in the current context
        /// </summary>
        public List<string> Columns
        {
            get;
            private set;
        }

        public DataFetchContext()
        {
            Columns = new List<string>();
        }

    }
}
