using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xSLx_Activator
{
	class SummonerManager
	{
		private Menu _menuSummoners;

		public SummonerManager(Menu menuSummoners)
		{
			_menuSummoners = menuSummoners;
			_menuSummoners.AddItem(new MenuItem("xSLxActivator_summonerManager_sep0", "Will be Added after PreSeasonPatch"));
		}
	}
}
