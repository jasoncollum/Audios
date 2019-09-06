using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class SongWriter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SongId { get; set; }

        [Required]
        public int WriterId { get; set; }
    }
}
