using Landfall.TABS;
using UnityEngine;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnimalKingdom
{
    public class AKMain
    {
        public AKMain()
        {
			var db = LandfallUnitDatabase.GetDatabase();
			List<Faction> factions = (List<Faction>)typeof(LandfallUnitDatabase).GetField("Factions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
			foreach (var fac in kermate.LoadAllAssets<Faction>())
			{
				var factionUnits = new List<UnitBlueprint>(fac.Units);
				var newFactionUnits = (
					from UnitBlueprint unit
					in factionUnits
					orderby unit.GetUnitCost()
					select unit).ToList();
				fac.Units = newFactionUnits.ToArray();
				foreach (var vFac in factions)
				{
					if (fac.Entity.Name == vFac.Entity.Name + "_NEW")
					{
						var vFacUnits = new List<UnitBlueprint>(vFac.Units);
						vFacUnits.AddRange(fac.Units);
						var newUnits = (
							from UnitBlueprint unit
							in vFacUnits
							orderby unit.GetUnitCost()
							select unit).ToList();
						vFac.Units = newUnits.ToArray();
						Object.DestroyImmediate(fac);
					}
				}
			}
			foreach (var sb in kermate.LoadAllAssets<SoundBank>())
			{
				if (sb.name.Contains("Sound"))
				{
					var vsb = ServiceLocator.GetService<SoundPlayer>().soundBank;
					var cat = vsb.Categories.ToList();
					cat.AddRange(sb.Categories);
					vsb.Categories = cat.ToArray();
				}
			}
			foreach (var fac in kermate.LoadAllAssets<Faction>())
			{
				db.AddFactionWithID(fac);
				foreach (var unit in fac.Units)
				{
					if (!db.UnitList.Contains(unit))
					{
						db.AddUnitWithID(unit);
					}
				}
			}
			foreach (var unit in kermate.LoadAllAssets<UnitBlueprint>())
			{
				if (!db.UnitList.Contains(unit))
				{
					db.AddUnitWithID(unit);
				}
				foreach (var b in db.UnitBaseList)
				{
					if (unit.UnitBase != null)
					{
						if (b.name == unit.UnitBase.name)
						{
							unit.UnitBase = b;
						}
					}
				}
				foreach (var b in db.WeaponList)
				{
					if (unit.RightWeapon != null && b.name == unit.RightWeapon.name) unit.RightWeapon = b;
					if (unit.LeftWeapon != null && b.name == unit.LeftWeapon.name) unit.LeftWeapon = b;
				}
			}
			foreach (var objecting in kermate.LoadAllAssets<GameObject>())
			{
				if (objecting != null)
				{
					if (objecting.GetComponent<Unit>())
					{
						List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("UnitBases", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
						stuff.Add(objecting);
						typeof(LandfallUnitDatabase).GetField("UnitBases", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
					}
					else if (objecting.GetComponent<WeaponItem>())
					{
						List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("Weapons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
						stuff.Add(objecting);
						typeof(LandfallUnitDatabase).GetField("Weapons", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
					}
					else if (objecting.GetComponent<ProjectileEntity>())
					{
						List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("Projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
						stuff.Add(objecting);
						typeof(LandfallUnitDatabase).GetField("Projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
					}
					else if (objecting.GetComponent<SpecialAbility>())
					{
						List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("CombatMoves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
						stuff.Add(objecting);
						typeof(LandfallUnitDatabase).GetField("CombatMoves", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
					}
					else if (objecting.GetComponent<PropItem>())
					{
						List<GameObject> stuff = (List<GameObject>)typeof(LandfallUnitDatabase).GetField("CharacterProps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
						stuff.Add(objecting);
						typeof(LandfallUnitDatabase).GetField("CharacterProps", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, stuff);
					}
				}
			}
			new AKSecretManager();
			ServiceLocator.GetService<CustomContentLoaderModIO>().QuickRefresh(WorkshopContentType.Unit, null);
        }

		public static AssetBundle kermate = AssetBundle.LoadFromMemory(Properties.Resources.animalkingdom);
	}
}
