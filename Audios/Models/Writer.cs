using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class Writer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Writer")]
        public string Name { get; set; }

        [Required]
        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        [Required]
        public int PROId { get; set; }

        public PRO PRO { get; set; }
    }
}
