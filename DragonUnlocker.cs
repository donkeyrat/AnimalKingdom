using UnityEngine;
using Landfall.TABS.Workshop;
using Landfall.TABS.GameMode;

namespace AnimalKingdom
{
    public class DragonUnlocker : MonoBehaviour
    {
        public void Unlock()
        {
            if (CampaignPlayerDataHolder.CurrentGameModeState == GameModeState.Campaign && !ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret(unlockKey))
            {
                ServiceLocator.GetService<ISaveLoaderService>().UnlockSecret(unlockKey);
            }
        }

        public string unlockKey;
    }
}
