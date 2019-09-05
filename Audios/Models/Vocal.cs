using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class Vocal
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Vocal")]
        public string Type { get; set; }
    }
}
