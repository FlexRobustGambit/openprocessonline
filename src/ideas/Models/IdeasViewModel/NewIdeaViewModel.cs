using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ideas.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace ideas.Models.IdeasViewModel
{
    public class NewIdeaViewModel {

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        public String Titel { get; set; }

        [Required]
        [StringLength(999999, ErrorMessage = "Minimun of {2} required", MinimumLength = 10)]
        [DataType(DataType.Text)]
        public String Text { get; set; }
        
        [Required(ErrorMessage = "1 tag reguired")]
        [DataType(DataType.Text)]
        public String JsonTags { get; set; }

        public List<Image> Images { get; set; }
        public String JsonImagesNew { get; set; }
        public String JsonImagesRemove { get; set; }

        public String FileName { get; set; }

        
    }
}
