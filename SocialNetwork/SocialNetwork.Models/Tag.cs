using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SocialNetwork.Models.Attributes;

namespace SocialNetwork.Models
{
    public class Tag
    {
        public Tag()
        {
            this.Albums = new HashSet<AlbumTag>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [TagAttribute]
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<AlbumTag> Albums { get; set; }
    }
}