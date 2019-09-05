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
        public string Title { get; set; }

        public decimal? Bpm { get; set; }

        [Display(Name = "Search Words")]
        public string SearchWords { get; set; }

        public string Lyrics { get; set; }

        [Display(Name = "One Stop")]
        public bool isOneStop { get; set; }

        public string AudioPath { get; set; }

        [Required]
        [Display(Name = "Vocal")]
        public int VocalId { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public Artist Artist { get; set; }
    }
}
