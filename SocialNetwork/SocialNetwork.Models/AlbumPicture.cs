using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetwork.Models
{
    public class AlbumPicture
    {
        [ForeignKey(nameof(Album))]
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }

        [ForeignKey(nameof(Picture))]
        public int PictureId { get; set; }
        public virtual Picture Picture { get; set; }
    }
}