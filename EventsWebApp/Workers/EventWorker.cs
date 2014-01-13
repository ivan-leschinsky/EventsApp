using EventsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsWebApp.Workers
{
    public static class eWorker
    {
        public static bool IsEventIcludeUserInterests(UserProfile user, Event_ event_)
        {
            foreach (string userInterest in user.Subjects.Split(','))
            {
                if (event_.Subjects.Contains(userInterest))
                {
                    return true;
                }

            }
            return false;
        }
        public static bool CheckForArchive(Event_ e)
        {
            if (e.EventTime <= DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static List<Event_> GetEventsByInterests(List<Event_> events, UserProfile currentuser)
        {
            List<Event_> trueEvents = new List<Event_>(); 

            foreach (var event_ in events)
            {
                foreach (string userInterest in currentuser.Subjects.Split(','))
                {
                    if (event_.Subjects.Contains(userInterest))
                    {
                        trueEvents.Add(event_);
                    }
                }
            }
            return trueEvents; 
        }


    }
}