using EventsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleLucene.Impl;
using SimpleLucene.IndexManagement;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using System.IO;


namespace EventsWebApp.Workers
{
    public static class iWorker
    {
        public static void CreateIndex(Event_ entity, string IndexPath)
        {
            var document = new Document();
            document.Add(new Field("Event_Id", entity.Event_Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("EventName", entity.EventName, Field.Store.YES, Field.Index.ANALYZED));
            if (!string.IsNullOrEmpty(entity.EventDescription))
            {
                document.Add(new Field("EventDescription", entity.EventDescription, Field.Store.YES, Field.Index.ANALYZED));
            }

            Directory directory = FSDirectory.Open(new DirectoryInfo(IndexPath));
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);

            var writer = new IndexWriter(directory, analyzer, false, IndexWriter.MaxFieldLength.LIMITED);
            writer.AddDocument(document);

            writer.Optimize();
            writer.Dispose();
        }

        public static List<Event_> SearchEvents(string IndexPath, string searchString, IEvent_Repository event_Repository)
        {
            var indexSearcher = new DirectoryIndexSearcher(new DirectoryInfo(IndexPath), true);
            Directory directory = FSDirectory.Open(new DirectoryInfo(IndexPath));
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            IndexReader indexReader = IndexReader.Open(directory, true);
            Searcher indexSearch = new IndexSearcher(indexReader);
            string[] fields = { "EventName", "EventDescription" };
            var queryParser = new Lucene.Net.QueryParsers.MultiFieldQueryParser(Version.LUCENE_30, fields, analyzer);
            var query = queryParser.Parse(searchString.ToLower() + "*");
            var hits = indexSearch.Search(query, indexReader.MaxDoc).ScoreDocs;
            List<Event_> events = new List<Event_>();
            foreach (var hit in hits)
            {
                Document documentFromSearcher = indexSearch.Doc(hit.Doc);
                events.Add(event_Repository.Find(int.Parse(documentFromSearcher.Get("Event_Id"))));
            }
            indexSearch.Dispose();
            directory.Dispose();
            return events;
        }




    }
}