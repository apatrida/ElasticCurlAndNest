﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ElasticCommon;
using ElasticCommon.Models;
using Nest;
using Newtonsoft.Json;
using SearchRequest = ElasticCommon.Models.SearchRequest;

namespace ElasticCurl
{
    class Program
    {
        private static readonly string ElasticClusterUri = ConfigurationManager.AppSettings["ElasticClusterUri"];
        private static readonly string ElasticAdminUserName = ConfigurationManager.AppSettings["ElasticAdminUserName"];
        private static readonly string ElasticAdminPassword = ConfigurationManager.AppSettings["ElasticAdminPassword"];

        static void Main(string[] args)
        {
            var connector = new ElasticConnector();

            var client = connector.GetClient(new[] { ElasticClusterUri }, ElasticAdminUserName, ElasticAdminPassword);

            Console.WriteLine("Search Templates (T) or Suggestions (S)?");

            var searchFor = Console.ReadLine();

            Console.WriteLine("What to search for?");

            var query = Console.ReadLine();

            if (string.Equals("S", searchFor, StringComparison.OrdinalIgnoreCase))
            {
                var response = connector.GetSuggestions(client, new SearchRequest
                {
                    Query = query,
                    PageSize = 20,
                    MinScore = 0.5
                }).Result;

                Console.WriteLine("Total records {0}", response.Count);

                foreach (var groups in response.Results.GroupBy(x => x.ValueType))
                {
                    foreach (var data in groups.OrderByDescending(x => x.Score))
                    {
                        Console.WriteLine(data.ValueType + "   " + data.Value + "   " + data.Score);
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("Time taken to search Mill Seconds {0}", TimeSpan.FromTicks(response.Ticks).Milliseconds);
            }

            if (string.Equals("T", searchFor, StringComparison.OrdinalIgnoreCase))
            {
                var response = connector.GetTemplates(client, new SearchRequest
                {
                    Query = query,
                    PageSize = 18,
                    MinScore = 0.1,
                    CurrentPage = 0
                }).Result;

                Console.WriteLine("Total records {0}", response.Count);

                foreach (var data in response.Results.OrderByDescending(x=>x.Score))
                {
                    Console.WriteLine("Title    {0}", data.Title);
                    Console.WriteLine("Desc     {0}", data.Desc);
                    Console.WriteLine("Authr    {0}", data.By);
                    Console.WriteLine("TmplCode {0}", data.TmplCode);
                    Console.WriteLine("TmplTags {0}", string.Join(", ", data.TmplTags));
                    Console.WriteLine("TmplTyps {0}", string.Join(", ", data.TmplTypes));
                    Console.WriteLine("InsAuthr {0}", data.InsAuthor);
                    Console.WriteLine("Score    {0}", data.Score);
                    Console.WriteLine("Featured {0}", data.IsFeatured);
                    Console.WriteLine("Deleted  {0}", data.Deleted);

                    //Console.WriteLine(JsonConvert.SerializeObject(data));
                    Console.WriteLine();
                }

                Console.WriteLine("Time taken to search Mill Seconds {0}", TimeSpan.FromTicks(response.Ticks).Milliseconds);
            }

            Console.ReadLine();
        }
    }
}
