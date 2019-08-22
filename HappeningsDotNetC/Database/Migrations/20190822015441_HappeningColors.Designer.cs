﻿// <auto-generated />
using System;
using HappeningsDotNetC.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HappeningsDotNetC.Database.Migrations
{
    [DbContext(typeof(HappeningsContext))]
    [Migration("20190822015441_HappeningColors")]
    partial class HappeningColors
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099");

            modelBuilder.Entity("HappeningsDotNetC.Models.Happening", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ControllingUserId");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("Flavor");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("ControllingUserId");

                    b.ToTable("Happenings");
                });

            modelBuilder.Entity("HappeningsDotNetC.Models.JoinEntities.HappeningUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("HappeningId");

                    b.Property<bool>("IsPrivate");

                    b.Property<Guid?>("ReminderId")
                        .IsRequired();

                    b.Property<int>("ReminderXMinsBefore");

                    b.Property<Guid>("UserId");

                    b.Property<int>("UserStatus");

                    b.HasKey("Id");

                    b.HasIndex("HappeningId");

                    b.HasIndex("ReminderId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("HappeningUser");
                });

            modelBuilder.Entity("HappeningsDotNetC.Models.Reminder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("HappeningTime");

                    b.Property<Guid?>("HappeningUserId")
                        .IsRequired();

                    b.Property<bool>("IsSilenced");

                    b.Property<DateTime>("StartRemindAt");

                    b.HasKey("Id");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("HappeningsDotNetC.Models.SystemData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("OpenRegistration");

                    b.HasKey("Id");

                    b.ToTable("_SystemData");
                });

            modelBuilder.Entity("HappeningsDotNetC.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CalendarVisibleToOthers");

                    b.Property<string>("FriendlyName");

                    b.Property<string>("Hash");

                    b.Property<string>("Name");

                    b.Property<int>("Role");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HappeningsDotNetC.Models.Happening", b =>
                {
                    b.HasOne("HappeningsDotNetC.Models.User", "ControllingUser")
                        .WithMany()
                        .HasForeignKey("ControllingUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HappeningsDotNetC.Models.JoinEntities.HappeningUser", b =>
                {
                    b.HasOne("HappeningsDotNetC.Models.Happening", "Happening")
                        .WithMany("AllUsers")
                        .HasForeignKey("HappeningId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("HappeningsDotNetC.Models.Reminder", "Reminder")
                        .WithOne("HappeningUser")
                        .HasForeignKey("HappeningsDotNetC.Models.JoinEntities.HappeningUser", "ReminderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HappeningsDotNetC.Models.User", "User")
                        .WithMany("Happenings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
