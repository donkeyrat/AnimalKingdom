using BepInEx;

namespace AnimalKingdom
{
	[BepInPlugin("teamgrad.animalkingdom", "Animal Kingdom", "0.2.1")]
	public class AKLauncher : BaseUnityPlugin
	{
		public AKLauncher()
		{
			AKBinder.UnitGlad();
		}
	}
}
