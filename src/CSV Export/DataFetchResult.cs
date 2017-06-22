using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.Elasticsearch.CSVExport
{

    public class DataFetchResult : IDisposable
    {

        public DataTable Result
        {
            get;
            private set;
        }

        public DataFetchContext Context
        {
            get;
            private set;
        }

        public DataFetchResult(DataTable source, DataFetchContext context)
        {
            Result = source;
            Context = context;
        }
        
        public void Dispose()
        {
            Result.Dispose();
        }
    }
}
