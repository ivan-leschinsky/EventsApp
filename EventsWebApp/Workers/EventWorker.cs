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
    }
}