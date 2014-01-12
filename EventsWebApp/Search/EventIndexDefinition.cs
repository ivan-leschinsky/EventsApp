using EventsWebApp.Models;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using SimpleLucene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsWebApp.Search
{
    public class EventIndexDefinition: IIndexDefinition<Event_> {
        
    public Document Convert(Event_ entity)
    {
        var document = new Document();
        document.Add(new Field("Event_Id", entity.Event_Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
        document.Add(new Field("EventName", entity.EventName, Field.Store.YES, Field.Index.ANALYZED));
        if (!string.IsNullOrEmpty(entity.EventDescription))
        {
            document.Add(new Field("EventDescription", entity.EventDescription, Field.Store.YES, Field.Index.ANALYZED));
        }
        
        return document;
    }

    public Term GetIndex(Event_ entity)
    {
        return new Term("Event_Id", entity.Event_Id.ToString());
    }




}




}