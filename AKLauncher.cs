using BepInEx;

namespace AnimalKingdom
{
	[BepInPlugin("teamgrad.animalkingdom", "Animal Kingdom", "0.3.0")]
	public class AKLauncher : BaseUnityPlugin
	{
		public AKLauncher()
		{
			AKBinder.UnitGlad();
		}
	}
}
