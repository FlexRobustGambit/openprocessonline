using ideas.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class Notification
    {
        public int Id { get; set; }
        public ApplicationUser Owner { get; set; }
        public ApplicationUser Initiator { get; set; }
        public NotificationType NotificationType { get; set; }
        public Idea Idea { get; set; }
        public DateTime DateTime { get; set; }
        public bool Viewed { get; set; }
        public bool? IsOwner { get; set; }

        public static void Add(NotificationType type, ApplicationUser owner, ApplicationUser initiator, Idea idea, IdeasContext context, bool viewed = false) {
            var notification = new Notification {
                Owner = owner,
                Initiator = initiator,
                NotificationType = type,
                Idea = idea,
                DateTime = DateTime.Now,
                Viewed = viewed
            };
            context.Notifications.Add(notification);
        }


        public static async Task<List<Notification>> Get(ApplicationUser owner, IdeasContext context) {
            var list = await context.Notifications
                         .Where(r => r.Owner == owner || r.Initiator == owner)
                         .OrderByDescending(r => r.DateTime)
                         .Select(r => new Notification {
                             Id = r.Id,
                             Owner = new AppUser { Id = r.Owner.Id, UserName = r.Owner.UserName},
                             Initiator = new AppUser { Id = r.Initiator.Id, UserName = r.Initiator.UserName, Image = context.Images.FirstOrDefault(z => z.Id == r.Initiator.ImageId) },
                             NotificationType = r.NotificationType,
                             Idea = r.Idea,
                             DateTime = r.DateTime,
                             Viewed = r.Viewed,
                             IsOwner = ((r.Owner == owner) ? true : false)
                         }
                         ).ToListAsync();
            return list;

        }

        public static void SetViewed(ApplicationUser owner, IdeasContext context) {
            context.Notifications.Where(r => r.Owner == owner).ToList().ForEach(r => r.Viewed = true);
        }


    }
}
