using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Support.Evade;
using Support.Util;
using ActiveGapcloser = Support.Util.ActiveGapcloser;
using SpellData = LeagueSharp.SpellData;

namespace Support.Plugins
{
    public class Leblanc : PluginBase
    {

        private bool firstW = false;
        public Leblanc()
        {

            Q = new Spell(SpellSlot.Q, 720);
            Q.SetTargetted(0.5f, 1500f);

            W = new Spell(SpellSlot.W, 670);
            W.SetSkillshot(0.6f, 220f, 1900f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 900);
            E.SetSkillshot(0.3f, 80f, 1650f, true, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 720);
        }


        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {

                if (Q.IsReady() && R.IsReady() && Target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(Target);
                    Utility.DelayAction.Add(100,() => R.CastOnUnit(Target));
                }

                if (W.IsReady() && Target.IsValidTarget(W.Range) && !firstW && (Player.HealthPercentage() > 30 || W.IsKillable(Target)))
                {
                    W.Cast(Target);
                    firstW = true;
                }
                if (Q.IsReady() && Target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(Target);
                }
                if (R.IsReady() && Target.IsValidTarget(Q.Range))
                {
                    R.CastOnUnit(Target);
                }
                if (E.IsReady() && Target.IsValidTarget(700))
                {
                    E.CastIfHitchanceEquals(Target, HitChance.Medium);
                }
                if (W.IsReady() && firstW && Player.HealthPercentage() < 30)
                {
                    W.Cast();
                    firstW = false;
                }

                if (W.Instance.State == SpellState.Cooldown)
                {
                    firstW = false;
                }
            }

            var pet = Player.Pet as Obj_AI_Base;
            var isPetValid = pet != null && pet.IsValid && !pet.IsDead && pet.Health > 0 && pet.CanMove;
            if (isPetValid)
            {
                Utility.DelayAction.Add(
                       100,
                       () =>
                       {
                           pet.IssueOrder(
                               GameObjectOrder.MoveTo,
                               (pet.Position + 500 * ((pet.Position - Player.Position).Normalized())));
                       });
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
