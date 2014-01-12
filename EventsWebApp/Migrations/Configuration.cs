using System.Data.Entity.Migrations;

namespace EventsWebApp.Migrations
{
    using EventsWebApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EventsWebApp.Models.EventsAppDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EventsWebApp.Models.EventsAppDb context)
        {
            
            var subjects = new List<Subject>
            {
                
                new Subject {SubjectId = 1, SubjectName = "Авто"},
                new Subject {SubjectId = 2,  SubjectName = "Компьютеры"},
                new Subject {SubjectId = 3,  SubjectName = "Цветы"},
                new Subject {SubjectId = 4,  SubjectName = "Музыка"},
                new Subject {SubjectId = 5,  SubjectName = "Кино"},
                new Subject {SubjectId = 6,  SubjectName = "Программирование"},
                new Subject {SubjectId = 7,  SubjectName = ".NET MVC 4"},
                new Subject {SubjectId = 8,  SubjectName = "Ruby On Rails"},
                new Subject {SubjectId = 9,  SubjectName = "Java EE"},
                new Subject {SubjectId = 10,  SubjectName = "PHP 5.5"}
            };

            subjects.ForEach(s => context.Subjects.AddOrUpdate(p => p.SubjectName, s));
            context.SaveChanges();
            
        }
    }
}
