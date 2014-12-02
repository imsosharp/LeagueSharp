using LeagueSharp;
using LeagueSharp.Common;

namespace Ultimate_Carry_Prevolution.Plugin
{
	class Kalista : Champion 
	{

		// just there nothing inside trolololololol

		public Kalista()
		{
			SetSpells();
			//LoadMenu();
		}

		private void SetSpells()
		{
			Q = new Spell(SpellSlot.Q, 1150);
			Q.SetSkillshot(0.25f, 40, 1200, true, SkillshotType.SkillshotLine);
		}

		public override void OnCombo()
		{
			//Cast_Q();
			// just couse i wrote for someone this Code, this champ is not supportet for now
		}

		//private void Cast_Q()
		//{
		//	if(!Q.IsReady() || target == null)
		//		return;
		//	var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
		//	var qPred = Q.GetPrediction(target);
		//	if(qPred.Hitchance >= HitChance.Medium)
		//		Q.Cast(target, UsePackets());
		//	if(qPred.Hitchance != HitChance.Collision)
		//		return;
		//	var coll = qPred.CollisionObjects;
		//	var goal = coll.FirstOrDefault(obj => Q.GetPrediction(obj).Hitchance >= HitChance.Medium && QDamage() > obj.Health);
		//	if(goal != null)
		//		Q.Cast(goal, UsePackets());
		//}

		//private float QDamage()
		//{
		//	throw new NotImplementedException();
		//}


	}
}
