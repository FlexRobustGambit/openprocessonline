using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ideas.Models.Database;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ideas.Models
{
    public class ApplicationUser : IdentityUser{
        public virtual int? ImageId { get; set;  }
        public virtual int? UserStatsId { get; set; }
           
        public virtual Image Image { get; set; }
        public virtual UserStats UserStats { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime Created { get; set; }
     
    }
}
