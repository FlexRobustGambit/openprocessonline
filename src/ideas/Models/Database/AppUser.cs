using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class AppUser : ApplicationUser {

        public DateTime Joined { get; set; }

        public static AppUser GetByUserName(string name, IdeasContext context) {
            var appUser = context.ApplicationUsers
                .Include(r => r.Image)
                .Include(r => r.UserStats)
                .Where(r => r.NormalizedUserName == name.ToUpper())
                .Select(r => new AppUser { Id = r.Id, UserName = r.UserName, Image = r.Image, Joined = r.Created, Description = r.Description, UserStats = r.UserStats })
                .FirstOrDefault();
            if (appUser.UserStats == null) {
                appUser.UserStats = new UserStats();
            }
            return appUser;
        }

        public static AppUser ApplicationUserToAppUser(ApplicationUser user) {
            var appuser = new AppUser {
                Id = user.Id,
                UserName = user.UserName,
                UserStats = user.UserStats,
                Image = user.Image,
                Joined = user.Created,
                Description = user.Description
            };
            return appuser;
        }
    }
}
