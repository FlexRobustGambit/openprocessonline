using ideas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Controllers
{
    public class ProcessController {
        private readonly IdeasContext context;
        private UserManager<ApplicationUser> userManager;
        private readonly IHostingEnvironment hostingEnvironment;



        public ProcessController(UserManager<ApplicationUser> userManager, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            this.userManager = userManager;
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }
             
    }
}
