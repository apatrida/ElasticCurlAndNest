﻿using Nest;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElasticCurl.Models
{
    [ElasticsearchType(IdProperty = "id", Name = "ts_template")]
    public class TsTemplate
    {
        public TsTemplate()
        {
            this.TmplTags = new List<string>();
            this.TmplTypes = new List<string>();
        }


        [String(Index = FieldIndexOption.NotAnalyzed, Name = "id")]
        public string Id { get; set; }


        [String(Name = "title",
            Index = FieldIndexOption.Analyzed,
            Analyzer = "suggestionAnalyzer")]
        public string Title { get; set; }


        [String(Name = "desc",
            Index = FieldIndexOption.Analyzed,
            Analyzer = "suggestionAnalyzer")]
        public string Desc { get; set; }


        [String(Name = "tmplUri",
            Index = FieldIndexOption.NotAnalyzed)]
        public string TmplUri { get; set; }


        [String(Name = "by",
            Index = FieldIndexOption.Analyzed,
            Analyzer = "suggestionAnalyzer")]
        public string By { get; set; }


        [String(Name = "byUri",
            Index = FieldIndexOption.NotAnalyzed)]
        public string ByUri { get; set; }


        [String(Name = "schlDist",
            Index = FieldIndexOption.Analyzed,
            Analyzer = "suggestionAnalyzer")]
        public string SchlDist { get; set; }


        [String(Name = "afmcCode",
            Index = FieldIndexOption.NotAnalyzed)]
        public string AfmcCode { get; set; }


        [Nested(Name = "tmplTags",
            IncludeInParent = true)]
        public List<string> TmplTags { get; set; }


        [Nested(Name = "tmplTypes",
            IncludeInParent = true)]
        public List<string> TmplTypes { get; set; }


        [String(Name = "insTmplId",
            Index = FieldIndexOption.NotAnalyzed)]
        public string InsTmplId { get; set; }


        [String(Name = "insAuthor",
            Index = FieldIndexOption.Analyzed,
            Analyzer = "suggestionAnalyzer")]
        public string InsAuthor { get; set; }


        [String(Name = "insAfmcCode",
            Index = FieldIndexOption.NotAnalyzed)]
        public string InsAfmcCode { get; set; }


        [String(Name = "insAutherUrl",
            Index = FieldIndexOption.NotAnalyzed)]
        public string InsAutherUrl { get; set; }


        [String(Name = "authorId",
            Index = FieldIndexOption.NotAnalyzed)]
        public string AuthorId { get; set; }


        [String(Name = "tmplCode",
            Index = FieldIndexOption.Analyzed,
            Analyzer = "suggestionAnalyzer")]
        public string TmplCode { get; set; }


        [String(Name = "isFeatured",
            Index = FieldIndexOption.NotAnalyzed)]
        public string IsFeatured { get; set; }

        [Boolean(Name = "deleted",
            Index = NonStringIndexOption.NotAnalyzed)]
        [JsonConverter(typeof(BoolConverter))]
        public bool Deleted { get; set; }

        public double Score { get; set; }
    }
}

