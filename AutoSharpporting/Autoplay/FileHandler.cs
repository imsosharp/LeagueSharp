using System;
using System.Linq;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using Support.Util;

namespace Support
{
    internal class FileHandler
    {
        private static string _cBuildsPath = Config.LeagueSharpDirectory + @"\AutoSharpporting\";
        public static string _theFile;
        public static ItemId[] CustomShopList;

        public static void DoChecks()
        {
            _theFile = _cBuildsPath + Utility.Map.GetMap().Type + @"\" + Autoplay.Bot.BaseSkinName + ".txt";
            if (!Directory.Exists(_cBuildsPath))
            {
                Directory.CreateDirectory(_cBuildsPath);
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.CrystalScar);
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.HowlingAbyss);
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.SummonersRift);
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.TwistedTreeline);
                Directory.CreateDirectory(_cBuildsPath + Utility.Map.MapType.Unknown);
            }
            if (File.Exists(_theFile))
            {
                Helpers.PrintMessage("Loaded: " + _theFile);
                string[] itemsStringArray = File.ReadAllLines(_theFile);
                int[] itemsIntArray = new int[itemsStringArray.Count()];
                CustomShopList = new ItemId[itemsStringArray.Count()];
                for(var i = 0; i < itemsStringArray.Count(); i++)
                {
                    itemsIntArray[i] = Convert.ToInt32(itemsStringArray[i]);
                }
                for (var i = 0; i < itemsIntArray.Count(); i++)
                {
                    CustomShopList[i] = (ItemId)itemsIntArray[i];
                }
            }
            if (!File.Exists(_theFile) && Utility.Map.GetMap().Type == Utility.Map.MapType.SummonersRift)
            {
                var newfile = File.Create(_theFile);
                newfile.Close();
                var content = "3157\n3089\n3165\n3174\n3116\n3222\n3092\n3151\n3100\n3190\n3027\n3135\n3146\n3020";
                var separator = new string[] { "\n" };
                string[] lines = content.Split(separator, StringSplitOptions.None);
                File.WriteAllLines(_theFile, lines);
                Helpers.PrintMessage("Created custom shoplist at: " + _theFile);
            }
        }
    }
}
