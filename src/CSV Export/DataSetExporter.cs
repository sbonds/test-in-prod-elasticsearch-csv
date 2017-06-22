using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypton.Elasticsearch.CSVExport
{

    public delegate void DataSetExporterReportProgressCallback(int current, int total);

    /// <summary>
    /// Provides a batch-export functionality for exporting ES data to CSV
    /// </summary>
    public class DataSetExporter : DataSetFetcher
    {

        private TaskScheduler sourceScheduler = null;



        public DataSetExporter()
        {
            sourceScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        protected async Task clearScrollRequest(DataFetchContext context)
        {
            var client = base.getClient();
            var clearRequest = client.ClearScroll<dynamic>(new { scroll_id = new string[] { context.ScrollToken } });
            if (clearRequest.Success)
            {
                context.Scroll = false;
                context.ScrollToken = null;
            }
            else
            {
                throw clearRequest.OriginalException;
            }
        }

        /// <summary>
        /// Performs a batch export returning assembly of several pieces of smaller batches
        /// </summary>
        /// <param name="token"></param>
        /// <param name="luceneQuery"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public async Task<BatchExportResult> BatchExport(CancellationToken token, string luceneQuery = "*", DataSetExporterReportProgressCallback progressCallback = null)
        {
            return await Task.Run(async () =>
            {
                var context = new DataFetchContext();
                context.LuceneQuery = luceneQuery;
                context.MaxBatchSize = MaxBatchSize;
                context.Scroll = true;

                var export = new BatchExportResult(context);
                var firstBatch = new BatchExportPiece();
                export.Pieces.Add(firstBatch);

                int docsTotal, docsProcessed;

                try
                {
                    // fill data from first batch
                    foreach (var hit in base.elasticsearchGetSearch(context))
                    {
                        token.ThrowIfCancellationRequested();
                        flattenDocumentToRow(firstBatch.Table, hit);
                    }

                    // add columns to context
                    foreach (DataColumn col in firstBatch.Table.Columns)
                    {
                        if (!context.Columns.Contains(col.ColumnName))
                            context.Columns.Add(col.ColumnName);
                    }

                    unchecked
                    {
                        docsTotal = (int)context.TotalHits;
                        docsProcessed = firstBatch.Table.Rows.Count;
                    }
                    if (progressCallback != null)
                    {
                        await Task.Factory.StartNew(() => { progressCallback(docsProcessed, docsTotal); }, CancellationToken.None, TaskCreationOptions.None, sourceScheduler);
                    }

                    // flush to disk and allow GC to collect memory
                    firstBatch.Unload();

                    // now get all the other batches
                    while (context.Scroll) // elasticsearchGetScroll will set Scroll to false at the end
                    {
                        var batchPiece = new BatchExportPiece();
                        export.Pieces.Add(batchPiece);
                        foreach (var hit in base.elasticsearchGetScroll(context))
                        {
                            token.ThrowIfCancellationRequested();
                            flattenDocumentToRow(batchPiece.Table, hit);
                        }

                        // add columns to context
                        foreach (DataColumn col in batchPiece.Table.Columns)
                        {
                            if (!context.Columns.Contains(col.ColumnName))
                                context.Columns.Add(col.ColumnName);
                        }

                        unchecked
                        {
                            docsProcessed += batchPiece.Table.Rows.Count;
                        }
                        if (progressCallback != null)
                        {
                            await Task.Factory.StartNew(() => { progressCallback(docsProcessed, docsTotal); }, CancellationToken.None, TaskCreationOptions.None, sourceScheduler);
                        }

                        batchPiece.Unload();
                    }
                }
                catch
                {
                    export.Dispose();
                    throw;
                }
                finally
                {
                    await clearScrollRequest(context);
                }

                return export;
            });
        }

    }
}
