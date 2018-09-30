using System;
using Google.Cloud.BigQuery.V2;

namespace BigQueryDemo
{
    class Program
    {
    	const string PROJECT_ID = "testproject-garrett";
    	
        static void Main(string[] args)
        {
			var client = BigQueryClient.Create(PROJECT_ID);
            var table = client.GetTable("bigquery-public-data", "samples", "shakespeare");
            var sql = $"SELECT corpus AS title, COUNT(word) AS unique_words FROM {table} GROUP BY title ORDER BY unique_words DESC LIMIT 10";

            var results = client.ExecuteQuery(sql, parameters: null);

            foreach (var row in results)
            {
                Console.WriteLine($"{row["title"]}: {row["unique_words"]}");
            }
            
            Print_GithubPublicData();
            
            LoadDataToBigQuery();	
        }
        
        static void LoadDataToBigQuery() {
        	var gcsUri = "gs://cloud-samples-data/bigquery/us-states/us-states.json";
            var client = BigQueryClient.Create(PROJECT_ID);
            var dataset = client.GetOrCreateDataset("us_states_dataset");

            var schema = new TableSchemaBuilder 
            {
                { "name", BigQueryDbType.String },
                { "post_abbr", BigQueryDbType.String }
            }.Build();

            var jobOptions = new CreateLoadJobOptions
            {
                SourceFormat = FileFormat.NewlineDelimitedJson
            };

            var table = dataset.GetTableReference("us_states_table");
            var loadJob = client.CreateLoadJob(gcsUri, table, schema, jobOptions);

            loadJob.PollUntilCompleted();
            loadJob.ThrowOnAnyError();
            Console.WriteLine("Json file loaded to BigQuery");
        }
        
        static void Print_GithubPublicData() 
        {
        	var client = BigQueryClient.Create(PROJECT_ID);
            var table = client.GetTable("bigquery-public-data", "github_repos", "commits");
            
            var sql = $"SELECT subject AS subject, COUNT(*) AS num_duplicates FROM {table} GROUP BY subject ORDER BY num_duplicates DESC LIMIT 10";

            var queryOptions = new QueryOptions {
                UseQueryCache = true
            };

            var results = client.ExecuteQuery(sql, parameters: null, queryOptions: queryOptions);

            foreach (var row in results)
            {
                Console.WriteLine($"{row["subject"]}: {row["num_duplicates"]}");
            }

            var job = client.GetJob(results.JobReference);
            var stats = job.Statistics;
            Console.WriteLine("----------");
            Console.WriteLine($"Creation time: {stats.CreationTime}");
            Console.WriteLine($"End time: {stats.EndTime}");
            Console.WriteLine($"Total bytes processed: {stats.TotalBytesProcessed}");
        }
    }
}
