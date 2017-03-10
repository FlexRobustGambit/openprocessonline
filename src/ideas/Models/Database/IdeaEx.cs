using ideas.Models.IdeasViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ideas.Models.Enum;

namespace ideas.Models.Database
{
    public class IdeaEx {
        public Idea Idea  { get; set; }
        public bool AddedToFavorites { get; set; }
        public bool Authorized { get; set; }
        public bool IsOwner { get; set; }
        public onFeedReason Reason { get; set; }

        public static async Task<List<IdeaEx>> Search(String searchWord, ApplicationUser owner, IdeasContext context) {
            searchWord = searchWord.ToLower();
            var ideas = await context.Ideas
                .Where(s =>
                    s.Titel.ToLower().Contains(searchWord) ||
                    s.Text.ToLower().Contains(searchWord) ||
                    ((s.Updates.Where(u => u.Text.ToLower().Contains(searchWord)).FirstOrDefault() != null) ? true : false) ||
                    ((s.Tags.Where(u => u.Value.ToLower().Contains(searchWord)).FirstOrDefault() != null) ? true : false)
                    )
                .Select(pr => new Idea {
                    Id = pr.Id,
                    Owner = new AppUser { Id = pr.Owner.Id, UserName = pr.Owner.UserName, Image = context.Images.FirstOrDefault(r => r.Id == pr.Owner.ImageId) },
                    Titel = pr.Titel,
                    Text = pr.Text,
                    Stats = pr.Stats,
                    DateTime = pr.DateTime,
                    Favorites = pr.Favorites.Select(r => new Favorite { Owner = r.Owner }).ToList(),
                    LatestUpdate = pr.LatestUpdate,
                    Updates = pr.Updates.OrderBy(r => r.DateTime).Select(r => new Update { Id = r.Id, Text = r.Text, DateTime = r.DateTime, Images = r.Images }).ToList(),
                    Images = pr.Images,
                    Tags = pr.Tags
                }
            )
            .ToListAsync();

            var ideaEx = ideas.Select(r =>
                   new IdeaEx {
                       Idea = r,
                       AddedToFavorites = r.Favorites.Any(a => a.Owner == owner),
                       Authorized = ((owner == null) ? false : true),
                       IsOwner = ((((owner != null)) && (owner.Id == r.Owner.Id)) ? true : false)
                   }).ToList();
            return ideaEx;
        }

        public static IdeaEx IdeaByIdAndOwner(int ideaId, ApplicationUser owner, IdeasContext context) {
            var idea = context.Ideas
                 .Where(r => r.Id == ideaId && r.Owner == owner)
                 .Select(pr => new Idea {
                     Id = pr.Id,
                     Owner = new AppUser { Id = pr.Owner.Id, UserName = pr.Owner.UserName, Image = context.Images.FirstOrDefault(r => r.Id == pr.Owner.ImageId), Joined = pr.Owner.Created, Description = pr.Owner.Description, UserStats = context.UserStats.FirstOrDefault(r => r.Id == pr.Owner.UserStatsId) },
                     Titel = pr.Titel,
                     Text = pr.Text,
                     Stats = pr.Stats,
                     DateTime = pr.DateTime,
                     LatestUpdate = pr.LatestUpdate,
                     Favorites = pr.Favorites.Select(r => new Favorite { Owner = r.Owner }).ToList(),
                     Updates = pr.Updates.OrderBy(r => r.DateTime).Select(r => new Update { Id = r.Id, Text = r.Text, DateTime = r.DateTime, Images = r.Images }).ToList(),
                     Comments = pr.Comments.OrderBy(r => r.DateTime)
                            .Select(r =>
                                new Comment {
                                    Owner = new AppUser { UserName = r.Owner.UserName, Image = context.Images.FirstOrDefault(x => x.Id == r.Owner.ImageId), Joined = r.Owner.Created, Description = r.Owner.Description, UserStats = context.UserStats.FirstOrDefault(x => x.Id == r.Owner.UserStatsId) },
                                    Text = r.Text,
                                    DateTime = r.DateTime
                                })
                                .ToList(),
                     Images = pr.Images.Select(r => new Image { Id = r.Id , FileName = r.FileName, Height = r.Height, Width = r.Width, OriginName = r.OriginName , DateTime = r.DateTime }).ToList(),
                     Tags = pr.Tags
                 } 
             ).FirstOrDefault();
            var ideaEx = new IdeaEx {
                Idea = idea,
                AddedToFavorites = idea.Favorites.Any(a => a.Owner == owner),
                Authorized = ((owner == null) ? false : true),
                IsOwner = ((owner.Id == idea.Owner.Id) ? true : false)
            };
            return ideaEx;
        }

        public static async Task<Idea> NewIdea(NewIdeaViewModel vm, ApplicationUser owner, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            var AddThis = Image.GetNewAndRemoveOld(vm.JsonImagesNew, vm.JsonImagesRemove, 0, 0, owner, hostingEnvironment, context).Result;
            Idea idea = new Idea {
                Owner = owner,
                Titel = vm.Titel.Trim(),
                Text = vm.Text.Trim(),
                DateTime = DateTime.Now,
                LatestUpdate = DateTime.Now,
                Tags = ((vm.JsonTags != null) ? JsonConvert.DeserializeObject<List<Tag>>(vm.JsonTags) : null)
            };
            if (AddThis != null) {
                AddThis = UnsavedImage.GetList(AddThis.Select(r => r.FileName).ToList(), context, owner);
                if (idea.Images != null) {
                    foreach (Image image in AddThis) {
                        idea.Images.Add(image);
                    }
                } else {
                    idea.Images = AddThis;
                }
                UnsavedImage.RemoveList(AddThis.Select(r => r.FileName).ToList(), context, owner);
            }
            UserStats.AddProject(owner, context);
            context.Ideas.Add(idea);
            return idea;
        }

        public static async Task<int> Remove(ApplicationUser owner, ListViewModel vm, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            var amountoffavs = 0;
            var idea = await context.Ideas
                   .Where(r => r.Id == vm.IdeaId && r.Owner == owner)
                   .Select(pr => new Idea {
                       Id = pr.Id,
                       Titel = pr.Titel,
                       Text = pr.Text,
                       Stats = pr.Stats,
                       DateTime = pr.DateTime,
                       LatestUpdate = pr.LatestUpdate,
                       Favorites = pr.Favorites,
                       Updates = pr.Updates.Select(z => new Update { Id = z.Id , DateTime  = z.DateTime , Text = z.Text , Images = z.Images }).ToList(),
                       Comments = pr.Comments,
                       Images = pr.Images,
                       Tags = pr.Tags
                   }
               ).FirstOrDefaultAsync();

            if (idea != null) {
                try {
                     amountoffavs = idea.Favorites.Count;

                    List<Image> listofimages = new List<Image>();
                    listofimages.AddRange(idea.Images);
                   
                    foreach (Models.Database.Update update in idea.Updates) {
                        listofimages.AddRange(update.Images);
                    }

                    foreach (Image image in listofimages) {
                        await Image.DeleteFromServer(image.FileName, context, hostingEnvironment);
                    }
                    context.Images.RemoveRange(listofimages);

                    if(idea.Stats != null)
                    context.Stats.Remove(idea.Stats);
                    context.Tags.RemoveRange(idea.Tags);
                    context.Comments.RemoveRange(idea.Comments);
                    context.Updates.RemoveRange(idea.Updates);
                    context.Favorites.RemoveRange(idea.Favorites);
                    context.Ideas.Remove(idea);
                    await context.SaveChangesAsync();
                } catch (Exception ex) {
                    ex = ex;
                }
            }
            return amountoffavs;
        }

        public static async Task UpdateIdea(ListViewModel vm ,  ApplicationUser owner, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            var idea = context.Ideas.Include(r => r.Tags).FirstOrDefault(r => r.Id == vm.IdeaId && r.Owner == owner);
            if (idea != null) {
                var jsonRemove  = ((vm.EditIdeaViewModel.JsonImagesRemove != null) ? JsonConvert.DeserializeObject<List<Image>>(vm.EditIdeaViewModel.JsonImagesRemove) : null);
                var jsonNew = ((vm.EditIdeaViewModel.JsonImagesNew != null) ? JsonConvert.DeserializeObject<List<Image>>(vm.EditIdeaViewModel.JsonImagesNew) : null);
                var AddThis = cleanUnSavedImage(jsonNew, jsonRemove, owner, context , hostingEnvironment).Result;
                finishRemoveList(vm.IdeaId, jsonRemove, context, hostingEnvironment);
            
                try {
                    var jsonTagsRemove = ((vm.EditIdeaViewModel.JsonTagsRemove != null) ? JsonConvert.DeserializeObject<List<Tag>>(vm.EditIdeaViewModel.JsonTagsRemove) : null);
                    var jsonTagsNew = ((vm.EditIdeaViewModel.JsonTags != null) ? JsonConvert.DeserializeObject<List<Tag>>(vm.EditIdeaViewModel.JsonTags) : null);

                    idea.Text = vm.EditIdeaViewModel.Text.Trim();
                    idea.Titel = vm.EditIdeaViewModel.Titel.Trim();
                    if (jsonTagsRemove != null) {
                        var remove = context.Tags.Where(r => jsonTagsRemove.Any(x=> x.Value == r.Value)).ToList();
                        idea.Tags.RemoveAll(r => remove.Any(s => s.Value == r.Value));
                    }
                    if (jsonTagsNew != null) {
                        if (idea.Tags != null && idea.Tags.Count > 0) {
                            jsonTagsNew = jsonTagsNew.Where(r => !idea.Tags.Any(a => r.Value == a.Value)).ToList();
                            idea.Tags.AddRange(jsonTagsNew);
                        } else {
                            idea.Tags = jsonTagsNew;
                        }
                    }

                    if (AddThis != null) {
                        AddThis = UnsavedImage.GetList(AddThis.Select(r => r.FileName).ToList(), context , owner) ;
                        if (idea.Images != null) {
                            foreach (Image image in AddThis) {
                                idea.Images.Add(image);
                            }
                        } else {
                            idea.Images = AddThis;
                        }
                        UnsavedImage.RemoveList(AddThis.Select(r => r.FileName).ToList() , context, owner);
                    }
                    await context.SaveChangesAsync();
                } catch (Exception ex) {
                    ex = ex;
                }
            }
        }
             

        private static async Task<List<Image>> cleanUnSavedImage(List<Image> jsonNew, List<Image>jsonRemove , ApplicationUser owner ,IdeasContext context, IHostingEnvironment hostingEnvironment) {
            if (jsonNew != null && jsonRemove != null) {
                var notsaved = jsonNew.Where(r => jsonRemove.Any(x => x.FileName == r.FileName)).ToList();
                var unsaved = context.UnsavedImages.Where(r => notsaved.Any(x => x.FileName == r.FileName) && r.Owner == owner).ToList();
                foreach (UnsavedImage image in unsaved) {
                    await Image.DeleteFromServer(image.FileName, context, hostingEnvironment);
                }
                UnsavedImage.RemoveList(unsaved.Select(r => r.FileName).ToList(), context, owner);
                jsonNew.RemoveAll(r => unsaved.Any(x => r.FileName == x.FileName));
                await context.SaveChangesAsync();
            }
            return jsonNew;
        }

        private static async void finishRemoveList(int id , List<Image>jsonRemove, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            if (jsonRemove != null) {
                var idea = context.Ideas.Include(r => r.Images).Where(r => r.Id == id).FirstOrDefault();
                var removelist = idea.Images.Where(r => jsonRemove.Any(x => x.FileName == r.FileName)).ToList();
                foreach (Image image in removelist) {
                    var delete = await Image.DeleteFromDb(image.FileName, context);
                    if (delete) {
                        await Image.DeleteFromServer(image.FileName, context, hostingEnvironment);
                    }
                }
            }
        }

        public static IdeaEx IdeaById(int ideaId, ApplicationUser owner, IdeasContext context) {
            var idea = context.Ideas
                     .Where(r => r.Id == ideaId)
                      .Select(pr => new Idea {
                          Id = pr.Id,
                          Owner = new AppUser { Id = pr.Owner.Id, UserName = pr.Owner.UserName, Image = context.Images.FirstOrDefault(r => r.Id == pr.Owner.ImageId), Joined = pr.Owner.Created, Description = pr.Owner.Description, UserStats = context.UserStats.FirstOrDefault(r => r.Id == pr.Owner.UserStatsId) },
                          Titel = pr.Titel,
                          Text = pr.Text,
                          Stats = pr.Stats,
                          DateTime = pr.DateTime,
                          LatestUpdate = pr.LatestUpdate,
                          Favorites = pr.Favorites.Select(r => new Favorite { Owner = r.Owner }).ToList(),
                          Updates = pr.Updates.OrderBy(r => r.DateTime).Select(r => new Update { Id = r.Id, Text = r.Text, DateTime = r.DateTime, Images = r.Images }).ToList(),
                          Comments = pr.Comments.OrderBy(r => r.DateTime)
                            .Select(r =>
                                new Comment {
                                    Owner = new AppUser { UserName = r.Owner.UserName, Image = context.Images.FirstOrDefault(x => x.Id == r.Owner.ImageId), Joined = r.Owner.Created, Description = r.Owner.Description, UserStats = context.UserStats.FirstOrDefault(x => x.Id == r.Owner.UserStatsId) },
                                    Text = r.Text,
                                    DateTime = r.DateTime,
                                    IsOwner = ((r.Owner.Id == owner.Id) ? true : false)
                                })
                                .ToList(),
                          Images = pr.Images,
                          Tags = pr.Tags
                      }
                 ).FirstOrDefault();
            var ideaEx = new IdeaEx {
                Idea = idea,
                AddedToFavorites = idea.Favorites.Any(a => a.Owner == owner),
                Authorized = ((owner == null) ? false : true),
                IsOwner = ((owner.Id == idea.Owner.Id) ? true : false)
            };
            return ideaEx;
        }

        public static Idea ById(int ideaId, ApplicationUser owner, IdeasContext context) {
           var idea = context.Ideas.Include(r => r.Owner).FirstOrDefault(r => r.Id == ideaId);
           return idea;
        }

        public static List<IdeaEx> IdeaList(ApplicationUser owner, IdeasContext context, int skip , int take = 5) {
            var ideaEx = default(List<IdeaEx>);
            try {
                //context.ApplicationUsers.Where(s => pr.Owner.Id == s.Id).Select(r => new AppUser { Id = r.Id, UserName = r.UserName, Image = r.Image, Joined = r.Created, Description = r.Description, UserStats = r.UserStats }).FirstOrDefault(),

                var ideas = context.Ideas
                      .Select(pr => new Idea {
                          Id = pr.Id,
                          Owner = new AppUser { Id = pr.Owner.Id, UserName = pr.Owner.UserName, Image = context.Images.FirstOrDefault(r => r.Id == pr.Owner.ImageId) },
                          Titel = pr.Titel,
                          Text = pr.Text,
                          Stats = pr.Stats,
                          DateTime = pr.DateTime,
                          LatestUpdate = pr.LatestUpdate,
                          Favorites = pr.Favorites.Select(r => new Favorite { Owner = r.Owner }).ToList(),
                          Updates = pr.Updates.OrderBy(r => r.DateTime).Select(r => new Update { Id = r.Id, Text = r.Text, DateTime = r.DateTime, Images = r.Images }).ToList(),
                          Comments = pr.Comments.OrderByDescending(r => r.DateTime)
                            .Select(r =>
                                new Comment {
                                    Id = r.Id,
                                    Owner = new AppUser { UserName = r.Owner.UserName, Image = context.Images.FirstOrDefault(x => x.Id == r.Owner.ImageId) },
                                    Text = r.Text,
                                    DateTime = r.DateTime,
                                    IsOwner = ((((owner != null)) && (owner.Id == r.Owner.Id)) ? true : false)
                                })
                                .ToList(),
                          Images = pr.Images,
                          Tags = pr.Tags
                      }
                  )
                  .OrderByDescending(r => ((r.LatestUpdate >= r.DateTime) ? r.LatestUpdate : r.DateTime))
                  .Skip(skip)
                  .Take(take)
                  .ToList();

            ideaEx = ideas.Select(r =>
                   new IdeaEx {
                       Idea = r,
                       AddedToFavorites = r.Favorites.Any(a => a.Owner == owner),
                       Authorized = ((owner == null) ? false : true),
                       IsOwner = ((((owner != null))&&(owner.Id == r.Owner.Id)) ? true : false)
                   }).ToList();
            } catch (Exception ex) {
                ex = ex;
            }
            return ideaEx;
        }


        public static List<IdeaEx> IdeaListFeed(ApplicationUser owner, IdeasContext context, int skip, int take = 10) {
            var following = context.Followers.Where(r => r.Owner == owner).ToList();
            var ideas = context.Ideas
                .Where(q => q.Owner == owner ||
                    q.Favorites.Any(r => r.Owner == owner) ||
                    ((q.Comments.Any(z => z.Owner == owner)) ? RestoreTimeLineForComment(q , owner) : false ) ||
                    ((following.Any(r => r.Track == q.Owner) ? RestoreTimeLineForFollowing(q) : false )))
                    .OrderByDescending(r => 
                                ((r.Comments.Any(x => x.Owner.Id == owner.Id)) ? r.DateTime :
                                ((following.Any(x => x.Owner == owner)) ? r.DateTime : ((r.LatestUpdate >= r.DateTime) ? r.LatestUpdate : r.DateTime)))
                                )
                     .Select(pr => new Idea {
                         Id = pr.Id,
                         Owner = new AppUser { Id = pr.Owner.Id, UserName = pr.Owner.UserName, Image = context.Images.FirstOrDefault(r => r.Id == pr.Owner.ImageId), Joined = pr.Owner.Created, Description = pr.Owner.Description, UserStats = context.UserStats.FirstOrDefault(r => r.Id == pr.Owner.UserStatsId) },
                         Titel = pr.Titel,
                         Text = pr.Text,
                         Stats = pr.Stats,
                         DateTime = pr.DateTime,
                         LatestUpdate = pr.LatestUpdate,
                         Favorites = pr.Favorites.Select(r => new Favorite { Owner = r.Owner , DateTime = r.DateTime }).ToList(),
                         Updates = pr.Updates.OrderBy(r => r.DateTime).Select(r => new Update { Id = r.Id, Text = r.Text, DateTime = r.DateTime, Images = r.Images }).ToList(),
                         Comments = pr.Comments.OrderByDescending(r => r.DateTime)
                            .Select(r =>
                                new Comment {
                                    Id = r.Id,
                                    Owner = new AppUser {Id = r.Owner.Id, UserName = r.Owner.UserName, Image = context.Images.FirstOrDefault(x => x.Id == r.Owner.ImageId), Joined = r.Owner.Created, Description = r.Owner.Description, UserStats = context.UserStats.FirstOrDefault(x => x.Id == r.Owner.UserStatsId) },
                                    Text = r.Text,
                                    DateTime = r.DateTime,
                                    IsOwner = ((r.Owner.Id == owner.Id)? true : false)
                                })
                                .ToList(),
                         Images = pr.Images,
                         Tags = pr.Tags
                     })
                .Skip(skip)
                .Take(take)
                .ToList();

            var ideaEx = ideas.Select(r =>
                   new IdeaEx {
                       Idea = r,
                       AddedToFavorites = r.Favorites.Any(a => a.Owner == owner),
                       Authorized = ((owner == null) ? false : true),
                       IsOwner = ((owner.Id == r.Owner.Id) ? true : false),
                       Reason = ((owner.Id == r.Owner.Id) ? onFeedReason.Owner :
                                ((r.Favorites.Any(x => x.Owner == owner)) ? checkReason(r, owner, onFeedReason.Favorited) :
                                ((r.Comments.Any(x => x.Owner.Id == owner.Id)) ? onFeedReason.Commented :
                                ((following.Any(x => x.Owner == owner))? onFeedReason.Following : onFeedReason.None))))
                   }).ToList();
            return ideaEx;
        }

        private static bool RestoreTimeLineForComment(Idea idea, ApplicationUser owner) {
            if (idea.Updates != null && idea.Updates.Count > 0) {
                var comment = idea.Comments.FirstOrDefault(z => z.Owner == owner);
                var nearestDiff = idea.Updates.Min(date => Math.Abs((date.DateTime - comment.DateTime).Ticks));
                var nearest = idea.Updates.Where(date => Math.Abs((date.DateTime - comment.DateTime).Ticks) == nearestDiff).First();
                idea.LatestUpdate = nearest.DateTime;
            }
            return true;
        }

        private static bool RestoreTimeLineForFollowing(Idea idea) {
            idea.LatestUpdate = idea.DateTime;
            return true;
        }

        private static onFeedReason  checkReason(Idea idea , ApplicationUser owner,  onFeedReason reason) {
            onFeedReason response = onFeedReason.Favorited;
            if (idea.Updates.Count >  0) {
                var update = idea.Updates.Last();
                var fav = idea.Favorites.First(r => r.Owner == owner);
                response = ((fav.DateTime < update.DateTime) ? onFeedReason.Updated : onFeedReason.Favorited);
            }
            return response;
        }

        public static List<IdeaEx> IdeasFromUser(ApplicationUser caller, AppUser user, IdeasContext context, int skip, int take = 5) {
            var ideas = context.Ideas
                  .Where(r => r.Owner.Id == user.Id)
                  .Select(pr => new Idea {
                      Id = pr.Id,
                      Owner = new AppUser { Id = pr.Owner.Id, UserName = pr.Owner.UserName, Image = context.Images.FirstOrDefault(r => r.Id == pr.Owner.ImageId), Joined = pr.Owner.Created, Description = pr.Owner.Description, UserStats = context.UserStats.FirstOrDefault(r => r.Id == pr.Owner.UserStatsId) },
                      Titel = pr.Titel,
                      Text = pr.Text,
                      Stats = pr.Stats,
                      DateTime = pr.DateTime,
                      LatestUpdate = pr.LatestUpdate,
                      Favorites = pr.Favorites.Select(r => new Favorite { Owner = r.Owner }).ToList(),
                      Updates = pr.Updates.OrderBy(r => r.DateTime).Select(r => new Update { Text = r.Text, DateTime = r.DateTime, Images = r.Images}).ToList(),
                      Comments = pr.Comments.OrderByDescending(r => r.DateTime)
                            .Select(r =>
                                new Comment {
                                    Owner = new AppUser { UserName = r.Owner.UserName, Image = context.Images.FirstOrDefault(x => x.Id == r.Owner.ImageId), Joined = r.Owner.Created, Description = r.Owner.Description, UserStats = context.UserStats.FirstOrDefault(x => x.Id == r.Owner.UserStatsId) },
                                    Text = r.Text,
                                    DateTime = r.DateTime,
                                    IsOwner = ((r.Owner.Id == user.Id) ? true : false)
                                })
                                .ToList(),
                      Images = pr.Images
                  }
              )
              .OrderByDescending(r => ((r.LatestUpdate >= r.DateTime) ? r.LatestUpdate : r.DateTime))
              .Skip(skip)
              .Take(take)
              .ToList();
            var ideaEx = ideas.Select(r =>
                   new IdeaEx {
                       Idea = r,
                       AddedToFavorites = r.Favorites.Any(a => a.Owner == caller),
                       Authorized = ((caller == null) ? false : true),
                       IsOwner = ((caller.Id == r.Owner.Id) ? true : false)
                   }).ToList();
            return ideaEx;
        }

        public static List<IdeaEx> GetFullList(int offset, AppUser user, IdeasContext context) {
            var ideas = context.Ideas
            .Where(r => r.Owner.Id == user.Id)
            .Select(pr => new Idea {
                Id = pr.Id,
                Titel = pr.Titel,
                Stats = pr.Stats
            }
        )
        .OrderByDescending(r => ((r.LatestUpdate >= r.DateTime) ? r.LatestUpdate : r.DateTime))
        .Skip(offset)
        .ToList();
            var ideaEx = ideas.Select(r =>
                      new IdeaEx {
                          Idea = r
                      }).ToList();
            return ideaEx;
        }
    }
        
}
