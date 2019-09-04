using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Playlist Name")]
        public string Name { get; set; }
    }
}
