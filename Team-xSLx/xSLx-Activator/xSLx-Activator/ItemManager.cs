using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;

namespace xSLx_Activator
{
	class ItemManager
	{
		private List <Item> AllItems;
		private class Item
		{
			public string Name
			{
				get;
				set;
			}
			public ItemId ItemId
			{
				get;
				set;
			}
			public int MapId
			{
				get;
				set;
			}

			public bool IsMap()
			{
				var currentMap = Utility.Map.GetMap()._MapType;
				var activeMaps = (Map)MapId;
				return activeMaps.ToString().Contains(currentMap.ToString());
			}

		}
		[Flags]
		private enum Map
		{
			SummonersRift = 1,
			TwistedTreeline = 2,
			CrystalScar = 4,
			HowlingAbyss = 8,
		};
		private Menu _menuItems;

		public ItemManager(Menu menuItems)
		{
			//LoadItems();
			_menuItems = menuItems;
			_menuItems.AddItem(new MenuItem("xSLxActivator_itemManager_sep0", "Will be Added after PreSeasonPatch"));
			//Game.OnGameUpdate += OnGameUpdate;
		}

		private void LoadItems()
		{
			AllItems = new List<Item>
				{
					new Item
					{
						Name = "Mercurial Scimitar",
						ItemId = (ItemId) 3139,
						MapId = (int)Map.SummonersRift + (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Quicksilver Sash",
						ItemId = (ItemId) 3140,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Dervish Blade",
						ItemId = (ItemId) 3137,
						MapId = (int)Map.SummonersRift + (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
					new Item
					{
						Name = "Mikael's Crucible",
						ItemId = (ItemId) 3222,
						MapId = (int)Map.SummonersRift + (int)Map.TwistedTreeline + (int)Map.CrystalScar+ (int)Map.HowlingAbyss
					},
				};
		}

		private void OnGameUpdate(EventArgs args)
		{
			
		}
	}
}
