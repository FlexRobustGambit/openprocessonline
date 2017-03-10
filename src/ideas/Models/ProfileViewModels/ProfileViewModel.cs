using ideas.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.ProfileViewModels
{   
    public class ProfileViewModel {

        public AppUser User { get; set; }
        public List<IdeaEx> Ideas { get; set; }
        public List<Stats> Stats { get; set; }

        public string UserName { get; set; }
        public int Offset { get; set; }
        public string JsonIdeas { get; set; }
        public bool Following { get; set; }
    }
}
