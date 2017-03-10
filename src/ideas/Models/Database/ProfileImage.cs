using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class ProfileImage
    {
        public int Id { get; set; }
        public ApplicationUser Owner { get; set; }
        public Image Image { get; set; }
    }
}
