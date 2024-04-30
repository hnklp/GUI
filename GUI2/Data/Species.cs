using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EFCore.Data
{
    public class Species
    {
        public int SpeciesId { get; set; }
        public string Title { get; set; }
        public HostilityLevel Hostility { get; set; }

        public virtual List<Mob>? Mobs { get; set; }
    }
}
