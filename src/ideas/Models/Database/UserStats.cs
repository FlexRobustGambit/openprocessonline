using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class UserStats{
        public int Id { get; set; }
        public int Projects { get; set; }
        public int Comments { get; set; }
        public int Favorites { get; set; }
        public int Favorited { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }

        public static void AddProject(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats == null) {
                user.UserStats = new UserStats { Projects = 1 };
            } else {
                user.UserStats.Projects = (user.UserStats.Projects + 1);
            }
        }
        public static void RemoveProject(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats != null && user.UserStats.Projects > 0) {
                user.UserStats.Projects = (user.UserStats.Projects - 1);
            }
        }

        public static void AddComment(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats == null) {
                user.UserStats = new UserStats { Comments = 1 };
            } else {
                user.UserStats.Comments = (user.UserStats.Comments + 1);
            }
        }

        public static void RemoveComment(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats != null && user.UserStats.Comments > 0) {
                user.UserStats.Comments = (user.UserStats.Comments - 1);
            }
        }

        public static void AddFavorite(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats == null) {
                user.UserStats = new UserStats { Favorites = 1 };
            } else {
                user.UserStats.Favorites = (user.UserStats.Favorites + 1);
            }
        }

        public static void RemoveFavorite(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats != null && user.UserStats.Favorites > 0) {
                user.UserStats.Favorites = (user.UserStats.Favorites - 1);
            }
        }

        public static void AddFavorited(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats == null) {
                user.UserStats = new UserStats { Favorited = 1 };
            } else {
                user.UserStats.Favorited = (user.UserStats.Favorited + 1);
            }
        }

        public static void RemoveFavorited(ApplicationUser user,  IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats != null && user.UserStats.Favorited > 0) {
                user.UserStats.Favorited = (user.UserStats.Favorited - 1);
            }
        }

        public static void RemoveFavorited(ApplicationUser user, IdeasContext context, int amount) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats != null && (user.UserStats.Favorited - amount) >= 0) {
                user.UserStats.Favorited = (user.UserStats.Favorited - amount);
            }
        }

        public static void AddFollower(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats == null) {
                user.UserStats = new UserStats { Followers = 1 };
            } else {
                user.UserStats.Followers = (user.UserStats.Followers + 1);
            }
        }

        public static void RemoveFollower(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats != null && user.UserStats.Followers > 0) {
                user.UserStats.Followers = (user.UserStats.Followers - 1);
            }
        }

        public static void AddFollowing(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats == null) {
                user.UserStats = new UserStats { Following = 1 };
            } else {
                user.UserStats.Following = (user.UserStats.Following + 1);
            }
        }

        public static void RemoveFollowing(ApplicationUser user, IdeasContext context) {
            if (user.UserStats == null) {
                user = context.ApplicationUsers.Include(r => r.UserStats).Where(r => r.Id == user.Id).FirstOrDefault();
            }
            if (user.UserStats != null && user.UserStats.Following > 0) {
                user.UserStats.Following = (user.UserStats.Following - 1);
            }
        }
    }
}
