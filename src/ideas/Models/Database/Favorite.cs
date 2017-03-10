using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class Favorite
    {
        public int Id { get; set; }
        public Idea Idea { get; set; }
        public ApplicationUser Owner { get; set; }
        public DateTime DateTime { get; set; }


        public static Favorite Add(Idea idea, ApplicationUser owner , IdeasContext context) {
            var favorite = default(Favorite);
            try {
                if (idea != null) {
                    favorite = new Favorite {
                        Idea = idea,
                        DateTime = DateTime.Now,
                        Owner = owner
                    };
                    Stats.AddFavorite(idea);
                    context.Favorites.Add(favorite);
                }
            } catch (Exception ex) {
                var exs = ex;
            }
            return favorite;
        }

     
    }
}
