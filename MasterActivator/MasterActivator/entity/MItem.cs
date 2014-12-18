using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterActivator.enumerator;

namespace MasterActivator.entity
{
    class MItem
    {
        public String name { get; set; }
        public String menuName { get; set; }
        public String menuVariable { get; set; }
        public int id { get; set; }
        public float range { get; set; }
        public ItemTypeId type { get; set; }
        public SpellSlot abilitySlot { get; set; }

        public MItem(String name, String menuName, String menuVariable, int id, ItemTypeId type, float range = 0, SpellSlot abilitySlot = SpellSlot.Unknown)
        {
            this.name = name;
            this.menuVariable = menuVariable;
            this.menuName = menuName;
            this.id = id;
            this.range = range;
            this.type = type;
            this.abilitySlot = abilitySlot;
        }
    }
}
