using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace ideas.Models.Database
{
    public class ImageRef {

           public string FileName { get; set; }
          

        public static List<ImageRef> ImageListToRef(List<Image> imagelist) {
            List<ImageRef> refs = new List<ImageRef>();
            foreach (Image image in imagelist) {
                var reff = new ImageRef {
                    FileName = image.FileName
                    };
                refs.Add(reff);
            }
            return refs;
        }
    }
}
