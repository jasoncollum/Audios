using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Audios.Models
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Publisher")]
        public string Name { get; set; }

        public virtual ICollection<Writer> Writers { get; set; }
    }
}
