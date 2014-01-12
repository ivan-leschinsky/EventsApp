using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsWebApp.Models
{
    public class SearchModel
    {
        public string SearchWord { get; set; }
        public string ResultsCount { get; set; }
        public ICollection<Event_> Events { get; set; }
        public SearchModel(string searchWord, string resultsCount, ICollection<Event_> events)
        {
            this.ResultsCount = resultsCount;
            this.SearchWord = searchWord;
            this.Events = events;
        }
    }
}