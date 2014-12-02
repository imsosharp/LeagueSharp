using System.Collections.Generic;
using LeagueSharp;

namespace xSLx_Data
{
	class Spelldata
	{
		public List<Spell> Spelllist;
 		
		public List<Spell> GetSpellsCollision(List<Obj_AI_Hero> championList)
		{
			GenerateList();
			return Spelllist;
		}

		private void GenerateList()
		{
			// Will be Added with all Spells soon, its just for testing right now 
			// If you cant Wait, send us some Spells like below
			// Lexxes.
			Spelllist = new List<Spell>
			{
				new Spell(SpellSlot.E, "Ahri", "AhriSeduce", "AhriSeduceMissile", true, true),
				new Spell(SpellSlot.Q, "Amumu", "BandageToss", "SadMummyBandageToss", true, true)
			};
		}
		internal class Spell
		{
			public SpellSlot Spellslot;
			public string HeroName;
			public string Spellname;
			public string SDataName;
			public bool CollisionMinion;
			public bool CollisionHero;

			public Spell (SpellSlot slot,string champname,string spellname,string sdataname,bool minioncollision,bool champcollision)
			{
				Spellslot = slot;
			HeroName =champname;
			 Spellname =spellname;
			 SDataName =sdataname;
			 CollisionMinion=minioncollision;
			 CollisionHero = champcollision;
			}

		}
	}
}
