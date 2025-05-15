using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class AlbumUser
    {
        [ForeignKey(nameof(Album))]
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public Role Role { get; set; }
    }

    public enum Role
    {
        Owner = 1,
        Viewer = 2
    }
}