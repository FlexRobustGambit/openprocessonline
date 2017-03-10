using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class Comment {
        public int Id { get; set; }
        public ApplicationUser Owner { get; set; }
        public Comment RepliedTo { get; set; }
        public String Text { get; set; }
        public DateTime DateTime { get; set; }
        public bool? IsOwner { get; set; }

        public static void Add(ApplicationUser owner , Idea idea , string text) {
            var comment = new Comment {
                Owner = owner,
                Text = text,
                DateTime = DateTime.Now
            };
            idea.Comments.Add(comment);
        }

        public static bool Remove(int id, ApplicationUser owner, IdeasContext context ) {
           var comment =  context.Comments.FirstOrDefault(r => r.Id == id && r.Owner == owner);
            if (comment != null) {
                context.Comments.Remove(comment);
                return true;
            }
            return false;
        }
    }
}
