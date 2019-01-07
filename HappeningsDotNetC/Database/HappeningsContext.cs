using HappeningsDotNetC.Models;
using HappeningsDotNetC.Models.JoinEntities;
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
        public DbSet<Happening> Happenings { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HappeningUser>().HasOne(eu => eu.User).WithMany(u => u.Happenings).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HappeningUser>().HasOne(eu => eu.Happening).WithMany(e => e.AllUsers).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HappeningUser>().HasOne(eu => eu.Reminder).WithOne(r => r.HappeningUser).OnDelete(DeleteBehavior.Cascade);
        }
    }

}
