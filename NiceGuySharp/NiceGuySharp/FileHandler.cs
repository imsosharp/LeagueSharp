using System;
using System.Linq;
using System.IO;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;

namespace NiceGuySharp
{
    internal class FileHandler
    {
        public static string Folder = Config.LeagueSharpDirectory + @"\NiceGuySharp\";


        public static string OnGameStartTxt;
        public static string OnDeathTxt;
        public static string OnGameEndTxt;

        public static string OnKillTxt;
        public static string OnDoubleTxt;
        public static string OnTripleTxt;
        public static string OnQuadraTxt;
        public static string OnPentaTxt;

        public static string OnAllyKillTxt;
        public static string OnAllyDoubleTxt;
        public static string OnAllyTripleTxt;
        public static string OnAllyQuadraTxt;
        public static string OnAllyPentaTxt;

        public static string OnEnemyKillTxt;
        public static string OnEnemyDoubleTxt;
        public static string OnEnemyTripleTxt;
        public static string OnEnemyQuadraTxt;
        public static string OnEnemyPentaTxt;

        public static string[] ParseFile(Type file)
        {
            if (file != null)
            {
                
            }
            return new string[1];
        }

        public static void DoChecks()
        {
            OnGameStartTxt = Folder + "OnGameStart.txt";
            OnGameEndTxt = Folder + "OnGameEnd.txt";

            OnKillTxt = Folder + "OnKill.txt";
            OnDeathTxt = Folder + "OnDeath.txt";
            OnDoubleTxt = Folder + "OnDouble.txt";
            OnTripleTxt = Folder + "OnTriple.txt";
            OnQuadraTxt = Folder + "OnQuadra.txt";
            OnPentaTxt = Folder + "OnPenta.txt";

            OnAllyKillTxt = Folder + "OnAllyKill.txt";
            OnAllyDoubleTxt = Folder + "OnAllyDouble.txt";
            OnAllyTripleTxt = Folder + "OnAllyTriple.txt";
            OnAllyQuadraTxt = Folder + "OnAllyQuadra.txt";
            OnAllyPentaTxt = Folder + "OnAllyPenta.txt";

            OnEnemyKillTxt = Folder + "OnEnemyKill.txt";
            OnEnemyDoubleTxt = Folder + "OnEnemyDouble.txt";
            OnEnemyTripleTxt = Folder + "OnEnemyTriple.txt";
            OnEnemyQuadraTxt = Folder + "OnEnemyQuadra.txt";
            OnEnemyPentaTxt = Folder + "OnEnemyPenta.txt";

            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
            if (!File.Exists(OnGameStartTxt))
            {
                var newfile = File.Create(OnGameStartTxt);
                newfile.Close();
                var content = "glhf\ngl hf\nhi";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnGameStartTxt, lines);
            }
            if (!File.Exists(OnDeathTxt))
            {
                var newfile = File.Create(OnDeathTxt);
                newfile.Close();
                var content = "oups\nwot\nwtf\ndat dmg\nthat damage\nfug\nfugg :DDD\nfml\ndamn I suck\nsorry";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnDeathTxt, lines);
            }
            if (!File.Exists(OnGameEndTxt))
            {
                var newfile = File.Create(OnGameEndTxt);
                newfile.Close();
                var content = "oups\nwot\nwtf\ndat dmg\nthat damage\nfug\nfugg :DDD\nfml\ndamn I suck\nsorry";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnGameEndTxt, lines);
            }
            if (!File.Exists(OnKillTxt))
            {
                var newfile = File.Create(OnKillTxt);
                newfile.Close();
                var content = "yay\nfk yeah\nowned\npwned\nkicked dat booty\nowned that ass\npwned that noob";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnKillTxt, lines);
            }
            if (!File.Exists(OnDoubleTxt))
            {
                var newfile = File.Create(OnDoubleTxt);
                newfile.Close();
                var content = "yay\nfk yeah\nowned\npwned\nright in the balls";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnDoubleTxt, lines);
            }
            if (!File.Exists(OnTripleTxt))
            {
                var newfile = File.Create(OnTripleTxt);
                newfile.Close();
                var content = "oooh baby a triple!\ni = god";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnTripleTxt, lines);
            }
            if (!File.Exists(OnQuadraTxt))
            {
                var newfile = File.Create(OnQuadraTxt);
                newfile.Close();
                var content = "godlike\nhaha\nowned\nxD";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnQuadraTxt, lines);
            }
            if (!File.Exists(OnPentaTxt))
            {
                var newfile = File.Create(OnPentaTxt);
                newfile.Close();
                var content = "PENTAKIRRU\nNULLED\nMe > Ur Whole Team\nLel REKT\nSo REKT\nREKT n ROLL baby\nomg I did it mom\nokay";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(OnPentaTxt, lines);
            }
            if (!File.Exists(OnAllyKillTxt))
            {
                var newfile = File.Create(OnAllyKillTxt);
                newfile.Close();
            }
            if (!File.Exists(OnAllyDoubleTxt))
            {
                var newfile = File.Create(OnAllyDoubleTxt);
                newfile.Close();
            }
            if (!File.Exists(OnAllyTripleTxt))
            {
                var newfile = File.Create(OnAllyTripleTxt);
                newfile.Close();
            }
            if (!File.Exists(OnAllyQuadraTxt))
            {
                var newfile = File.Create(OnAllyQuadraTxt);
                newfile.Close();
            }
            if (!File.Exists(OnAllyPentaTxt))
            {
                var newfile = File.Create(OnAllyPentaTxt);
                newfile.Close();
            }
            if (!File.Exists(OnEnemyKillTxt))
            {
                var newfile = File.Create(OnEnemyKillTxt);
                newfile.Close();
            }
            if (!File.Exists(OnEnemyDoubleTxt))
            {
                var newfile = File.Create(OnEnemyDoubleTxt);
                newfile.Close();
            }
            if (!File.Exists(OnEnemyTripleTxt))
            {
                var newfile = File.Create(OnEnemyTripleTxt);
                newfile.Close();
            }
            if (!File.Exists(OnEnemyQuadraTxt))
            {
                var newfile = File.Create(OnEnemyQuadraTxt);
                newfile.Close();
            }
            if (!File.Exists(OnEnemyPentaTxt))
            {
                var newfile = File.Create(OnEnemyPentaTxt);
                newfile.Close();
            }
        }
    }
}
