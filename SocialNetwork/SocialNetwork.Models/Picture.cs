using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Models
{
    public class Picture
    {
        public Picture()
        {
            this.Albums = new HashSet<AlbumPicture>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Caption { get; set; }

        [Required]
        public string Path { get; set; }

        // Navigation properties
        public virtual ICollection<AlbumPicture> Albums { get; set; }
    }
}