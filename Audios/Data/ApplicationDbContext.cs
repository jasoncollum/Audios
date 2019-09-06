using System;
using System.Collections.Generic;
using System.Text;
using Audios.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Audios.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Artist> Artist { get; set; }
        public DbSet<Song> Song { get; set; }
        public DbSet<Writer> Writer { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
        public DbSet<PRO> PRO { get; set; }
        public DbSet<Vocal> Vocal { get; set; }
        public DbSet<SongWriter> SongWriter { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<PlaylistSong> PlaylistSong { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create a new ADMIN USER for Identity Framework
            ApplicationUser user = new ApplicationUser
            {
                FirstName = "Jason",
                LastName = "Collum",
                UserName = "jason@email.com",
                NormalizedUserName = "JASON@EMAIL.COM",
                Email = "jason@email.com",
                NormalizedEmail = "JASON@EMAIL.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "7f434309-a4d9-48e9-9ebb-8803db794577",
                Id = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            // Create three Artists
            modelBuilder.Entity<Artist>().HasData(
                new Artist()
                {
                    Id = 1,
                    Name = "Perrin Lamb",
                    ImageUrl = "~/images/perrin-lamb.jpg"
                },
                new Artist()
                {
                    Id = 2,
                    Name = "Bandit Heart",
                    ImageUrl = "~/images/bandit-heart.png"
                },
                new Artist()
                {
                    Id = 3,
                    Name = "Veaux",
                    ImageUrl = "~/images/veaux.png"
                }
            );

            // Create five Songs
            modelBuilder.Entity<Song>().HasData(
                new Song()
                {
                    Id = 1,
                    Title = "Everyone's Got Something",
                    Bpm = null,
                    SearchWords = "acoustic, folk",
                    Lyrics = "",
                    VocalId = 1,
                    ArtistId = 1,
                    AudioUrl = "",
                    isOneStop = true
                },
                new Song()
                {
                    Id = 2,
                    Title = "Have Yourself A Merry Little Christmas",
                    Bpm = null,
                    SearchWords = "acoustic, folk, christmas, holiday",
                    Lyrics = "",
                    VocalId = 1,
                    ArtistId = 1,
                    AudioUrl = "",
                    isOneStop = false
                },
                new Song()
                {
                    Id = 3,
                    Title = "Back To Black",
                    Bpm = null,
                    SearchWords = "piano",
                    Lyrics = "",
                    VocalId = 1,
                    ArtistId = 2,
                    AudioUrl = "",
                    isOneStop = true
                },
                new Song()
                {
                    Id = 4,
                    Title = "Back In Baby's Arms",
                    Bpm = null,
                    SearchWords = "piano, cover, Patsy Cline",
                    Lyrics = "",
                    VocalId = 1,
                    ArtistId = 2,
                    AudioUrl = "",
                    isOneStop = false
                },
                new Song()
                {
                    Id = 5,
                    Title = "Safe And Sound",
                    Bpm = null,
                    SearchWords = "pop rock, cover, Sheryl Crow, trailer, movie",
                    Lyrics = "",
                    VocalId = 1,
                    ArtistId = 3,
                    AudioUrl = "",
                    isOneStop = false
                }
            );

            // Create Vocal Types: Male, Female, MaleFemale, None
            modelBuilder.Entity<Vocal>().HasData(
                new Vocal()
                {
                    Id = 1,
                    Type = "Male"
                },
                new Vocal()
                {
                    Id = 2,
                    Type = "Female"
                },
                new Vocal()
                {
                    Id = 3,
                    Type = "Male/Female"
                },
                new Vocal()
                {
                    Id = 4,
                    Type = "None"
                }
            );

            // Create Writer
            modelBuilder.Entity<Writer>().HasData(
                new Writer()
                {
                    Id = 1,
                    Name = "Perrin Lamb",
                    PublisherId = 1,
                    PROId = 1
                }
            );

            // Create Publisher
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher()
                {
                    Id = 1,
                    Name = "Sorted Noise"
                }
            );

            // Create three PROs
            modelBuilder.Entity<PRO>().HasData(
                new PRO()
                {
                    Id = 1,
                    Name = "ASCAP"
                },
                new PRO()
                {
                    Id = 2,
                    Name = "BMI"
                },
                new PRO()
                {
                    Id = 3,
                    Name = "SESAC"
                }
            );

            // Create SongWriter join table
            modelBuilder.Entity<SongWriter>().HasData(
                new SongWriter()
                {
                    Id = 1,
                    SongId = 1,
                    WriterId = 1
                }
            );

            // Create one Playlist
            modelBuilder.Entity<Playlist>().HasData(
                new Playlist()
                {
                    Id = 1,
                    Name = "Perrin Lamb Songs",
                    ApplicationUserId = "00000000-ffff-ffff-ffff-ffffffffffff"
                }
            );

            // Create two PlaylistSongs
            modelBuilder.Entity<PlaylistSong>().HasData(
                new PlaylistSong()
                {
                    Id = 1,
                    PlaylistId = 1,
                    SongId = 1,
                    TrackNumber = 1
                },
                new PlaylistSong()
                {
                    Id = 2,
                    PlaylistId = 1,
                    SongId = 2,
                    TrackNumber = 2
                }
            );
        }
    }
}
