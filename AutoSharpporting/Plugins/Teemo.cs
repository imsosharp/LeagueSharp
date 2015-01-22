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
    public class Teemo : PluginBase
    {
        private  readonly Random Rand = new Random((42 / 13 * DateTime.Now.Millisecond) + DateTime.Now.Second);
        private  Vector2 pos;

        public Teemo()
        {

            Q = new Spell(SpellSlot.Q, 680);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R, 230);
            Q.SetTargetted(0f, 2000f);
            R.SetSkillshot(0.1f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {


                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }

                if (R.CastCheck(Target, "ComboR"))
                {
                    R.Cast(Target);
                }
                if (R.IsReady())
                {
                    int _randRange = Rand.Next(-100, 100);
 
                    pos.X = Player.Position.X + _randRange;
                    pos.Y = Player.Position.Y + _randRange;
                    R.Cast(pos.To3D(),UsePackets);
                }
                if (Orbwalking.InAutoAttackRange(Target) && Player.HealthPercentage() > 30)
                {
                    if (W.IsReady())
                    {
                        W.Cast();
                    }
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Target);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
        }

    }
}
