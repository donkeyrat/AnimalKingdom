using BepInEx;

namespace AnimalKingdom
{
	[BepInPlugin("teamgrad.animalkingdom", "Animal Kingdom", "1.0.0")]
	public class AKLauncher : BaseUnityPlugin
	{
		public AKLauncher()
		{
			AKBinder.UnitGlad();
		}
	}
}
