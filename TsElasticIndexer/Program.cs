﻿using System;
using System.Configuration;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using TsElasticCommon;
using TsElasticCommon.Models;

namespace TsElasticIndexer
{
    public class Program
    {
        private static readonly string EndpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        private static readonly string AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["DatabaseId"];
        private static readonly string SuggestionCollectionId = ConfigurationManager.AppSettings["SuggestionCollectionId"];
        private static readonly string TemplateCollectionId = ConfigurationManager.AppSettings["TemplateCollectionId"];

        private static DocumentClient _documentClient;
        static void Main(string[] args)
        {
            using (_documentClient = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
            {
                //ensure the database & collection exist before running samples
                Init();

                //get all suggestions changed in last 10 mins and update them
                UpdateSuggestionIndex(DatabaseId, SuggestionCollectionId);

                //get all templates changed in last x mins and update them
                UpdateTemplateIndex(DatabaseId, TemplateCollectionId);
            }
        }

        private static void Init()
        {
            GetDatabase(DatabaseId);

            GetCollection(DatabaseId, SuggestionCollectionId);
        }

        private static Database GetDatabase(string databaseId)
        {
            var database = _documentClient.CreateDatabaseQuery()
                .Where(db => db.Id == databaseId)
                .ToArray()
                .FirstOrDefault();

            if (database == null)
            {
                throw new Exception("Could not find Document Db database");
            }

            return database;
        }

        private static DocumentCollection GetCollection(string databaseId, string collectionId)
        {
            var databaseUri = UriFactory.CreateDatabaseUri(databaseId);

            var collection = _documentClient.CreateDocumentCollectionQuery(databaseUri)
                .Where(c => c.Id == collectionId)
                .AsEnumerable()
                .FirstOrDefault();

            if (collection == null)
            {
                throw new Exception("Could not find Document Db Collection");
            }

            return collection;
        }

        private static void UpdateSuggestionIndex(string databaseId, string collectionId)
        {
            var elasticConnector = new ElasticConnector();

            var elasticClient = elasticConnector.GetClient();

            // form documentDb collection uri
            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            var timeToGoBackFrom = DateTime.UtcNow.AddMinutes(-20).ToEpoch();

            //build up the query string
            var sql = string.Format("SELECT * FROM c where c._ts >= {0}", timeToGoBackFrom);

            //Get all the updated suggestions in last 10 mins
            var documents = _documentClient.CreateDocumentQuery(collectionLink, sql)
                .ToList();

            foreach (var d in documents)
            {
                var suggestion  = JsonConvert.DeserializeObject<TsSuggestion>(d.ToString());

                //Updating the suggestion index
                elasticConnector.IndexSuggestionDocument(elasticClient, suggestion);
            }
        }

        private static void UpdateTemplateIndex(string databaseId, string collectionId)
        {
            var elasticConnector = new ElasticConnector();

            var elasticClient = elasticConnector.GetClient();

            // form documentDb collection uri
            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            var timeToGoBackFrom = DateTime.UtcNow.AddMinutes(-20).ToEpoch();

            //build up the query string
            var sql = string.Format("SELECT * FROM c where c._ts >= {0}", timeToGoBackFrom);

            //Get all the updated templates in last 10 mins
            var documents = _documentClient.CreateDocumentQuery(collectionLink, sql)
                .ToList();

            foreach (var d in documents)
            {
                var template = JsonConvert.DeserializeObject<TsTemplate>(d.ToString());

                //Updating the templates index
                elasticConnector.IndexTemplateDocument(elasticClient, template);
            }
        }
    }
}
