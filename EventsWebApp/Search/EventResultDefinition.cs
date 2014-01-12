using EventsWebApp.Models;
using Lucene.Net.Documents;
using SimpleLucene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsWebApp.Search
{
    public class EventResultDefinition : IResultDefinition<Event_>
    {
        public Event_ Convert(Document document)
        {
            var event_ = new Event_();
            event_.Event_Id = document.GetValue<int>("Event_Id");
            event_.EventName = document.GetValue("EventName");
            event_.EventDescription = document.GetValue("EventDescription");
            return event_;
        }
    }
}