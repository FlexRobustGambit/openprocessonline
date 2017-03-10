using ideas.Models.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace ideas.Models
{
    public class IdeasContext : IdentityDbContext<ApplicationUser> {

        public IdeasContext(DbContextOptions<IdeasContext> options) : base(options) { }

        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ProfileImage> ProfileImages { get; set; }  
        public DbSet<Update> Updates { get; set; }
        public DbSet<UnsavedImage> UnsavedImages { get; set; }
        public DbSet<Follow> Followers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<UserStats> UserStats { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            var entity = modelBuilder.Entity<Comment>();
            entity.Ignore(b => b.IsOwner);
            entity.Property(p => p.Text).HasColumnType("text");
            var entity2 = modelBuilder.Entity<Idea>();
            entity2.Property(p => p.Text).HasColumnType("text");
            var entity3 = modelBuilder.Entity<Update>();
            entity3.Property(p => p.Text).HasColumnType("text");
            var entity4 = modelBuilder.Entity<Notification>();
            entity4.Ignore(b => b.IsOwner);
            
        }
    }

}
