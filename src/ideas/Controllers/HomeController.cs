using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ideas.Models.Database;
using Microsoft.AspNetCore.Identity;
using ideas.Models;
using ideas.Models.IdeasViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;
using ideas.Models.Enum;

namespace ideas.Controllers
{
    public class HomeController : Controller {

        private readonly IdeasContext context;
        private UserManager<ApplicationUser> userManager;
        private readonly IHostingEnvironment hostingEnvironment;
        private string[] acceptedTypes = { "image/jpeg", "image/png", "image/jpg" };

        public HomeController(UserManager<ApplicationUser> userManager, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            this.userManager = userManager;
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index() {
            return View();
        }

        [Authorize]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([FromBody] ListViewModel vm) {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            var ideas = await IdeaEx.Search(vm.SearchViewModel.SearchWord, owner, context);
            var JsonData = JsonConvert.SerializeObject(ideas, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonData);
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notifications() {
           var owner = await userManager.GetUserAsync(HttpContext.User);
           var notifications =  await Notification.Get(owner, context);      
           var JsonData = JsonConvert.SerializeObject(notifications, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
           return Json(JsonData);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewedNotifications() {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            Notification.SetViewed(owner, context);
            await context.SaveChangesAsync();
            return Json("");
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPrucess([FromBody]NewIdeaViewModel vm) {
            if (ModelState.IsValid) {
                var owner = await userManager.GetUserAsync(HttpContext.User);
                var idea = await IdeaEx.NewIdea(vm, owner, context, hostingEnvironment);
                var followers = Follow.Get(owner, context).Result;
                foreach (Follow track in followers) {
                    Notification.Add(NotificationType.NewPost, track.Owner, owner, idea, context);
                }
                await context.SaveChangesAsync();
                var titel = Regex.Replace(idea.Titel.ToLower(), @"\s+", "_");
                var json = JsonConvert.SerializeObject(new { Id = idea.Id, Name = titel }, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                return Json(json);
            }
            var JsonData = JsonConvert.SerializeObject(ModelState.Values, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonData);
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromBody]ListViewModel vm) {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            try {
                if (vm.EditId == 0 && vm.IdeaId != null) {
                    var amountFavorites = await IdeaEx.Remove(owner, vm, context, hostingEnvironment);
                    UserStats.RemoveProject(owner, context);
                    UserStats.RemoveFavorited(owner, context, amountFavorites);
                } else {
                    await Models.Database.Update.Remove(owner, vm, context, hostingEnvironment);
                    vm.IdeaEx = IdeaEx.IdeaByIdAndOwner(vm.IdeaId, owner, context);
                }
                await context.SaveChangesAsync();
            } catch (Exception ex) {
                ex = ex;
            }
            var JsonData = JsonConvert.SerializeObject(vm, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonData);
         }

        [Authorize]
        public async Task<IActionResult> Details(int id) {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            var idea = IdeaEx.IdeaById(id, owner, context);
            Stats.AddViews(idea.Idea);
            var json = new DetailsViewModel {
                    JsonData = JsonConvert.SerializeObject(idea, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            };
            return View(json);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment([FromBody]ListViewModel vm) {
            if (ModelState.IsValid) {
                try {
                    var owner = await userManager.GetUserAsync(HttpContext.User);
                    var idea = context.Ideas.Include(r => r.Stats).Include(r => r.Comments).Include(r => r.Owner).FirstOrDefault(r => r.Id == vm.IdeaId);
                    if (idea.Owner != owner) {
                        Notification.Add(NotificationType.Commented, idea.Owner, owner, idea, context);
                    }
                    Comment.Add(owner, idea, vm.Comment);
                    UserStats.AddComment(owner, context);
                    Stats.AddComments(idea);
                    await context.SaveChangesAsync();
                    var ideaEx = IdeaEx.IdeaById(vm.IdeaId, owner, context);
                    var JsonData = JsonConvert.SerializeObject(ideaEx, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    return Json(JsonData);
                } catch (Exception ex){
                    ex = ex;
                }
            } else {
                var JsonDataError = JsonConvert.SerializeObject(ModelState.Values, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                return Json(JsonDataError);
            } 
            return Json("");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveComment([FromBody]ListViewModel vm) {
            if (ModelState.IsValid) {
                try {
                    var owner = await userManager.GetUserAsync(HttpContext.User);
                    var idea = context.Ideas.Include(r=> r.Stats).Where(r => r.Comments.Any(x => x.Id == vm.CommentId && x.Owner.Id == owner.Id) && r.Id == vm.IdeaId).FirstOrDefault();
                    if (idea != null && Comment.Remove(vm.CommentId, owner, context)) {
                        UserStats.RemoveComment(owner, context);
                        Stats.RemoveComments(idea);
                        await context.SaveChangesAsync();
                        var ideaEx = IdeaEx.IdeaById(vm.IdeaId, owner, context);
                        var JsonData = JsonConvert.SerializeObject(ideaEx, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                        return Json(JsonData);
                    }
                } catch (Exception ex) {
                    ex = ex;
                }
            } else {
                var JsonDataError = JsonConvert.SerializeObject(ModelState.Values, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                return Json(JsonDataError);
            }
            return Json("");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ById([FromBody] ListViewModel vm) {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            var idea = IdeaEx.IdeaById(vm.IdeaId, owner, context);
            var JsonData = JsonConvert.SerializeObject(idea, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonData);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFiles(IEnumerable<IFormFile> files) {
            if (ModelState.IsValid) {
                var owner = await userManager.GetUserAsync(HttpContext.User);
                var path = Path.Combine(hostingEnvironment.WebRootPath, "images/uploads");
                var imageslist = await Image.Upload(owner, path, files, context);
                UnsavedImage.SaveList(imageslist, owner, context);
                var refs = ImageRef.ImageListToRef(imageslist);
                var JsonData = JsonConvert.SerializeObject(refs, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                return Json(JsonData);
            }
            return Json("");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public void Reset([FromBody]NewIdeaViewModel model) {
            try {
                if (ModelState.IsValid) {
                    string fullPath = Path.Combine(hostingEnvironment.WebRootPath, "images/uploads/");
                    foreach (var image in model.Images) {
                        var file = string.Format("{0}{1}", fullPath, image.FileName);
                        if (System.IO.File.Exists(file)) {
                            System.IO.File.Delete(file);
                        }
                    }
                }
            } catch (Exception e) {
                var ee = e;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LazyRequest([FromBody]ListViewModel vm) {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            var idealist = ((vm.AppUser == null) ? IdeaEx.IdeaList(owner, context, vm.Start) : IdeaEx.IdeasFromUser(owner, vm.AppUser, context, vm.Start));
            return Json(idealist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LazyRequestFeed([FromBody]ListViewModel vm) {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            var idealist = IdeaEx.IdeaListFeed(owner, context, vm.Start);
            return Json(idealist);
        }

        [Authorize]
        public async Task<IActionResult> Update() {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            var appuser = AppUser.ApplicationUserToAppUser(owner);
            var idealist = IdeaEx.GetFullList(0, appuser, context);
            var vm = new UpdateViewModel {
                JsonIdeas = JsonConvert.SerializeObject(idealist, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            };
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> UpdateProcess(int id) {
            if (id == null) {
                return RedirectToAction("Home", "Update");
            }
            var owner = await userManager.GetUserAsync(HttpContext.User);
            var appuser = AppUser.ApplicationUserToAppUser(owner);
            var idea = IdeaEx.IdeaById(id, owner, context);
            if (idea.Authorized && idea.Idea.Owner.Id == owner.Id) {
                var vm = new UpdateViewModel {
                    JsonData = JsonConvert.SerializeObject(idea, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
                };
                return View(vm);
            } else {
                return RedirectToAction("Account", "Login");
            }
        }


        //deze mag weg he 
        [HttpPost]
        public async Task<IActionResult> UpdateProcess(UpdateViewModel vm) {
            if (ModelState.IsValid) {
                var idea = context.Ideas.Include(r => r.Stats).FirstOrDefault(r => r.Id == vm.Process);
                try {
                    var update = new Update {
                        Text = vm.Text,
                        DateTime = DateTime.Now,
                        Images = ((vm.JsonImages != null) ? JsonConvert.DeserializeObject<List<Image>>(vm.JsonImages) : null)
                    };
                    idea.Updates.Add(update);
                    idea.LatestUpdate = DateTime.Now;
                    Stats.AddUpdate(idea);
                    await context.SaveChangesAsync();
                } catch (Exception ex) {
                    var exs = ex;
                }
                return RedirectToAction("Details", "Home", new { id = idea.Id });
            } else {
                return RedirectToAction("UpdateProcess", "Home", new { id = vm.Process });
            }
        }
      
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Favorite([FromBody]ListViewModel vm) {
            try {
                var owner = await userManager.GetUserAsync(HttpContext.User);
                var idea = IdeaEx.ById(vm.IdeaId, owner, context);
                UserStats.AddFavorite(owner, context);
                UserStats.AddFavorited(idea.Owner, context);
                if (idea.Owner != owner) {
                    Notification.Add(NotificationType.Favorited, idea.Owner, owner, idea, context);
                }
                var favorite = Models.Database.Favorite.Add(idea, owner, context);
                await context.SaveChangesAsync();
                return Json(favorite);
            } catch (Exception ex) {
                ex = ex;
            }
            return Json("");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]ListViewModel vm) {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            vm.IdeaEx = IdeaEx.IdeaByIdAndOwner(vm.IdeaId, owner, context);
            if (vm.IdeaEx != null) {
                if (vm.EditId != 0) {
                    vm.Update = await ideas.Models.Database.Update.GetByUpdateFromIdea(vm.IdeaEx.Idea.Id , vm.EditId , owner , context);
                }
            }
            var JsonData = JsonConvert.SerializeObject(vm, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonData);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveEdit([FromBody]ListViewModel vm) {
            if (ModelState.IsValid) {
                var owner = await userManager.GetUserAsync(HttpContext.User);
                //var JsonData = "";
                if (vm.EditId == 0) {
                    try {
                        await IdeaEx.UpdateIdea(vm, owner, context, hostingEnvironment);
                        vm.IdeaEx = IdeaEx.IdeaByIdAndOwner(vm.IdeaId, owner, context);
                    } catch (Exception ex) {
                        ex = ex;
                    }
                } else {
                    await Models.Database.Update.UpdateUpdate(vm, owner, context, hostingEnvironment);
                    vm.IdeaEx = IdeaEx.IdeaByIdAndOwner(vm.IdeaId, owner, context);
                }
                var JsonData = JsonConvert.SerializeObject(vm, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                return Json(JsonData);
            }
            var JsonDataError = JsonConvert.SerializeObject(ModelState.Values, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonDataError);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostUpdate([FromBody]ListViewModel vm) {
            if (ModelState.IsValid) {
                var owner = await userManager.GetUserAsync(HttpContext.User);
                try {
                    await Models.Database.Update.PostUpdate(vm, owner, context, hostingEnvironment);
                    vm.IdeaEx = IdeaEx.IdeaByIdAndOwner(vm.IdeaId, owner, context);
                    foreach (Favorite fav in vm.IdeaEx.Idea.Favorites) {
                        Notification.Add(NotificationType.Update, fav.Owner , owner, vm.IdeaEx.Idea, context);
                    }
                    var JsonData = JsonConvert.SerializeObject(vm, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    return Json(JsonData);
                } catch (Exception ex) {
                    ex = ex;
                }
            }
            var JsonDataError = JsonConvert.SerializeObject(ModelState.Values, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonDataError);
        }
            
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFavorite([FromBody]ListViewModel vm) {
            var idea = context.Ideas.Include(r => r.Stats).Include(r => r.Owner).FirstOrDefault(r => r.Id == vm.IdeaId);
            var favorite = default(Favorite);
            try {
                var owner = await userManager.GetUserAsync(HttpContext.User);
                UserStats.RemoveFavorite(owner, context);
                UserStats.RemoveFavorited(idea.Owner, context);
                favorite = context.Favorites.Include(r=> r.Idea).FirstOrDefault(r => (r.Owner == owner  && r.Idea.Id == vm.IdeaId ));
                if (idea.Stats != null) {
                    if (idea.Stats.Favorites > 0) {
                        idea.Stats.Favorites = idea.Stats.Favorites - 1;
                    }
                }
                context.Favorites.Remove(favorite);
                await context.SaveChangesAsync();
            } catch (Exception ex) {
                var exs = ex;
            }
            return Json(favorite);
        }

        [HttpPost]
        public async Task<IActionResult> LoadComments([FromBody]ListViewModel vm) {
            var comments = context.Ideas
                .Where(r => r.Id == vm.IdeaId)
                .Select(x => x.Comments.Select(z =>
                          new Comment {
                              Id = z.Id,
                              Owner = z.Owner,
                              Text = z.Text,
                              DateTime = z.DateTime,
                              RepliedTo = z.RepliedTo
                          }).OrderByDescending(r => r.DateTime).ToList()
            );
            var JsonData = JsonConvert.SerializeObject(comments, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(JsonData);
        }

        public async Task<IActionResult> Feed() {
            var owner = await userManager.GetUserAsync(HttpContext.User);
            if (owner == null) {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }



        public IActionResult Error()
        {
          return View();
        }

    }
}
