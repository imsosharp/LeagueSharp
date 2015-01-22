//from AlrikSharp

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
    public class Gragas : PluginBase
    {
        public GameObject Bomb;
        public Vector3 UltPos;
        public Obj_AI_Hero CurrentQTarget;

        public Gragas()
        {

            Q = new Spell(SpellSlot.Q, 775);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 1050);
            Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.3f, 50, 1000, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.3f, 700, 1000, false, SkillshotType.SkillshotCircle);

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        public double BombMaxDamageTime { get; set; }
        public double BombCreateTime { get; set; }
        public bool BarrelIsCast { get; set; }
        public bool Exploded { get; set; }

        private void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                Bomb = sender;
                BombCreateTime = Game.Time;
                BombMaxDamageTime = BombCreateTime + 2;
                BarrelIsCast = true;
            }
            if (sender.Name == "Gragas_Base_R_End.troy")
            {
                Exploded = true;
                UltPos = sender.Position;
                Utility.DelayAction.Add(3000, () => { Exploded = false; });
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                Bomb = null;
            }
        }

        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {
                Combo(Target);
            }

        }

        private void ThrowBarrel(Obj_AI_Hero tar)
        {
            if (BarrelIsCast) return;
            if (Q.Cast(tar) == Spell.CastStates.SuccessfullyCasted)
            {
                BarrelIsCast = true;
                CurrentQTarget = tar;
            }
        }

        private bool SecondQReady()
        {
            return Q.IsReady() && Bomb != null;
        }

        public bool TargetCloseToQEdge(Obj_AI_Hero t)
        {
            var qPos = Bomb.Position;
            var qRadius = Bomb.BoundingRadius;
            var disTtoQ = t.Distance(qPos);
            var difference = qRadius - disTtoQ;
            if (disTtoQ > qRadius) return false;
            return difference > 5 && difference < 40;
        }

        private void ExplodeBarrel()
        {
            if (!BarrelIsCast) return;
            Q.Cast();
            BarrelIsCast = false;
            CurrentQTarget = null;
        }

        public bool TargetIsInQ(Obj_AI_Hero t)
        {
            var qPos = Bomb.Position;
            var qRadius = Bomb.BoundingRadius;
            var disTtoQ = t.Distance(qPos);

            if (disTtoQ > qRadius) return false;
            return true;
        }

        private bool RKillStealIsTargetInQ(Obj_AI_Hero target)
        {
            return Bomb != null && TargetIsInQ(target);
        }

        private void Combo(Obj_AI_Hero t)
        {
            var useQ = true;
            var useW = true;
            var useE = true;
            var useR = true;
            if (useW && W.IsReady() && t.IsValidTarget(Q.Range))
            {
                W.Cast();
            }
            if (useQ && Q.IsReady())
            {
                if (FirstQReady() && t.IsValidTarget(Q.Range))
                {
                    ThrowBarrel(t);
                }
                if (SecondQReady() && CurrentQTarget != null)
                {
                    if (TargetCloseToQEdge(CurrentQTarget)) ExplodeBarrel();
                    if (CurrentQTarget.IsMoving && TargetIsInQ(CurrentQTarget))
                    {
                        ExplodeBarrel();
                    }
                }
            }


            if (useE && E.IsReady())
            {
                if (t.IsValidTarget(E.Range))
                {
                    if (E.WillHit(t, E.GetPrediction(t).CastPosition))
                    {
                        if (E.Cast(t) == Spell.CastStates.SuccessfullyCasted)
                        {
                            if (ObjectManager.Player.HasBuff("gragaswself"))
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackTo, t);
                        }
                    }
                }
            }


            if (useR && R.IsReady())
            {
                if (t.IsValidTarget(R.Range))
                {
                    if (R.IsKillable(t))
                    {
                        if (RKillStealIsTargetInQ(t))
                        {
                            if (Q.IsKillable(t))
                            {
                                ExplodeBarrel();
                            }
                        }
                        else
                        {
                            var pred = Prediction.GetPrediction(t, R.Delay, R.Width / 2, R.Speed);
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        private bool FirstQReady()
        {
            if (Q.IsReady() && Bomb == null)
            {
                BarrelIsCast = false;
                return true;
            }
            return false;
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
