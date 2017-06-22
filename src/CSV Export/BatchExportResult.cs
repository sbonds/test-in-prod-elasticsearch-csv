using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.Elasticsearch.CSVExport
{
    public class BatchExportResult : IDisposable
    {

        public List<BatchExportPiece> Pieces
        {
            get;
            private set;
        }

        public DataFetchContext Context
        {
            get;
            private set;
        }

        public BatchExportResult(DataFetchContext context)
        {
            Context = context;
            Pieces = new List<BatchExportPiece>();
        }

        bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                foreach (var item in Pieces)
                {
                    item.Dispose();
                }
                Pieces.Clear();
                Context = null;
                Pieces = null;
                disposed = true;
            }
        }
    }
}
