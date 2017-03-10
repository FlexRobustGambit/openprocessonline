



using System.ComponentModel.DataAnnotations;

namespace ideas.Models.IdeasViewModel {
    public class EditIdeaViewModel {
        [Required]
        public string Titel { get; set; }
        [Required]
        public string Text { get; set; }

        [Required]
        public string JsonTags { get; set; }
        public string JsonTagsRemove { get; set; }
        public string JsonTagsNew { get; set; }

        public string JsonImagesNew { get; set; }
        public string JsonImagesRemove { get; set; }

    }
}
