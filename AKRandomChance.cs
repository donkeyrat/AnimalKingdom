using UnityEngine;
using UnityEngine.Events;

namespace AnimalKingdom
{
    public class AKRandomChance : MonoBehaviour
    {
        public void Start()
        {
            if (onStart) { Randomize(); }
        }

        public void Randomize()
        {
            if (Random.value > randomChance)
            {
                randomEvent.Invoke();
            }
        }

        public UnityEvent randomEvent;

        public float randomChance;

        public bool onStart;
    }
}