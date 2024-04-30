using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.Data
{
     public class Mob
        {
            public int MobId { get; set; }
            public string? Name { get; set; }
            public int SpeciesId { get; set; }
            public virtual Species? Species { get; set; }
            public DateTime Date { get; set; } = DateTime.Now;

        }
}
