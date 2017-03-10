using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ideas.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace ideas.Models.IdeasViewModel
{
    public class UpdateViewModel {
        public List<IdeaEx> Ideas { get; set; }
        public IdeaEx Idea { get; set; }
        public int Process { get; set; }
        public string JsonIdeas { get; set; }
        public string JsonData { get; set; }
        public string JsonImages { get; set; }
                   

        [Required]
        public string Text { get; set; }
    }
}
