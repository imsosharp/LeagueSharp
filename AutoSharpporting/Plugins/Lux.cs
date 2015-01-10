//lux get part of script from ChewyMoon
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

    public class Lux : PluginBase
    {
        public static GameObject EGameObject;

        public Lux()
        {
          
            Q = new Spell(SpellSlot.Q, 1300);
            W = new Spell(SpellSlot.W, 1075);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 3340);

            Q.SetSkillshot(0.25f, 70, 1200, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 150, 1200, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 275, 1300, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1, 190, float.MaxValue, false, SkillshotType.SkillshotLine);

        }


        public override void OnUpdate(EventArgs args)
        {

            KS();
            StealBlue();
            StealRed();

            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    CastQ(Target);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    CastE(Target);
                }
            }

        }

        private void StealBlue()
        {
            if (!R.IsReady()) return;

            var blueBuffs = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name.ToUpper().Equals("SRU_BLUE"));
            foreach (
                var blueBuff in
                    blueBuffs.Where(
                        blueBuff => Player.GetSpellDamage(blueBuff, SpellSlot.R) > blueBuff.Health))
            {
                R.Cast(blueBuff, UsePackets);
            }
        }

        private void StealRed()
        {
            if (!R.IsReady()) return;

            var redBuffs = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name.ToUpper().Equals("SRU_RED"));
            foreach (
                var redBuff in
                    redBuffs.Where(
                        redBuff => Player.GetSpellDamage(redBuff, SpellSlot.R) > redBuff.Health))
            {
                R.Cast(redBuff, UsePackets);
            }
        }

        public  bool EActivated
        {
            get { return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 || EGameObject != null; }
        }




        private void CastE(Obj_AI_Hero target)
        {
            if (EActivated)
            {
                if (
                    !ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.IsValidTarget())
                        .Any(enemy => enemy.Distance(EGameObject.Position) <= E.Width)) return;

                var isInAaRange = Player.Distance(target) <= Orbwalking.GetRealAutoAttackRange(Player);

                if (isInAaRange && !HasPassive(target))
                    E.Cast();

                // Pop E if the target is out of AA range
                if (!isInAaRange)
                    E.Cast();
            }
            else
            {
                E.Cast(target);
            }
        }

        private void CastQ(Obj_AI_Base target)
        {
            var input = Q.GetPrediction(target);
            var col = Q.GetCollision(Player.ServerPosition.To2D(), new List<Vector2> { input.CastPosition.To2D() });
            var minions = col.Where(x => !(x is Obj_AI_Hero)).Count(x => x.IsMinion);

            if (minions <= 1)
                Q.Cast(input.CastPosition);
        }

        public  bool HasPassive(Obj_AI_Hero hero)
        {
            return hero.HasBuff("luxilluminatingfraulein");
        }

        public void KS()
        {
            if (!R.IsReady())
                return;
            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsValidTarget())
                        .Where(x => !x.IsZombie)
                        .Where(x => !x.IsDead)
                        .Where(enemy => Player.GetDamageSpell(enemy, SpellSlot.R).CalculatedDamage > enemy.Health))
            {
                R.Cast(enemy, UsePackets);
                return;
            }
        }



        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboRKS", "Use R KS", true);
        }
    }
}


