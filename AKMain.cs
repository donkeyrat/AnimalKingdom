using Landfall.TABS;
using UnityEngine;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using DM;

namespace AnimalKingdom
{
    public class AKMain
    {
        public AKMain()
        {
	        var db = ContentDatabase.Instance();
	        AssetBundle.LoadFromMemory(Properties.Resources.calderaceum);
	        AssetBundle.LoadFromMemory(Properties.Resources.weepingcopse);
	        
	        var newMapList = ((MapAsset[])typeof(LandfallContentDatabase).GetField("m_orderedMapAssets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db.LandfallContentDatabase)).ToList();
	        var newMapDict = (Dictionary<DatabaseID, int>)typeof(LandfallContentDatabase).GetField("m_mapAssetIndexLookup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db.LandfallContentDatabase);

	        foreach (var map in kermate.LoadAllAssets<MapAsset>()) 
	        {
		        newMapList.Add(map);
		        newMapDict.Add(map.Entity.GUID, newMapList.IndexOf(map));
	        }

	        typeof(LandfallContentDatabase).GetField("m_orderedMapAssets", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db.LandfallContentDatabase, newMapList.ToArray());
	        typeof(LandfallContentDatabase).GetField("m_mapAssetIndexLookup", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db.LandfallContentDatabase, newMapDict);
	        
	        var factions = db.LandfallContentDatabase.GetFactions().ToList();
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
				if (sb.name.Contains("Sound")) {
					var vsb = ServiceLocator.GetService<SoundPlayer>().soundBank;
					foreach (var sound in sb.Categories) { sound.categoryMixerGroup = vsb.Categories[0].categoryMixerGroup; }
					var cat = vsb.Categories.ToList();
					cat.AddRange(sb.Categories);
					vsb.Categories = cat.ToArray();
				}
				if (sb.name.Contains("Music")) {
					var vsb = ServiceLocator.GetService<MusicHandler>().bank;
					var cat = vsb.Categories.ToList();
					cat.AddRange(sb.Categories);
					foreach (var categ in sb.Categories) {
						foreach (var sound in categ.soundEffects) {
							var song = new SongInstance();
							song.clip = sound.clipTypes[0].clips[0];
							song.soundEffectInstance = sound;
							song.songRef = categ.categoryName + "/" + sound.soundRef;
							ServiceLocator.GetService<MusicHandler>().m_songs.Add(song.songRef, song);
						}
					}
					vsb.Categories = cat.ToArray();
				}
			}
			
			new AKSceneManager();

			new Harmony("DAMNTHEYREALLYCALLEDTHISTHINGFERNASS").PatchAll();
	        
	        foreach (var fac in kermate.LoadAllAssets<Faction>())
			{
				newFactions.Add(fac);
			}
	        
			foreach (var unit in kermate.LoadAllAssets<UnitBlueprint>())
			{
				newUnits.Add(unit);
				foreach (var b in db.LandfallContentDatabase.GetUnitBases().ToList()) { if (unit.UnitBase != null) { if (b.name == unit.UnitBase.name) { unit.UnitBase = b; } } }
				foreach (var b in db.LandfallContentDatabase.GetWeapons().ToList()) { if (unit.RightWeapon != null && b.name == unit.RightWeapon.name) unit.RightWeapon = b; if (unit.LeftWeapon != null && b.name == unit.LeftWeapon.name) unit.LeftWeapon = b; }
			}
			
			int startID = 37105;
			foreach (var sprite in kermate.LoadAllAssets<Sprite>()) {

				if (sprite.name.Contains("Icons_128x128")) {

					var icon = Object.Instantiate(db.GetFactionIcon(db.LandfallContentDatabase.GetFactionIconIds().ToList()[0]));
					icon.name = sprite.name;
					icon.Entity.SetSpriteIcon(sprite);
					icon.Entity.GUID = new DatabaseID(-2, startID);
					startID++;
					newFactionIcons.Add(icon);
				}
			}
			
			foreach (var objecting in kermate.LoadAllAssets<GameObject>())
            {
                if (objecting != null) {

	                if (objecting.GetComponent<Unit>())
	                {
		                if (!objecting.GetComponent<Outline>()) objecting.AddComponent<Outline>().OutlineWidth = 1f;
		                newBases.Add(objecting);
	                }
                    else if (objecting.GetComponent<WeaponItem>()) {
                        newWeapons.Add(objecting);
                        int totalSubmeshes = 0;
                        foreach (var rend in objecting.GetComponentsInChildren<MeshFilter>()) {
                            if (rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled == true) {

                                totalSubmeshes += rend.mesh.subMeshCount;
                            }
                        }
                        foreach (var rend in objecting.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                            if (rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled) {

                                totalSubmeshes += rend.sharedMesh.subMeshCount;
                            }
                        }
                        if (totalSubmeshes != 0) {
                            float average = 1f / totalSubmeshes;
                            var averageList = new List<float>();
                            for (int i = 0; i < totalSubmeshes; i++) { averageList.Add(average); }
                            objecting.GetComponent<WeaponItem>().SubmeshArea = null;
                            objecting.GetComponent<WeaponItem>().SubmeshArea = averageList.ToArray();
                        }
                    }
                    else if (objecting.GetComponent<ProjectileEntity>()) newProjectiles.Add(objecting);
                    else if (objecting.GetComponent<SpecialAbility>()) newAbilities.Add(objecting);
                    else if (objecting.GetComponent<PropItem>()) {
                        newProps.Add(objecting);
                        int totalSubmeshes = 0;
                        foreach (var rend in objecting.GetComponentsInChildren<MeshFilter>()) {
                            if (rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled == true) {

                                totalSubmeshes += rend.mesh.subMeshCount;
                            }
                        }
                        foreach (var rend in objecting.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                            if (rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled) {

                                totalSubmeshes += rend.sharedMesh.subMeshCount;
                            }
                        }
                        if (totalSubmeshes != 0) {
                            float average = 1f / totalSubmeshes;
                            var averageList = new List<float>();
                            for (int i = 0; i < totalSubmeshes; i++) { averageList.Add(average); }
                            objecting.GetComponent<PropItem>().SubmeshArea = null;
                            objecting.GetComponent<PropItem>().SubmeshArea = averageList.ToArray();
                        }
                    }
                }
            }
			
			AddContentToDatabase();
        }
        
        public void AddContentToDatabase()
        {
	        Dictionary<DatabaseID, UnityEngine.Object> nonStreamableAssets = (Dictionary<DatabaseID, UnityEngine.Object>)typeof(AssetLoader).GetField("m_nonStreamableAssets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ContentDatabase.Instance().AssetLoader);
	        
            var db = ContentDatabase.Instance().LandfallContentDatabase;
            
            Dictionary<DatabaseID, UnitBlueprint> units = (Dictionary<DatabaseID, UnitBlueprint>)typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var unit in newUnits)
            {
	            if (!units.ContainsKey(unit.Entity.GUID))
	            {
		            units.Add(unit.Entity.GUID, unit);
		            nonStreamableAssets.Add(unit.Entity.GUID, unit);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, units);
            
            Dictionary<DatabaseID, Faction> factions = (Dictionary<DatabaseID, Faction>)typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            List<DatabaseID> defaultHotbarFactions = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var faction in newFactions)
            {
	            if (!factions.ContainsKey(faction.Entity.GUID))
	            {
		            factions.Add(faction.Entity.GUID, faction);
		            nonStreamableAssets.Add(faction.Entity.GUID, faction);
		            defaultHotbarFactions.Add(faction.Entity.GUID);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, factions);
            typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, defaultHotbarFactions.OrderBy(x => factions[x].index).ToList());

            Dictionary<DatabaseID, TABSCampaignAsset> campaigns = (Dictionary<DatabaseID, TABSCampaignAsset>)typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var campaign in newCampaigns)
            {
	            if (!campaigns.ContainsKey(campaign.Entity.GUID))
	            {
		            campaigns.Add(campaign.Entity.GUID, campaign);
		            nonStreamableAssets.Add(campaign.Entity.GUID, campaign);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, campaigns);
            
            Dictionary<DatabaseID, TABSCampaignLevelAsset> campaignLevels = (Dictionary<DatabaseID, TABSCampaignLevelAsset>)typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var campaignLevel in newCampaignLevels)
            {
	            if (!campaignLevels.ContainsKey(campaignLevel.Entity.GUID))
	            {
		            campaignLevels.Add(campaignLevel.Entity.GUID, campaignLevel);
		            nonStreamableAssets.Add(campaignLevel.Entity.GUID, campaignLevel);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, campaignLevels);
            
            Dictionary<DatabaseID, VoiceBundle> voiceBundles = (Dictionary<DatabaseID, VoiceBundle>)typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var voiceBundle in newVoiceBundles)
            {
	            if (!voiceBundles.ContainsKey(voiceBundle.Entity.GUID))
	            {
		            voiceBundles.Add(voiceBundle.Entity.GUID, voiceBundle);
		            nonStreamableAssets.Add(voiceBundle.Entity.GUID, voiceBundle);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, voiceBundles);
            
            List<DatabaseID> factionIcons = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var factionIcon in newFactionIcons)
            {
	            if (!factionIcons.Contains(factionIcon.Entity.GUID))
	            {
		            factionIcons.Add(factionIcon.Entity.GUID);
		            nonStreamableAssets.Add(factionIcon.Entity.GUID, factionIcon);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, factionIcons);
            
            Dictionary<DatabaseID, GameObject> unitBases = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var unitBase in newBases)
            {
	            if (!unitBases.ContainsKey(unitBase.GetComponent<Unit>().Entity.GUID))
	            {
		            unitBases.Add(unitBase.GetComponent<Unit>().Entity.GUID, unitBase);
		            nonStreamableAssets.Add(unitBase.GetComponent<Unit>().Entity.GUID, unitBase);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, unitBases);
            
            Dictionary<DatabaseID, GameObject> props = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var prop in newProps)
            {
	            if (!props.ContainsKey(prop.GetComponent<PropItem>().Entity.GUID))
	            {
		            props.Add(prop.GetComponent<PropItem>().Entity.GUID, prop);
		            nonStreamableAssets.Add(prop.GetComponent<PropItem>().Entity.GUID, prop);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, props);
            
            Dictionary<DatabaseID, GameObject> abilities = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var ability in newAbilities)
            {
	            if (!abilities.ContainsKey(ability.GetComponent<SpecialAbility>().Entity.GUID))
	            {
		            abilities.Add(ability.GetComponent<SpecialAbility>().Entity.GUID, ability);
		            nonStreamableAssets.Add(ability.GetComponent<SpecialAbility>().Entity.GUID, ability);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, abilities);
            
            Dictionary<DatabaseID, GameObject> weapons = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var weapon in newWeapons)
            {
	            if (!weapons.ContainsKey(weapon.GetComponent<WeaponItem>().Entity.GUID))
	            {
		            weapons.Add(weapon.GetComponent<WeaponItem>().Entity.GUID, weapon);
		            nonStreamableAssets.Add(weapon.GetComponent<WeaponItem>().Entity.GUID, weapon);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, weapons);
            
            Dictionary<DatabaseID, GameObject> projectiles = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var proj in newProjectiles)
            {
	            if (!projectiles.ContainsKey(proj.GetComponent<ProjectileEntity>().Entity.GUID))
	            {
		            projectiles.Add(proj.GetComponent<ProjectileEntity>().Entity.GUID, proj);
		            nonStreamableAssets.Add(proj.GetComponent<ProjectileEntity>().Entity.GUID, proj);
	            }
            }
            typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, projectiles);



            ServiceLocator.GetService<CustomContentLoaderModIO>().QuickRefresh(WorkshopContentType.Unit, null);
        }
        
        public List<UnitBlueprint> newUnits = new List<UnitBlueprint>();

        public List<Faction> newFactions = new List<Faction>();
        
        public List<TABSCampaignAsset> newCampaigns = new List<TABSCampaignAsset>();
        
        public List<TABSCampaignLevelAsset> newCampaignLevels = new List<TABSCampaignLevelAsset>();
        
        public List<VoiceBundle> newVoiceBundles = new List<VoiceBundle>();
        
        public List<FactionIcon> newFactionIcons = new List<FactionIcon>();
        
        public List<GameObject> newBases = new List<GameObject>();
        
        public List<GameObject> newProps = new List<GameObject>();
        
        public List<GameObject> newAbilities = new List<GameObject>();

        public List<GameObject> newWeapons = new List<GameObject>();
        
        public List<GameObject> newProjectiles = new List<GameObject>();

        public static AssetBundle kermate = AssetBundle.LoadFromMemory(Properties.Resources.animalkingdom);
	}
}
