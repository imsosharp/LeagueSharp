using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace xSLx_Activator
{
	class PotionManager
	{
		public static Obj_AI_Hero MyHero = ObjectManager.Player;
		private class Potion
		{
			public string Name
			{
				get;
				set;
			}
			public int MinCharges
			{
				get;
				set;
			}
			public ItemId ItemId
			{
				get;
				set;
			}
			public List<PotionType> TypeList
			{
				get;
				set;
			}
		}

		private enum PotionType
		{
			Health,
			Mana
		};

		private static List<Potion> _potions;

		private readonly Menu _menuPotions;

		public PotionManager(Menu menuPotions)
		{
			_menuPotions = menuPotions;
			CreateMenu();
		}

		private void CreateMenu()
		{
			_potions = new List<Potion>
				{
					new Potion
					{
						Name = "ItemCrystalFlask",
						MinCharges = 1,
						ItemId = (ItemId) 2041,
						TypeList = new List<PotionType> {PotionType.Health, PotionType.Mana}
					},
					new Potion
					{
						Name = "RegenerationPotion",
						MinCharges = 0,
						ItemId = (ItemId) 2003,
						TypeList = new List<PotionType> {PotionType.Health}
					},
					new Potion
					{
						Name = "ItemMiniRegenPotion",
						MinCharges = 0,
						ItemId = (ItemId) 2010,
						TypeList = new List<PotionType> {PotionType.Health}
					},
					new Potion
					{
						Name = "FlaskOfCrystalWater",
						MinCharges = 0,
						ItemId = (ItemId) 2004,
						TypeList = new List<PotionType> {PotionType.Mana}
					}
				};

			_menuPotions.AddItem(new MenuItem("xSLxActivator_potionmanager_sep0", "====== Health"));
			_menuPotions.AddItem(new MenuItem("xSLxActivator_potionmanager_HealthPotion", "= Use Health Potion").SetValue(true));
			_menuPotions.AddItem(new MenuItem("xSLxActivator_potionmanager_HealthPercent", "= HP Trigger Percent").SetValue(new Slider(30)));
			_menuPotions.AddItem(new MenuItem("xSLxActivator_potionmanager_sep1", "====== Mana"));
			_menuPotions.AddItem(new MenuItem("xSLxActivator_potionmanager_ManaPotion", "Use Mana Potion").SetValue(true));
			_menuPotions.AddItem(new MenuItem("xSLxActivator_potionmanager_ManaPercent", "MP Trigger Percent").SetValue(new Slider(30)));

			Game.OnGameUpdate += OnGameUpdate;
		}

		private void OnGameUpdate(EventArgs args)
		{
			try
			{
				if(MyHero.HasBuff("Recall") || Utility.InShopRange() || Utility.InFountain())
					return;
				if(_menuPotions.Item("xSLxActivator_potionmanager_HealthPotion").GetValue<bool>())
				{
					if(GetPlayerHealthPercentage() <= _menuPotions.Item("xSLxActivator_potionmanager_HealthPercent").GetValue<Slider>().Value)
					{
						var healthSlot = GetPotionSlot(PotionType.Health);
						if(!IsBuffActive(PotionType.Health))
							healthSlot.UseItem();
					}
				}
				if(_menuPotions.Item("xSLxActivator_potionmanager_ManaPotion").GetValue<bool>())
				{
					if(GetPlayerManaPercentage() <= _menuPotions.Item("xSLxActivator_potionmanager_ManaPercent").GetValue<Slider>().Value)
					{
						var manaSlot = GetPotionSlot(PotionType.Mana);
						if(!IsBuffActive(PotionType.Mana))
							manaSlot.UseItem();
					}
				}
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch(Exception)
			{

			}
		}

		private static InventorySlot GetPotionSlot(PotionType type)
		{
			return (from potion in _potions
					where potion.TypeList.Contains(type)
					from item in MyHero.InventoryItems
					where item.Id == potion.ItemId && item.Charges >= potion.MinCharges
					select item).FirstOrDefault();
		}

		private static bool IsBuffActive(PotionType type)
		{
			return (from potion in _potions
					where potion.TypeList.Contains(type)
					from buff in MyHero.Buffs
					where buff.Name == potion.Name && buff.IsActive
					select potion).Any();
		}
		internal static float GetPlayerHealthPercentage()
		{
			return MyHero.Health * 100 / MyHero.MaxHealth;
		}

		internal static float GetPlayerManaPercentage()
		{
			return MyHero.Mana * 100 / MyHero.MaxMana;
		}
	}
}
