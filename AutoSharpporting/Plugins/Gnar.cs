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
    public class Gnar : PluginBase
    {

        public static Spell QMini;
        public static Spell WMini;
        public static Spell EMini;
        public static Spell RMini;

        public static Spell QMega;
        public static Spell WMega;
        public static Spell EMega;
        public static Spell RMega;

        public Gnar()
        {
            QMini = new Spell(SpellSlot.Q, 1100);
            WMini = new Spell(SpellSlot.W);
            EMini = new Spell(SpellSlot.E, 475);
            RMini = new Spell(SpellSlot.R);
            // Mega
            QMega = new Spell(SpellSlot.Q, 1100);
            WMega = new Spell(SpellSlot.W, 525);
            EMega = new Spell(SpellSlot.E, 475);
            RMega = new Spell(SpellSlot.R, 420);

            // Finetune spells
            // Mini
            QMini.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
            EMini.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.SkillshotCircle);
            // Mega
            QMega.SetSkillshot(0.25f, 80, 1200, true, SkillshotType.SkillshotLine);
            WMega.SetSkillshot(0.25f, 80, float.MaxValue, false, SkillshotType.SkillshotLine);
            EMega.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.SkillshotCircle);
            RMega.Delay = 0.25f;
        }

        public override void OnUpdate(EventArgs args)
        {


            if (ComboMode)
            {
                OnCombo(Target);
            }


        }


        public void OnCombo(Obj_AI_Hero target)
        {
           
            // Mini
            if (IsMiniGnar(Player))
            {
                // Q usage
                if (QMini.IsReady())
                {
                    var pred = QMini.GetPrediction(target);
                    QMini.Cast(pred.CastPosition);
                }

                // E usage (only when transforming into Mega Gnar)
                if (EMini.IsReady() && IsAboutToTransform(Player))
                {
                    var pred = EMini.GetPrediction(target);
                    EMini.Cast(pred.CastPosition);
                }
            }
            // Mega
            else
            {
                // R usage
                #region Ult calculations

                if (RMega.IsReady())
                {
                        var prediction = Prediction.GetPrediction(target, RMega.Delay);
                        if (prediction.Hitchance >= HitChance.High && RMega.InRange(prediction.UnitPosition))
                        {
                            // 12 angle checks for casting, prefer to player direction
                            var direction = (Player.ServerPosition - prediction.UnitPosition).Normalized();
                            var maxAngle = 180f;
                            var step = maxAngle / 6f;
                            var currentAngle = 0f;
                            var currentStep = 0f;
                            while (true)
                            {
                                // Validate the counter, break if no valid spot was found in previous loops
                                if (currentStep > maxAngle && currentAngle < 0)
                                    break;

                                // Check next angle
                                if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                                {
                                    currentAngle = (currentStep) * (float)Math.PI / 180;
                                    currentStep += step;
                                }
                                else if (currentAngle > 0)
                                    currentAngle = -currentAngle;

                                Vector3 checkPoint;

                                // One time only check for direct line of sight without rotating
                                if (currentStep == 0)
                                {
                                    currentStep = step;
                                    checkPoint = prediction.UnitPosition + 500 * direction;
                                }
                                // Rotated check
                                else
                                    checkPoint = prediction.UnitPosition + 500 * direction.Rotated(currentAngle);

                                // Check for a wall between the checkPoint and the target position
                                if (prediction.UnitPosition.GetFirstWallPoint(checkPoint).HasValue)
                                {
                                    // Cast ult into the direction where the wall is located
                                    R.Cast(Player.Position + 500 * (checkPoint - prediction.UnitPosition).Normalized());
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion

                // W usage
                if (WMega.IsReady())
                {
                        W.Cast(target);
                }

                // E usasge
                if (E.IsReady())
                {
                    if (target != null)
                    {
                        // Cast without much logic
                        E.Cast(target);
                    }
                }

                // Q usage
                if (QMega.IsReady())
                {
                    if (target != null)
                    {
                        // Cast without much logic
                        Q.Cast(target);
                    }
                }
            }
        }

        public bool IsAboutToTransform(Obj_AI_Hero target)
        {
            return IsMiniGnar(target) && (target.Mana == target.MaxMana && (target.HasBuff("gnartransformsoon") || target.HasBuff("gnartransform"))) || // Mini to mega
                IsMegaGnar(target) && target.ManaPercentage() <= 0.1; // Mega to mini
        }

        public bool IsMiniGnar(Obj_AI_Hero target)
        {
            return target.BaseSkinName == "Gnar";
        }

        public bool IsMegaGnar(Obj_AI_Hero target)
        {
            return target.BaseSkinName == "gnarbig";
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
