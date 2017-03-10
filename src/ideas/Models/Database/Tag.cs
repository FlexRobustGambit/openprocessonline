using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database
{
    public class Tag {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime DateTime { get; set; }
        public int TimesUsed { get; set; }

       
             
        public static async void Post(Tag tag, IdeasContext context) {
            try {
                tag.DateTime = DateTime.Now;
                context.Tags.Add(tag);
                await context.SaveChangesAsync();
            } catch (Exception ex) {
                ex = ex;
            }
        }

        public static async void PostList(List<Tag> tags,  IdeasContext context) {
            try {
               // context.Tags.Where(t => tags.Any(x => x.Value == t.Value) && idea.Id == t.idea )).ToList();
                foreach (Tag tag in tags) {
                    tag.DateTime = DateTime.Now;
                    context.Tags.Add(tag);
                }
                await context.SaveChangesAsync();
            } catch (Exception ex) {
                ex = ex;
            }
        }


        public void Update() {

        }

        public static async void RemoveList(List<Tag> tags, IdeasContext context) {
            context.Tags.RemoveRange(tags);
            await context.SaveChangesAsync();
        }

        public static Tag Get(string val , IdeasContext context) {
            var tag  = context.Tags.FirstOrDefault(r => r.Value == val);
            return tag;
        }
    }
}
