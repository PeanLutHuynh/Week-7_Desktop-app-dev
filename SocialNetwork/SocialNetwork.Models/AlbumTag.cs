using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class AlbumTag
    {
        [ForeignKey(nameof(Album))]
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }

        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}