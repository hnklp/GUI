using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Data
{
     public class Mob
        {
            public int MobId { get; set; }
            [Required]
            public string Name { get; set; }
            public int? SpeciesId { get; set; }
            public virtual Species? Species { get; set; }
            public DateTime DateOfCapture { get; set; } = DateTime.Now;

        }
}
