using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class Album
    {
        public Album()
        {
            this.Pictures = new HashSet<AlbumPicture>();
            this.AlbumTags = new HashSet<AlbumTag>();
            this.Users = new HashSet<AlbumUser>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string BackgroundColor { get; set; }

        public bool IsPublic { get; set; }

        [ForeignKey(nameof(Owner))]
        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }

        // Navigation properties
        public virtual ICollection<AlbumPicture> Pictures { get; set; }

        public virtual ICollection<AlbumTag> AlbumTags { get; set; }

        public virtual ICollection<AlbumUser> Users { get; set; }
    }
}