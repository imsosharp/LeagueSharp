using System;
using AutoSharpporting.Util;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;

namespace AutoSharpporting.Plugins
{
	public class Shaco : PluginBase
	{
		public Vector2 pos;
		public bool Rcast;

		public Shaco()
		{
			Q = new Spell(SpellSlot.Q, 400);
			W = new Spell(SpellSlot.W, 425);
			E = new Spell(SpellSlot.E, 625);
			R = new Spell(SpellSlot.R, 0);
		}

		public override void OnUpdate(EventArgs args)
		{
			var target1 = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
			if (target1==null) return;
			var rnd = new Random();
			var move = rnd.Next(100, 200);

			if (Q.IsReady() && Player.HealthPercentage() < 50 && Player.CountEnemiesInRange(1300) >= 1)
			{
				if (Player.Team == GameObjectTeam.Order)
				{
					pos.X = Player.Position.X -move;
					pos.Y = Player.Position.Y -move;
				}
				else
				{
					pos.X = Player.Position.X +move;
					pos.Y = Player.Position.Y +move;
				}

				Q.Cast(pos.To3D());
			}

			if (R.IsReady() && Player.CountEnemiesInRange(1300) >= 1 &&Player.HealthPercentage() < 80)
			{
				R.Cast();
			}

			if (W.IsReady() && Player.CountEnemiesInRange(1300) >= 2)
			{
				if (Player.Team == GameObjectTeam.Order)
				{
					pos.X = Player.Position.X +move;
					pos.Y = Player.Position.Y +move;
				}
				else
				{
                    pos.X = Player.Position.X - move;
                    pos.Y = Player.Position.Y - move;
				}				
				W.Cast(pos.To3D());
                }
				if (E.IsReady())
				{

					E.CastOnUnit(target1);
				}
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}