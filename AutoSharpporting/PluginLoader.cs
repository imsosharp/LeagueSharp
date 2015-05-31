using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Support.Plugins;

namespace Support
{
    public class PluginLoader
    {
        private static bool loaded;
        public PluginLoader()
        {
            if (!loaded)
            {
                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "aatrox":
                        new Aatrox();
                        loaded = true;
                        break;
                    case "ahri":
                        new Ahri();
                        loaded = true;
                        break;
                    case "akali":
                        new Akali();
                        loaded = true;
                        break;
                    case "alistar":
                        new Alistar();
                        loaded = true;
                        break;
                    case "amumu":
                        new Amumu();
                        loaded = true;
                        break;
                    case "anivia":
                        new Anivia();
                        loaded = true;
                        break;
                    case "annie":
                        new Annie();
                        loaded = true;
                        break;
                    case "ashe":
                        new Ashe();
                        loaded = true;
                        break;
                    case "blitzcrank":
                        new Blitzcrank();
                        loaded = true;
                        break;
                    case "brand":
                        new Brand();
                        loaded = true;
                        break;
                    case "braum":
                        new Braum();
                        loaded = true;
                        break;
                    case "caitlyn":
                        new Caitlyn();
                        loaded = true;
                        break;
                    case "cassiopeia":
                        new Cassiopeia();
                        loaded = true;
                        break;
                    case "chogath":
                        new Chogath();
                        loaded = true;
                        break;
                    case "corki":
                        new Corki();
                        loaded = true;
                        break;
                    case "darius":
                        new Darius();
                        loaded = true;
                        break;
                    case "diana":
                        new Diana();
                        loaded = true;
                        break;
                    case "draven":
                        new Draven();
                        loaded = true;
                        break;
                    case "evelynn":
                        new Evelynn();
                        loaded = true;
                        break;
                    case "ezreal":
                        new Ezreal();
                        loaded = true;
                        break;
                    case "fiddlesticks":
                        new FiddleSticks();
                        loaded = true;
                        break;
                    case "fiora":
                        new Fiora();
                        loaded = true;
                        break;
                    case "fizz":
                        new Fizz();
                        loaded = true;
                        break;
                    case "galio":
                        new Galio();
                        loaded = true;
                        break;
                    case "gangplank":
                        new Gangplank();
                        loaded = true;
                        break;
                    case "garen":
                        new Garen();
                        loaded = true;
                        break;
                    case "gragas":
                        new Gragas();
                        loaded = true;
                        break;
                    case "graves":
                        new Graves();
                        loaded = true;
                        break;
                    case "heimerdinger":
                        new Heimerdinger();
                        loaded = true;
                        break;
                    case "irelia":
                        new Irelia();
                        loaded = true;
                        break;
                    case "karthus":
                        new Karthus();
                        loaded = true;
                        break;
                    case "kayle":
                        new Kayle();
                        loaded = true;
                        break;
                    case "leblanc":
                        new Leblanc();
                        loaded = true;
                        break;
                    case "lulu":
                        new Lulu();
                        loaded = true;
                        break;
                    case "malzahar":
                        new Malzahar();
                        loaded = true;
                        break;
                    case "morgana":
                        new Morgana();
                        loaded = true;
                        break;
                    case "nami":
                        new Nami();
                        loaded = true;
                        break;
                    case "nunu":
                        new Nunu();
                        loaded = true;
                        break;
                    case "poppy":
                        new Poppy();
                        loaded = true;
                        break;
                    case "riven":
                        new Riven();
                        loaded = true;
                        break;
                    case "shaco":
                        new Shaco();
                        loaded = true;
                        break;
                    case "sivir":
                        new Sivir();
                        loaded = true;
                        break;
                    case "skarner":
                        new Skarner();
                        loaded = true;
                        break;
                    case "sona":
                        new Sona();
                        loaded = true;
                        break;
                    case "soraka":
                        new Soraka();
                        loaded = true;
                        break;
                    case "taric":
                        new Taric();
                        loaded = true;
                        break;
                    case "teemo":
                        new Teemo();
                        loaded = true;
                        break;
                    case "thresh":
                        new Thresh();
                        loaded = true;
                        break;
                    case "tristana":
                        new Tristana();
                        loaded = true;
                        break;
                    case "vayne":
                        new Vayne();
                        loaded = true;
                        break;
                    case "veigar":
                        new Veigar();
                        loaded = true;
                        break;
                    case "zilean":
                        new Zilean();
                        loaded = true;
                        break;
                    case "zyra":
                        new Zyra();
                        loaded = true;
                        break;
                    default:
                        new Default();
                        loaded = true;
                        break;
                }
            }
        }

    }
}
