using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace NiceGuySharp
{
    public class Program
    {
        public static Menu Menu;
        public static Random Rand = new Random(Environment.TickCount);
        public static Obj_AI_Hero MyHero = ObjectManager.Player;
        public static List<Obj_AI_Hero> AllHeroes;
        public static List<Obj_AI_Hero> AllyHeroes;
        public static List<Obj_AI_Hero> EnemyHeroes;
        public static int Deaths,
            Kills,
            Doubles,
            Triples,
            Quadras,
            Pentas,
            AllyKills,
            AllyDeaths,
            AllyDoubles,
            AllyQuadras,
            AllyPentas,
            EnemyKills,
            EnemyDeaths,
            EnemyDoubles,
            EnemyTriples,
            EnemyQuadras,
            EnemyPentas;

        public static int LastSentMessage;
        public static int MinTimeBeforeNewMessage = Rand.Next(5000, 60000);
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {

            Menu = new Menu("Nice Guy Sharp", "niceguy", true);
            Menu.AddItem(new MenuItem("enabled", "Enable").SetValue(true));
            
            Menu.AddToMainMenu();
            FileHandler.DoChecks();
            AllHeroes = ObjectManager.Get<Obj_AI_Hero>().ToList();
            AllyHeroes = AllHeroes.FindAll(hero => hero.IsAlly).ToList();
            EnemyHeroes = AllHeroes.FindAll(hero => !hero.IsAlly).ToList();
            Game.PrintChat("NICE-GUY SHARP SUCCESFULLY LOADED.");
            Game.PrintChat("Edit what it's going to say at:");
            Game.PrintChat(FileHandler.Folder);
            
            string[] onGameStart = File.ReadAllLines(FileHandler.OnGameStartTxt);
            int randMessage = Rand.Next(onGameStart.Count());
            if (onGameStart.Length > 0)
            {
                Game.Say(onGameStart[randMessage]);
            }

            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (MyHero.Deaths > Deaths)
            {
                string[] onDeath = File.ReadAllLines(FileHandler.OnDeathTxt);
                int randMessage = Rand.Next(onDeath.Count());
                if (onDeath.Length > 0)
                {
                    TryToSay(onDeath[randMessage]);
                }
                Deaths = MyHero.Deaths;
            }
            if (MyHero.DoubleKills > Doubles)
            {
                string[] onDouble = File.ReadAllLines(FileHandler.OnDoubleTxt);
                int randMessage = Rand.Next(onDouble.Count());
                if (onDouble.Length > 0)
                {
                    TryToSay(onDouble[randMessage]);
                }
                Doubles = MyHero.DoubleKills;
            }
            if (MyHero.TripleKills > Triples)
            {
                string[] onTriple = File.ReadAllLines(FileHandler.OnTripleTxt);
                int randMessage = Rand.Next(onTriple.Count());
                if (onTriple.Length > 0)
                {
                    TryToSay(onTriple[randMessage]);
                }
                Triples = MyHero.TripleKills;
            }
            if (MyHero.QuadraKills > Quadras)
            {
                string[] onQuadra = File.ReadAllLines(FileHandler.OnQuadraTxt);
                int randMessage = Rand.Next(onQuadra.Count());
                if (onQuadra.Length > 0)
                {
                    TryToSay(onQuadra[randMessage]);
                }
                Quadras = MyHero.QuadraKills;
            }
            if (MyHero.PentaKills > Pentas)
            {
                string[] onPenta = File.ReadAllLines(FileHandler.OnPentaTxt);
                int randMessage = Rand.Next(onPenta.Count());
                if (onPenta.Length > 0)
                {
                    TryToSay(onPenta[randMessage]);
                }
                Pentas = MyHero.PentaKills;
            }
            if (MyHero.ChampionsKilled > Kills)
            {
                string[] onKill = File.ReadAllLines(FileHandler.OnKillTxt);
                int randMessage = Rand.Next(onKill.Count());
                if (onKill.Length > 0)
                {
                    TryToSay(onKill[randMessage]);
                }
                Kills = MyHero.ChampionsKilled;
            }
            if (RealAllyKills() > AllyKills)
            {
                string[] onAllyKills = File.ReadAllLines(FileHandler.OnAllyKillTxt);
                int randMessage = Rand.Next(onAllyKills.Count());
                if (onAllyKills.Length > 0)
                {
                    TryToSay(onAllyKills[randMessage]);
                }
                AllyKills = RealAllyKills();
            }
            if (RealAllyDeaths() > AllyDeaths)
            {
                string[] onAllyDeath = File.ReadAllLines(FileHandler.OnAllyDeathTxt);
                int randMessage = Rand.Next(onAllyDeath.Count());
                if (onAllyDeath.Length > 0)
                {
                    TryToSay(onAllyDeath[randMessage]);
                }
                AllyDeaths = RealAllyDeaths();
            }
            if (RealAllyQuadras() > AllyQuadras)
            {
                string[] onAllyQuadra = File.ReadAllLines(FileHandler.OnAllyQuadraTxt);
                int randMessage = Rand.Next(onAllyQuadra.Count());
                if (onAllyQuadra.Length > 0)
                {
                    TryToSay(onAllyQuadra[randMessage]);
                }
                AllyPentas = RealAllyPentas();
            }
            if (RealAllyPentas() > AllyPentas)
            {
                string[] onAllyPenta = File.ReadAllLines(FileHandler.OnAllyPentaTxt);
                int randMessage = Rand.Next(onAllyPenta.Count());
                if (onAllyPenta.Length > 0)
                {
                    TryToSay(onAllyPenta[randMessage]);
                }
                AllyPentas = RealAllyPentas();
            }
            if (RealEnemyPentas() > EnemyPentas)
            {
                string[] onEnemyPenta = File.ReadAllLines(FileHandler.OnEnemyPentaTxt);
                int randMessage = Rand.Next(onEnemyPenta.Count());
                if (onEnemyPenta.Length > 0)
                {
                    TryToSay(onEnemyPenta[randMessage]);
                }
                EnemyPentas = RealEnemyPentas();
            }

        }

        public static void TryToSay(string message)
        {
            try
            {
                if (Menu.Item("enabled").GetValue<bool>())
                {
                    if (Environment.TickCount - LastSentMessage > MinTimeBeforeNewMessage)
                    {
                        if (message != null)
                        {
                            Utility.DelayAction.Add(message.Length * (new Random(Environment.TickCount).Next(100, 300)), () => Game.Say(message));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Game.OnGameUpdate -= Game_OnGameUpdate;
            }
        }

        public static int RealAllyKills()
        {
            return AllyHeroes.Sum(hero => hero.ChampionsKilled);
        }

        public static int RealAllyDoubles()
        {
            return AllyHeroes.Sum(hero => hero.DoubleKills);
        }

        public static int RealAllyTriples()
        {
            return AllyHeroes.Sum(hero => hero.TripleKills);
        }

        public static int RealAllyQuadras()
        {
            return AllyHeroes.Sum(hero => hero.QuadraKills);
        }

        public static int RealAllyPentas()
        {
            return AllyHeroes.Sum(hero => hero.PentaKills);
        }

        public static int RealAllyDeaths()
        {
            return AllyHeroes.Sum(hero => hero.Deaths);
        }

        public static int RealEnemyPentas()
        {
            return EnemyHeroes.Sum(hero => hero.PentaKills);
        }
    }
}
