using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Tibbuhs
{
    class Program
    {
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static SpellSlot Flash;
        private static PredictionInput FlashTibbers_pi;
        private static PredictionOutput FlashTibbers_po;
        private static Menu menu;
        private static Orbwalking.Orbwalker orbw;
        private static TargetSelector ts;
        private static Obj_AI_Hero Player = ObjectManager.Player;
        static void Main(string[] args)
        {
            if (Player.BaseSkinName != "Annie") return;
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        #region OnGameLoad
        private static void Game_OnGameLoad(EventArgs args)
        {
            #region Spells
            Q = new Spell(SpellSlot.Q, 650);
            Q.SetTargetted(250, 1400);
            W = new Spell(SpellSlot.W, 625);
            W.SetSkillshot(600, (float)(50 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
            R = new Spell(SpellSlot.R, 600);
            R.SetSkillshot(250, 200, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Flash = ObjectManager.Player.GetSpellSlot("SummonerFlash", true);
            SpellList.Add(Q); SpellList.Add(W); SpellList.Add(E); SpellList.Add(R);
            #endregion

            #region Menu
            menu = new Menu("Tibbers is SoSharp", "tibbuhs", true);
            orbw = new Orbwalking.Orbwalker(menu.SubMenu("orbwalker"));
            SimpleTs.AddToMenu(menu);

            menu.AddSubMenu(new Menu("Laning settings", "farm"));
            menu.AddSubMenu(new Menu("Teamfight settings", "combo"));
            menu.AddSubMenu(new Menu("Defensive settings", "defense"));
            menu.AddSubMenu(new Menu("Misc settings", "misc"));

            menu.SubMenu("farm").AddItem(new MenuItem("Qlasthit", "Use Q Lasthit")).SetValue(true);
            menu.SubMenu("farm").AddItem(new MenuItem("Qstunlasthit", "Use Q stun to Lasthit")).SetValue(false);
            menu.SubMenu("farm").AddItem(new MenuItem("Qstunharass", "Use Q Harass")).SetValue(true);
            menu.SubMenu("farm").AddItem(new MenuItem("Qstunharass", "Use Q stun to Harass")).SetValue(true);
            menu.SubMenu("farm").AddItem(new MenuItem("Qharassmana", "Use Q to harass when %mana more than")).SetValue(new Slider(25, 0, 100));
            menu.SubMenu("farm").AddItem(new MenuItem("Elanestuncharge", "Charge E stun in lane")).SetValue(true);
            menu.SubMenu("farm").AddItem(new MenuItem("lanetoggle", "Active:")).SetValue((new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            menu.SubMenu("combo").AddItem(new MenuItem("Qcombo", "Use Q in teamfights")).SetValue(true);
            menu.SubMenu("combo").AddItem(new MenuItem("Wcombo", "Use W in teamfights")).SetValue(true);
            menu.SubMenu("combo").AddItem(new MenuItem("Wcombo", "Only use W if it will hit X enemies")).SetValue(new Slider(1, 1, 5));
            menu.SubMenu("combo").AddItem(new MenuItem("Wcombo", "Only use W if stun ready")).SetValue(false);
            menu.SubMenu("combo").AddItem(new MenuItem("Ecombostuncharge", "Charge E stun in teamfights")).SetValue(true);
            menu.SubMenu("combo").AddItem(new MenuItem("RcomboOnlyOn4Stacks", "Only summon Tibbers if can stun")).SetValue(true);
            menu.SubMenu("combo").AddItem(new MenuItem("FlashTibbers", "Flash-Tibbers to stun")).SetValue(true);
            menu.SubMenu("combo").AddItem(new MenuItem("FlashTibbersmin", "Flash-Tibbers only if it will hit X enemies")).SetValue(new Slider(3,1,5));
            menu.SubMenu("combo").AddItem(new MenuItem("combotoggle", "Active:")).SetValue((new KeyBind(32, KeyBindType.Press)));



            menu.SubMenu("defense").AddItem(new MenuItem("Wstungapcloser", "Use W stun as a gapcloser")).SetValue(true);
            menu.SubMenu("defense").AddItem(new MenuItem("Eenemies", "Use E when enemies near you")).SetValue(true);
            menu.SubMenu("defense").AddItem(new MenuItem("Eenemiesrange", "Use E when enemies closer than")).SetValue(new Slider(250, 0, 1250));


            menu.SubMenu("misc").AddItem(new MenuItem("passivestacker", "Always stack passive with E")).SetValue(false);
            menu.SubMenu("misc").AddItem(new MenuItem("FlashTibbersanytime", "Flash-Tibbers anytime it is possible")).SetValue(true);
            menu.SubMenu("misc").AddItem(new MenuItem("FlashTibbersanytimemin", "Min targets for Flash-Tibbers anytime")).SetValue(new Slider(3, 1, 5));
            menu.SubMenu("misc").AddItem(new MenuItem("AntiGapcloser", "Anti-Gapcloser")).SetValue(true);
            menu.SubMenu("misc").AddItem(new MenuItem("packets", "Use Packets")).SetValue(true);
            #endregion

            #region Events
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            #endregion
        }
        #endregion

        #region OnGameUpdate
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (menu.Item("farmtoggle").GetValue<KeyBind>().Active)
            {
                Laning();
            }
            if (menu.Item("combotoggle").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            PassiveStacker();
        }
        #endregion

        #region OnDraw
        private static void Drawing_OnDraw(EventArgs args)
        {

        }
        #endregion

        #region OnPossibleToInterrupt
        private static void OnPossibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (!target.IsEnemy)
                return;
            if (GetPassiveStacks() == 4)
            {
                if (Q.IsReady())
                {
                    Q.Cast(target, UsePackets());
                }
                else if (W.IsReady() && W.InRange(target.Position))
                {
                    W.Cast(target, UsePackets());
                }
            }
            if (GetPassiveStacks() == 3)
            {
                if (E.IsReady()) E.Cast(UsePackets());
                if (Q.IsReady())
                {
                    Q.Cast(target, UsePackets());
                }
                else if (W.IsReady() && W.InRange(target.Position))
                {
                    W.Cast(target, UsePackets());
                }
            }
        }
        #endregion

        #region AntiGapcloser
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly || gapcloser.Sender == Player || !(menu.Item("AntiGapcloser").GetValue<bool>())) return;
            if (GetPassiveStacks() == 4)
            {
                if (Q.IsReady())
                {
                    Q.Cast(gapcloser.Sender, UsePackets());
                }
                else if (W.IsReady() && W.InRange(gapcloser.Sender.Position))
                {
                    W.Cast(gapcloser.Sender, UsePackets());
                }
            }
            if (GetPassiveStacks() == 3)
            {
                if (E.IsReady()) E.Cast(UsePackets());
                if (Q.IsReady())
                {
                    Q.Cast(gapcloser.Sender, UsePackets());
                }
                else if (W.IsReady() && W.InRange(gapcloser.Sender.Position))
                {
                    W.Cast(gapcloser.Sender, UsePackets());
                }
            }
        }
        #endregion



        #region Laning
        private static void Laning()
        {
            Cast_Q("farm");
            Cast_E("farm");
        }
        #endregion

        #region Combo
        private static void Combo()
        {
            Cast_Q("combo");
            Cast_W();
            Cast_E("combo");
            Cast_R();
            if (menu.Item("FlashTibbersanytime").GetValue<bool>())
            {
                FlashTibbers_pi.Aoe = true; FlashTibbers_pi.Collision = false; FlashTibbers_pi.Delay = 250; FlashTibbers_pi.Range = 1000; FlashTibbers_pi.Speed = float.MaxValue; FlashTibbers_pi.Type = SkillshotType.SkillshotCircle; FlashTibbers_pi.Radius = 100;
                FlashTibbers_po = Prediction.GetPrediction(FlashTibbers_pi);
                var flashtibbers_hitcount = FlashTibbers_po.AoeTargetsHitCount;
                var flashtibbers_hitchance = FlashTibbers_po.Hitchance;
                var flashtibbers_targetpos = FlashTibbers_po.UnitPosition;
                if (flashtibbers_hitcount > menu.Item("FlashTibbersanytimemin").GetValue<int>() && flashtibbers_hitchance >= HitChance.Medium)
                {
                    Player.Spellbook.CastSpell(Flash, flashtibbers_targetpos);
                    R.Cast(flashtibbers_targetpos, UsePackets());
                }
            }
        }
        #endregion



        #region Passive
        private static bool PassiveStacker()
        {
            if(Utility.InFountain())
            {
                if (GetPassiveStacks() < 4)
                {
                    if (W.IsReady()) W.Cast();
                }
                if (GetPassiveStacks() < 4)
                {
                    if (E.IsReady()) E.Cast();
                }
            }

            if(menu.Item("passivestacker").GetValue<bool>())
            {
                if (E.IsReady()) E.Cast();
                return true;
            }
            else return false;
        }
        #endregion

        #region Casting Q
        private static void Cast_Q(string mode)
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            //#TODO work on better targeting, like not wasting Q on tanks.
            #region Q Farm Mode
            if (mode == "farm")
            {
                if (GetPassiveStacks() == 4 && !menu.Item("Qstunlasthit").GetValue<bool>())
                {
                    if (target != null)
                    {
                        Q.Cast(target, UsePackets());
                        if (Orbwalking.CanAttack())
                        {
                            orbw.ForceTarget(target);
                        }
                    }
                }
                var allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly).ToList();
                var minionLastHit = allMinions.Where(x => HealthPrediction.LaneClearHealthPrediction(x, (int)Q.Delay) < Player.GetSpellDamage(x, SpellSlot.Q) * 0.8).OrderBy(x => x.Health);
                var enoughmanatoharass = menu.Item("Qharassmana").GetValue<int>();
                if (!minionLastHit.Any() && menu.Item("Qstunharass").GetValue<bool>() && HaveMana(enoughmanatoharass))
                {
                    if (target != null)
                    {
                        Q.Cast(target, UsePackets());
                        if (Orbwalking.CanAttack())
                        {
                            orbw.ForceTarget(target);
                        }                        
                    }
                }
                var unit = minionLastHit.First();
                Q.CastOnUnit(unit, UsePackets());
            }
            #endregion
            #region Q Combo Mode
            if (mode == "combo")
            {
                if (menu.Item("Qcombo").GetValue<bool>())
                {
                    if (target != null)
                    {
                        Q.Cast(target, UsePackets());
                    }
                }
            }
            #endregion

        }
        #endregion

        #region Casting W
        private static void Cast_W()
        {
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Magical);
            //#TODO work on better targeting, like not wasting Q on tanks.
            if (target != null)
            {
                W.Cast(target, UsePackets());
            }
        }
        #endregion

        #region Casting E
        private static void Cast_E(string mode)
        {
            if(mode == "farm" && !PassiveStacker() && (menu.Item("Elanestuncharge").GetValue<bool>()))
            {
                if(GetPassiveStacks() < 4)
                {
                    if (E.IsReady()) E.Cast();
                }
            }

            if (mode == "combo" && !PassiveStacker() && (menu.Item("Ecombostuncharge").GetValue<bool>()))
            {
                if (GetPassiveStacks() < 4)
                {
                    if (E.IsReady()) E.Cast();
                }
            }
        }
        #endregion

        #region Casting R
        private static void Cast_R()
        {
            if (menu.Item("FlashTibbers").GetValue<bool>())
            {
                FlashTibbers_pi.Aoe = true; FlashTibbers_pi.Collision = false; FlashTibbers_pi.Delay = 250; FlashTibbers_pi.Range = 1000; FlashTibbers_pi.Speed = float.MaxValue; FlashTibbers_pi.Type = SkillshotType.SkillshotCircle; FlashTibbers_pi.Radius = 100;
                FlashTibbers_po = Prediction.GetPrediction(FlashTibbers_pi);
                var flashtibbers_hitcount = FlashTibbers_po.AoeTargetsHitCount;
                var flashtibbers_hitchance = FlashTibbers_po.Hitchance;
                var flashtibbers_targetpos = FlashTibbers_po.UnitPosition;
                if (flashtibbers_hitcount > menu.Item("FlashTibbersmin").GetValue<int>() && flashtibbers_hitchance >= HitChance.Medium)
                {
                    Player.Spellbook.CastSpell(Flash, flashtibbers_targetpos);
                    R.Cast(flashtibbers_targetpos, UsePackets());
                }
            }
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
            var minTargets = menu.Item("flashtibbersmin").GetValue<int>();
            if (menu.Item("RcomboOnlyOn4Stacks").GetValue<bool>())
            {
                if (GetPassiveStacks() == 4)
                {
                    R.Cast(target, UsePackets());
                }
                else if (GetPassiveStacks() == 3)
                {
                    E.Cast();
                    if (GetPassiveStacks() == 4)
                    {
                        R.Cast(target, UsePackets());
                    }
                }
            }
            else
            {
                R.Cast(target, UsePackets());
            }
        }
        #endregion

        #region Utility

        private static int GetPassiveStacks()
        {
            var buffs = ObjectManager.Player.Buffs.Where(buff => (buff.Name.ToLower() == "pyromania" || buff.Name.ToLower() == "pyromania_particle"));
            var buffInstances = buffs as BuffInstance[] ?? buffs.ToArray();
            if (!buffInstances.Any())
                return 0;
            var buf = buffInstances.First();
            var count = buf.Count >= 4 ? 4 : buf.Count;
            return buf.Name.ToLower() == "pyromania_particle" ? 4 : count;
        }

        private static double SpellDmg(Obj_AI_Hero target, SpellSlot spell)
        {
            var spelldamage = ObjectManager.Player.GetSpellDamage(target, spell);
            return spelldamage;
        }

        private static double ComboDmg()
        {
            return 0.0d;
        }

        private static bool HaveMana(int minMana)
        {
            if (((Player.Mana / Player.MaxMana) * 100) > (float)minMana)
            {
                return true;
            }
            else return false;
        }

        private static bool UsePackets()
        {
            if (menu.Item("packets").GetValue<bool>()) return true;
            else return false;
        }
        #endregion

    }
}
