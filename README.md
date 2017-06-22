# elasticsearch-csv
Windows utility to export elasticsearch query results to CSV files

This is a "quick and dirty" utility I created a while ago that lets a user export 
Lucene query results from elasticsearch to a generic CSV file to then pass that onto their
own workflow.

This utility does not limit how much data can be exported (although such limits may be imposed
by your running instance of elasticsearch and available disk space, network bandwidth, and memory).

The tool will automatically discover variances in document structure (e.g. certain document `_type` in
elasticsearch will have differences in available properties since they are after all, json documents).

Furthermore, the tool will attempt to flatten hierarchial structure of JSON documents into CSV columns.
Therefore, for complex or big documents you'll get a lot of columns.

## Configuration

Open `es-csv-export.exe.config` supplied with the executable and adjust these settings:

  - `clusterUrl` = HTTP cluster URL of your elasticsearch instance. This can be individual node (for example,
			     a read-only node or data node). Elasticsearch must have http enabled as a transport
				 
  - `indexPattern` = Index pattern to query indices. This is similar to what you specify in kibana when
					 setting up a search.
					 
Run `es-csv-export.exe`.

The 500-row preview can give you a quick glimpse at the data and it is equivalent to your basic
kibana "Discover" search. The full export can take a long time depending on how much data you have.

The Lucene query (textbox) can be tuned to achive a smaller subset of data or filtering.