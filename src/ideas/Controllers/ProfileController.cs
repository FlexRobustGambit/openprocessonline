using ideas.Models;
using ideas.Models.Database;
using ideas.Models.Enum;
using ideas.Models.ProfileViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Controllers
{
    public class ProfileController : Controller {
        private readonly IdeasContext context;
        private UserManager<ApplicationUser> userManager;
        private readonly IHostingEnvironment hostingEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            this.userManager = userManager;
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Index() {
          var currentuser = await  userManager.GetUserAsync(HttpContext.User);
          return RedirectToAction("User", "Profile", new { user = currentuser.UserName});
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> User(string user) {
            var caller = await userManager.GetUserAsync(HttpContext.User);
            Follow follow;
            AppUser currentuser;
            if (user == null) {
                follow = null;
               currentuser = AppUser.GetByUserName(caller.UserName, context);
            }else {
                currentuser = AppUser.GetByUserName(user, context);
                follow = context.Followers.FirstOrDefault(r => r.Owner == caller && r.Track.Id == currentuser.Id);
            }
            var ideas = IdeaEx.IdeasFromUser(caller, currentuser, context, 0);
            var profileVM = new ProfileViewModel {
                User = currentuser,
                Following = ((follow == null) ? false : true),
                JsonIdeas = JsonConvert.SerializeObject(ideas, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
            };
            return View(profileVM);
        }

        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessList([FromBody]ProfileViewModel vm) {
            var username = "";
            try {
                username = vm.UserName;
            } catch (NullReferenceException ex) {
                var owner = await userManager.GetUserAsync(HttpContext.User);
                username = await userManager.GetUserNameAsync(owner);
            }
                     
            var user =  AppUser.GetByUserName(username, context);
            var ideas = IdeaEx.GetFullList(vm.Offset, user, context);
            var json = JsonConvert.SerializeObject(ideas, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return Json(json);
        }

        public async Task<bool> Follow([FromBody] ProfileViewModel vm) {
            try { 
                var caller = await userManager.GetUserAsync(HttpContext.User);
                var trackUser = await userManager.FindByNameAsync(vm.UserName);
                if (trackUser != null) {
                    var follow = new Follow {
                        DateTime = DateTime.Now,
                        Owner = caller,
                        Track = trackUser
                    };
                    context.Followers.Add(follow);
                    UserStats.AddFollower(trackUser, context);
                    UserStats.AddFollowing(caller, context);
                    if (trackUser != caller) {
                        Notification.Add(NotificationType.NewFollower, trackUser, caller, null, context);
                    }
                    await context.SaveChangesAsync();
                    return true;
                }
            } catch (Exception ex) {
                ex = ex;
            }
            return false; 
        }

        public async Task<bool> UnFollow([FromBody] ProfileViewModel vm) {
            var caller = await userManager.GetUserAsync(HttpContext.User);
            var trackUser = await userManager.FindByNameAsync(vm.UserName);
            if (trackUser != null) {
                var follow = context.Followers.FirstOrDefault(r=>r.Owner == caller && r.Track == trackUser); 
                context.Followers.Remove(follow);
                UserStats.RemoveFollower(trackUser, context);
                UserStats.RemoveFollowing(caller, context);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
