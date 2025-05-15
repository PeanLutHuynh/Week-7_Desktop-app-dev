using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;

namespace SocialNetwork.Data
{
    public class SocialNetworkDbContext : DbContext
    {
        public SocialNetworkDbContext()
        {
        }

        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserFriend> UserFriends { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<AlbumPicture> AlbumPictures { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<AlbumTag> AlbumTags { get; set; }
        public virtual DbSet<AlbumUser> AlbumUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=SocialNetwork.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary keys
            modelBuilder.Entity<UserFriend>()
                .HasKey(uf => new { uf.UserId, uf.FriendId });

            // Configure relationships for UserFriend
            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.Friend)
                .WithMany(u => u.FriendsOf)
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure composite primary key for AlbumPicture
            modelBuilder.Entity<AlbumPicture>()
                .HasKey(ap => new { ap.AlbumId, ap.PictureId });

            // Configure relationships for AlbumPicture
            modelBuilder.Entity<AlbumPicture>()
                .HasOne(ap => ap.Album)
                .WithMany(a => a.Pictures)
                .HasForeignKey(ap => ap.AlbumId);

            modelBuilder.Entity<AlbumPicture>()
                .HasOne(ap => ap.Picture)
                .WithMany(p => p.Albums)
                .HasForeignKey(ap => ap.PictureId);

            // Configure composite primary key for AlbumTag
            modelBuilder.Entity<AlbumTag>()
                .HasKey(at => new { at.AlbumId, at.TagId });

            // Configure relationships for AlbumTag
            modelBuilder.Entity<AlbumTag>()
                .HasOne(at => at.Album)
                .WithMany(a => a.AlbumTags)
                .HasForeignKey(at => at.AlbumId);

            modelBuilder.Entity<AlbumTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.Albums)
                .HasForeignKey(at => at.TagId);

            // Configure composite primary key for AlbumUser
            modelBuilder.Entity<AlbumUser>()
                .HasKey(au => new { au.AlbumId, au.UserId });

            // Configure relationships for AlbumUser
            modelBuilder.Entity<AlbumUser>()
                .HasOne(au => au.Album)
                .WithMany(a => a.Users)
                .HasForeignKey(au => au.AlbumId);

            modelBuilder.Entity<AlbumUser>()
                .HasOne(au => au.User)
                .WithMany(u => u.SharedAlbums)
                .HasForeignKey(au => au.UserId);
        }
    }
}