using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class PlaylistSong
    {
        public int Id { get; set; }

        public int PlaylistId { get; set; }

        public int SongId { get; set; }

        public int TrackNumber { get; set; }
    }
}
