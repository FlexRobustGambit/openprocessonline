using ideas.Models.IdeasViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ideas.Models.Database {
    public class Update {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public List<Image> Images { get; set; }

        public static async Task Remove(ApplicationUser owner, ListViewModel vm , IdeasContext context, IHostingEnvironment hostingEnviroment) {
            try {
                var update = await context.Ideas
                       .Where(r => r.Id == vm.IdeaId && r.Owner == owner)
                       .Select(o => o.Updates
                           .Where(g => g.Id == vm.EditId).Select(x => new Update {
                               Id = x.Id,
                               Text = x.Text,
                               DateTime = x.DateTime,
                               Images = x.Images.Select(z => new Image { Id = z.Id, Name = z.Name, Height = z.Height, Width = z.Width, FileName = z.FileName, OriginName = z.OriginName }).ToList()
                           }).FirstOrDefault()).FirstOrDefaultAsync();

                if (update != null) {
                    foreach (Image image in update.Images) {
                        await Image.DeleteFromServer(image.FileName, context, hostingEnviroment);
                    }
                    context.Images.RemoveRange(update.Images);
                    context.Updates.Remove(update);
                    await context.SaveChangesAsync();
                }
            } catch (Exception ex) {
                ex = ex;
            }
        }

        public static async Task<Update> GetByUpdateFromIdea(int ideaId, int editId, ApplicationUser owner, IdeasContext context) {
            var update =  await context.Ideas
                .Where(r => r.Id == ideaId && r.Owner == owner)
                .Select(o => o.Updates
                    .Where(g => g.Id == editId).Select(x => new Update {
                        Id = x.Id,
                        Text = x.Text,
                        DateTime = x.DateTime,
                        Images = x.Images.Select(z => new Image { Id = z.Id, Name = z.Name, Height = z.Height, Width = z.Width, FileName = z.FileName, OriginName = z.OriginName }).ToList()
                    }).FirstOrDefault()).FirstOrDefaultAsync();       
            
            /*var update = await context.Updates.Where(r => r.Id == editId && r.Idea.Id == ideaId && r.Idea.Owner == owner)
                   .Select(r => new Update {
                       Id = r.Id,
                       Text = r.Text,
                       DateTime = r.DateTime,
                       Images = r.Images.Select(x => new Image { Id = x.Id, Name = x.Name, Height = x.Height, Width = x.Width, FileName = x.FileName, OriginName = x.OriginName }).ToList()
                   }).FirstOrDefaultAsync(); */
            return update;
        }

        public static async Task PostUpdate(ListViewModel vm, ApplicationUser owner, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            var idea = await context.Ideas.Include(r=> r.Updates).FirstOrDefaultAsync(r=> r.Id == vm.IdeaId && r.Owner == owner);
            if (idea != null) {
                var AddThis = Image.GetNewAndRemoveOld(vm.EditUpdateViewModel.JsonImagesNew, vm.EditUpdateViewModel.JsonImagesRemove, vm.IdeaId, vm.EditId , owner, hostingEnvironment , context ).Result;
                try {
                    var update = new Update();
                    update.Text = vm.EditUpdateViewModel.Text.Trim();
                    update.DateTime = DateTime.Now;
                    if (AddThis != null) {
                        AddThis = UnsavedImage.GetList(AddThis.Select(r => r.FileName).ToList(), context, owner);
                        update.Images = AddThis;
                        UnsavedImage.RemoveList(AddThis.Select(r => r.FileName).ToList(), context, owner);
                    }
                    idea.Updates.Add(update);
                    idea.LatestUpdate = DateTime.Now;
                    Stats.AddUpdate(idea);
                    await context.SaveChangesAsync();
                } catch (Exception ex) {
                    ex = ex;
                }
            } 
        }

        public static async Task UpdateUpdate(ListViewModel vm, ApplicationUser owner, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            var update = GetByUpdateFromIdea(vm.IdeaId, vm.EditId, owner, context).Result;
            if (update != null) {
                var jsonRemove = ((vm.EditUpdateViewModel.JsonImagesRemove != null) ? JsonConvert.DeserializeObject<List<Image>>(vm.EditUpdateViewModel.JsonImagesRemove) : null);
                var jsonNew = ((vm.EditUpdateViewModel.JsonImagesNew != null) ? JsonConvert.DeserializeObject<List<Image>>(vm.EditUpdateViewModel.JsonImagesNew) : null);
                var AddThis = cleanUnSavedImage(jsonNew, jsonRemove, owner, context, hostingEnvironment).Result;
                finishRemoveList(vm.IdeaId, vm.EditId, jsonRemove, context, hostingEnvironment);
                try {
                    update = context.Updates.First(r => r.Id == vm.EditId);
                    update.Text = vm.EditUpdateViewModel.Text.Trim();
                    if (AddThis != null) {
                        AddThis = UnsavedImage.GetList(AddThis.Select(r => r.FileName).ToList(), context, owner);
                        if (update.Images != null) {
                            foreach (Image image in AddThis) {
                                update.Images.Add(image);
                            }
                        } else {
                            update.Images = AddThis;
                        }
                        UnsavedImage.RemoveList(AddThis.Select(r => r.FileName).ToList(), context, owner);
                    }
                    await context.SaveChangesAsync();
                } catch (Exception ex) {
                    ex = ex;
                }
            }
        }
        
        private static async Task<List<Image>> cleanUnSavedImage(List<Image> jsonNew, List<Image> jsonRemove, ApplicationUser owner, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            if (jsonNew != null && jsonRemove != null) {
                var notsaved = jsonNew.Where(r => jsonRemove.Any(x => x.FileName == r.FileName)).ToList();
                var unsaved = context.UnsavedImages.Where(r => notsaved.Any(x => x.FileName == r.FileName) && r.Owner == owner).ToList();
                foreach (UnsavedImage image in unsaved) {
                    await Image.DeleteFromServer(image.FileName, context, hostingEnvironment);
                }
                UnsavedImage.RemoveList(unsaved.Select(r => r.FileName).ToList(), context, owner);
                jsonNew.RemoveAll(r => unsaved.Any(x => r.FileName == x.FileName));
                await context.SaveChangesAsync();
            }
            return jsonNew;
        }

        private static async void finishRemoveList(int id, int updateId , List<Image> jsonRemove, IdeasContext context, IHostingEnvironment hostingEnvironment) {
            if (jsonRemove != null) {
                var idea = context.Ideas.Include(r=> r.Updates).Where(r => r.Id == id).FirstOrDefault();
                var update = context.Updates.Include(r => r.Images).Where(r => r.Id == updateId).FirstOrDefault();
                var removelist = update.Images.Where(r => jsonRemove.Any(x => x.FileName == r.FileName)).ToList();
                foreach (Image image in removelist) {
                    var delete = await Image.DeleteFromDb(image.FileName, context);
                    if (delete) {
                        await Image.DeleteFromServer(image.FileName, context, hostingEnvironment);
                    }
                }
            }
        }
    }
}
