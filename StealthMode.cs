using UnityEngine;
using Landfall.TABS;
using Landfall.TABS.AI.Components;
using Unity.Entities;

namespace AnimalKingdom
{
    public class StealthMode : MonoBehaviour
    {
        public void Start()
        {
            if (stealthOnStart)
            {
                EnterStealth();
            }
        }

        public void EnterStealth()
        {
            transform.root.GetComponent<GameObjectEntity>().EntityManager.AddComponentData<IsDead>(transform.root.GetComponent<GameObjectEntity>().Entity, default);
        }

        public void LeaveStealth()
        {
            if (!transform.root.GetComponent<Unit>().data.Dead)
            {
                transform.root.GetComponent<GameObjectEntity>().EntityManager.RemoveComponent<IsDead>(transform.root.GetComponent<GameObjectEntity>().Entity);
            }
        }

        public bool stealthOnStart = true;
    }
}
