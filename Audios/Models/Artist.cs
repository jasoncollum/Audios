using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Artist")]
        public string Name { get; set; }

        //[Required]
        [Display(Name = "Artist Image")]
        public string ImageUrl { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
}
