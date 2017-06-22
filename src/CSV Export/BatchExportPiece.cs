using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.Elasticsearch.CSVExport
{
    /// <summary>
    /// Represents a piece in the batch export
    /// </summary>
    public class BatchExportPiece : IDisposable
    {

        /// <summary>
        /// Gets the data in memory
        /// </summary>
        public DataTable Table
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets information about the temporary file
        /// </summary>
        public FileInfo TempFile
        {
            get;
            private set;
        }

        /// <summary>
        /// Allocates a temporary file and allocates a new instance of BatchExportPiece
        /// </summary>
        public BatchExportPiece()
        {
            Table = new DataTable();
            TempFile = new FileInfo(Path.GetTempFileName());
        }

        /// <summary>
        /// Loads serialized DataTable from the TempFile into memory
        /// </summary>
        public void Load()
        {
            if (Table != null)
            {
                Table.Dispose();
            }

            using (var fs = File.OpenRead(TempFile.FullName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Table = (DataTable)bf.Deserialize(fs);
            }
        }

        /// <summary>
        /// Saves the currently loaded DataTable to disk, Disposing the loaded DataTable
        /// </summary>
        public void Unload(bool save = true)
        {
            if (Table == null)
                throw new InvalidOperationException("Table is not loaded");
            if (save)
            {
                if (TempFile.Exists)
                    TempFile.Delete();
                using (var fs = File.OpenWrite(TempFile.FullName))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                    bf.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
                    bf.Serialize(fs, Table);
                }
            }
            Table.Dispose();
            Table = null;
        }

        /// <summary>
        /// Disposes the current Table and deletes the associated TempFile
        /// </summary>
        public void Dispose()
        {
            if (Table != null)
            {
                Table.Dispose();
                Table = null;
            }
            if (TempFile.Exists)
            {
                TempFile.Delete();
            }
        }
    }
}
