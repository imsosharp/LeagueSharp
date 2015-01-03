using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;

namespace Support
{
    internal class FileHandler
    {
        private static string _cBuildsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AutoSharpporting\\"; //y u do dis csharp
        private static string theFile = ""; //better to intialize at ongameload
        private static string[] _itemsStringArray = { };
        public static int[] Items = { };

        public static void DoChecks()
        {
            theFile = _cBuildsPath + Utility.Map.GetMap().Type.ToString() + Autoplay.Bot.BaseSkinName + ".txt";
            if (!Directory.Exists(_cBuildsPath))
            {
                Directory.CreateDirectory(_cBuildsPath);
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.CrystalScar.ToString());
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.HowlingAbyss.ToString());
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.SummonersRift.ToString());
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.TwistedTreeline.ToString());
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.Unknown.ToString());
            }
            if (ExistsCustomBuild())
            {
                Console.WriteLine("Found custom build");
                var contents = File.ReadAllText(theFile);
                string[] separator = { "," };
                _itemsStringArray = contents.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                for(var i = 0; i < _itemsStringArray.Count(); i++)
                {
                    Int32.TryParse(_itemsStringArray[i], out Items[i]);
                }
            }
        }

        public static bool ExistsCustomBuild()
        {
            return (File.Exists(theFile));
        }

        public static ItemId[] IDAtoITEMA()
        {
            ItemId[] localCopy = { };
            for(var i = 0; i < Items.Count(); i++)
            {
                localCopy[i] = (ItemId)Items[i];
            }
            return localCopy;
        }
    }
}
