using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SocialNetwork.Models
{
    public class User
    {
        public User()
        {
            // Initialize collections
            this.Friends = new HashSet<UserFriend>();
            this.FriendsOf = new HashSet<UserFriend>();
            this.Albums = new HashSet<Album>();
            this.SharedAlbums = new HashSet<AlbumUser>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+<>?])[A-Za-z\d!@#$%^&*()_+<>?]{6,50}$",
            ErrorMessage = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 digit, and 1 special symbol")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+([._-][a-zA-Z0-9]+)*@[a-zA-Z0-9]+([.-][a-zA-Z0-9]+)*\.[a-zA-Z]{2,}$",
            ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [MaxLength(1024 * 1024)] // Max size 1MB
        public byte[] ProfilePicture { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? LastTimeLoggedIn { get; set; }

        [Range(1, 120)]
        public int? Age { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation properties
        public virtual ICollection<UserFriend> Friends { get; set; }

        public virtual ICollection<UserFriend> FriendsOf { get; set; }

        public virtual ICollection<Album> Albums { get; set; }

        public virtual ICollection<AlbumUser> SharedAlbums { get; set; }
    }
}