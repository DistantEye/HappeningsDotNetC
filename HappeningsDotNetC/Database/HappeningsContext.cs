using HappeningsDotNetC.Entities;
using HappeningsDotNetC.Entities.JoinEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Infrastructure
{
    public class HappeningsContext : DbContext
    {
        public HappeningsContext(DbContextOptions<HappeningsContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Event { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventUser>().HasOne(eu => eu.User).WithMany(u => u.Events).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EventUser>().HasOne(eu => eu.Event).WithMany(e => e.AllUsers).OnDelete(DeleteBehavior.Restrict);
        }
    }

}
