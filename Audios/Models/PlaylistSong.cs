using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class PlaylistSong
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; }

        [Required]
        public int SongId { get; set; }

        public Song Song { get; set; }

        [Required]
        public int TrackNumber { get; set; }
    }
}
