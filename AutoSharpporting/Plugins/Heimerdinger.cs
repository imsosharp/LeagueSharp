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
    public class Heimerdinger : PluginBase
    {
        public Vector2 pos;

        public Heimerdinger()
        {
            Q = new Spell(SpellSlot.Q, 525);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 100);


            W.SetSkillshot(250f, 200, 1400, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.51f, 120, 1200, false, SkillshotType.SkillshotCircle);
        }
        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                if (W.CastCheck(Target, "ComboW"))
                {
                    var pred = W.GetPrediction(Target);
                    W.Cast(pred.CastPosition);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    var pred = E.GetPrediction(Target);
                    E.Cast(pred.CastPosition);
                }
                if (R.IsReady() && Player.CountEnemiesInRange(W.Range) > 2)
                {
                    R.Cast();
                }
                if (Q.IsReady() && Player.CountEnemiesInRange(1300)>2)
                {
                    Random rnd = new Random();
                    pos.X = Player.Position.X + rnd.Next(-20,20); 
                    pos.Y = Player.Position.Y + rnd.Next(-20,20);
                    Q.Cast(pos.To3D());
                }
               

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
