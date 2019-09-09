using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Song Title")]
        public string Title { get; set; }

        public decimal? Bpm { get; set; }

        [Display(Name = "Search Words")]
        public string SearchWords { get; set; }

        public string Lyrics { get; set; }

        [Required]
        [Display(Name = "One Stop")]
        public bool isOneStop { get; set; }

        [Required]
        [Display(Name = "Audio Url")]
        public string AudioUrl { get; set; }

        [Required]
        [Display(Name = "Vocal")]
        public int VocalId { get; set; }

        public Vocal Vocal { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public Artist Artist { get; set; }

        public virtual ICollection<Writer> Writers { get; set; }
    }
}
