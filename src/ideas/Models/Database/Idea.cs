using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class Idea{
        public int Id { get; set; }
        public ApplicationUser Owner { get; set; }
        public Stats Stats { get; set; }
        public String Titel { get; set; }
        public String Text { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime LatestUpdate { get; set; }
        public List<Favorite> Favorites { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Image> Images { get; set; }
        public List<Update> Updates { get; set; }
        public List<Tag> Tags { get; set; }
                    

    }
}
