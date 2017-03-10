using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.IdeasViewModel
{
    public class EditUpdateViewModel
    {
      
        [Required]
        public string Text { get; set; }

        public string JsonImagesNew { get; set; }
        public string JsonImagesRemove { get; set; }
    }
}
