using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.Elasticsearch.CSVExport
{
    /// <summary>
    /// Assembles a CSV file from multiple BatchExportPieces
    /// </summary>
    public class CsvAssembler : IDisposable
    {

        private IEnumerable<string> columns;
        private FileInfo csvFile;
        private StreamWriter csvFileStream;
        private CsvWriter csvWriter;

        public CsvAssembler(string filename, IEnumerable<string> columns)
        {
            csvFile = new FileInfo(filename);
            csvFileStream = new StreamWriter(filename, false, Encoding.UTF8) { AutoFlush = true };
            csvWriter = new CsvWriter(csvFileStream);
            this.columns = columns;

            writeColumnHeaders();
        }

        private void writeColumnHeaders()
        {
            foreach (string column in columns)
            {
                csvWriter.WriteField(column);
            }
            csvWriter.NextRecord();
        }

        /// <summary>
        /// Flushes an exported batch piece to the current CSV file
        /// </summary>
        /// <param name="piece"></param>
        public void FlushPiece(BatchExportPiece piece)
        {
            piece.Load();
            var table = piece.Table;

            foreach (DataRow row in table.Rows)
            {
                foreach (var columnName in columns)
                {
                    csvWriter.WriteField(table.Columns.Contains(columnName) ? row[columnName] : string.Empty);
                }
                csvWriter.NextRecord();

                // TODO: progress report
            }

            piece.Unload(false);
        }

        public void Dispose()
        {
            csvWriter.Dispose();
            csvFileStream.Dispose();
            columns = null;
            csvFile = null;
            csvFileStream = null;
            csvWriter = null;
        }
    }
}
