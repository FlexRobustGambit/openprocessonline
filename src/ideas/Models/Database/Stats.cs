using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class Stats {
        public int Id { get; set; }
        public int Favorites { get; set; }
        public int Comments { get; set; }
        public int Updates { get; set; }
        public int Views { get; set; }

        public static void AddFavorite(Idea idea) {
            if (idea.Stats == null) {
                var stats = new Stats {
                    Favorites = 1
                };
                idea.Stats = stats;
            } else {
                idea.Stats.Favorites = (idea.Stats.Favorites + 1);
            }
        }

        public static void RemoveFavorite(Idea idea) {
            if (idea.Stats != null) {
                idea.Stats.Favorites = (idea.Stats.Favorites - 1);
            }
        }

        public static void AddUpdate(Idea idea) {
            if (idea.Stats == null) {
                var stats = new Stats {
                    Updates = 1
                };
                idea.Stats = stats;
            } else {
                idea.Stats.Updates = (idea.Stats.Updates + 1);
            }
        }

        public static void RemoveUpdate(Idea idea) {
            if (idea.Stats != null) {
                idea.Stats.Updates = (idea.Stats.Updates - 1);
            }
        }

        public static void AddComments(Idea idea) {
            if (idea.Stats == null) {
                var stats = new Stats {
                    Comments = 1
                };
                idea.Stats = stats;
            } else {
                idea.Stats.Comments = (idea.Stats.Comments + 1);
            }
        }
        public static void RemoveComments(Idea idea) {
            if (idea.Stats != null) {
                idea.Stats.Comments = (idea.Stats.Comments - 1);
            }
        }

        public static void AddViews(Idea idea) {
            if (idea.Stats == null) {
                var stats = new Stats {
                    Views = 1
                };
                idea.Stats = stats;
            } else {
                idea.Stats.Views = (idea.Stats.Views + 1);
            }
        }

        public static void RemoveViews(Idea idea) {
            if (idea.Stats != null) {
                idea.Stats.Views = (idea.Stats.Views - 1);
            }
        }

    }
}
