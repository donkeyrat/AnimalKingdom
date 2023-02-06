using System.Collections;
using BepInEx;
using UnityEngine;

namespace AnimalKingdom
{
	[BepInPlugin("teamgrad.animalkingdom", "Animal Kingdom", "1.0.4")]
	public class AKLauncher : BaseUnityPlugin
	{
		public void Awake()
		{
			//DoConfig();
			StartCoroutine(LaunchMod());
		}
		
		private static IEnumerator LaunchMod() 
		{
			yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);
			
			new AKMain();
		}
	}
}
