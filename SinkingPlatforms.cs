using UnityEngine;
using Landfall.TABS;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS.GameState;

namespace AnimalKingdom
{
    public class SinkingPlatforms : GameStateListener
    {
        public override void OnEnterPlacementState()
        {
            transform.gameObject.layer = originalMask;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = originalMask;
            }
        }
        
        public override void OnEnterBattleState()
        {
            transform.gameObject.layer = newMask;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = newMask;
            }
        }
        
        public int originalMask;

        public int newMask;
    }
}