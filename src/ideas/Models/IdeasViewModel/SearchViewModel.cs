using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.IdeasViewModel
{
    public class SearchViewModel
    {
        [Required]
        public string SearchWord { get; set; }

        public string SearchReponse { get; set; }
    }
}
