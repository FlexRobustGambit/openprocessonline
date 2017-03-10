using ideas.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.IdeasViewModel
{
    public class ListViewModel {
        //collect
        public List<Idea> Ideas { get; set; }
        public List<IdeaEx> IdeasEx { get; set; }

        //return 
        public int IdeaId { get; set; }
        public String Comment { get; set; }
        public int EditId { get; set; }

        //lazy
        public int Start { get; set; }
        public DateTime StartPoint { get; set; }
//      public string UserName { get; set; }
        public AppUser AppUser { get; set; }

        //public Idea Idea { get; set; }
        public IdeaEx IdeaEx { get; set; }
        public Update Update { get; set; }

        public int CommentId { get; set; }

        //public string Text { get; set; }

        //public string JsonImagesRemove { get; set; }
        //public string JsonImagesNew { get; set; }

        /*        public string JsonTags { get; set; }
                public string JsonTagsRemove { get; set; }

                public string JsonImagesRemove { get; set; }
                public string JsonImagesNew { get; set; }

                public string Titel { get; set; } */


        public EditIdeaViewModel EditIdeaViewModel { get; set; }
        public EditUpdateViewModel EditUpdateViewModel { get; set; }
        public SearchViewModel SearchViewModel { get; set; }
    }


}
