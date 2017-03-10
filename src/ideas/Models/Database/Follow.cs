using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class Follow {
        public int Id { get; set; }
        public ApplicationUser Track { get; set; }
        public ApplicationUser Owner { get; set; }
        public DateTime DateTime { get; set; }


        public static async Task<List<Follow>> Get(ApplicationUser owner, IdeasContext context){
            var list = await context.Followers.Include(r => r.Owner).Include(r => r.Track).Where(r => r.Track == owner).ToListAsync();
            return list;                        
        }
    }
}
