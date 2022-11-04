using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalKingdom
{
    public class GeyserTimer : MonoBehaviour
    {
        public void Start()
        {
            foreach (var de in events)
            {
                isErupting.Add(false);
            }
        }
        
        public void DoRandomEvent()
        {
            StartCoroutine(DoEruption());
        }

        public IEnumerator DoEruption()
        {
            var index = Random.Range(0, events.Count - 1);
            if (isErupting[index] == true)
            {
                StartCoroutine(DoEruption());
                yield break;
            }
            var chosenEvent = events[index];
            chosenEvent.Go();
            isErupting[index] = true;
            if (Random.value < doMultipleEruptionsChance)
            {
                StartCoroutine(DoSecondEruption());
            }
        }
        
        public IEnumerator DoSecondEruption()
        {
            var index = Random.Range(0, events.Count - 1);
            if (isErupting[index] == true)
            {
                DoRandomEvent();
                yield break;
            }
            var chosenEvent = events[index];
            chosenEvent.Go();
            isErupting[index] = true;
        }

        public List<DelayEvent> events = new List<DelayEvent>();

        public List<bool> isErupting = new List<bool>();

        public float doMultipleEruptionsChance = 0.2f;
    }
}