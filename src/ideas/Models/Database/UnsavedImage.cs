using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace ideas.Models.Database
{
    public class UnsavedImage
    {
        public int Id { get; set; }
        public string ContentDisposition { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string OriginName { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime DateTime { get; set; }
        public ApplicationUser Owner { get; set; }

        public UnsavedImage() { }

        public UnsavedImage(Image image) {
            this.ContentDisposition = image.ContentDisposition;
            this.ContentType = image.ContentType;
            this.FileName = image.FileName;
            this.OriginName = image.OriginName;
            this.Length = image.Length;
            this.Name = image.Name;
            this.Width = image.Width;
            this.Height = image.Height;
            this.DateTime = image.DateTime;
        }
                
        public async static void RemoveList(List<string> filenames, IdeasContext context, ApplicationUser owner) {
            var list = context.UnsavedImages.Where(r => filenames.Any(x => x == r.FileName) && r.Owner == owner).ToList();
            foreach (UnsavedImage unsaved in list) {
                context.UnsavedImages.Remove(unsaved);
            }
            await context.SaveChangesAsync();
        }

        public async static void Remove(int id, IdeasContext context, ApplicationUser owner) {
            var image = context.UnsavedImages.Where(r => r.Id == id && r.Owner == owner).FirstOrDefault();
            context.UnsavedImages.Remove(image);
            await context.SaveChangesAsync();
        }

        public static List<Image> GetList(List<string> filenames, IdeasContext context, ApplicationUser owner) {
            var unsavedimages = context.UnsavedImages.Where(r => filenames.Any(x => x == r.FileName) && r.Owner == owner)
                .Select(r =>  new Image {
                        Height = r.Height,
                        Width = r.Width,
                        Length = r.Length,
                        OriginName = r.OriginName,
                        FileName = r.FileName,
                        ContentType = r.ContentType,
                        ContentDisposition = r.ContentDisposition,
                        DateTime = r.DateTime,
                        Name = r.Name        
                }).ToList();
            return unsavedimages; 
        }

        public static UnsavedImage Get(int id, IdeasContext context, ApplicationUser owner) {
            var unsavedimages = context.UnsavedImages.Where(r => id == r.Id && r.Owner == owner).FirstOrDefault();
            return unsavedimages;
        }

        public async static void Save(UnsavedImage unsavedImage, IdeasContext context) {
            context.UnsavedImages.Add(unsavedImage);
            await context.SaveChangesAsync();
        }

        public async static void SaveList(List<Image> unsavedImage, ApplicationUser owner , IdeasContext context) {
            foreach(Image image in unsavedImage) {
                var unsaved = new UnsavedImage(image);
                unsaved.Owner = owner;
                context.UnsavedImages.Add(unsaved);
            }
            await context.SaveChangesAsync();
        }
    }
}
