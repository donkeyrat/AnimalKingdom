using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerator DoEruption(bool isSecondEruption = false)
        {
            availableEvents = events.Where(x => !isErupting[events.IndexOf(x)]).ToArray();
            var index = Random.Range(0, availableEvents.Length - 1);
            if (availableEvents.Length > 0)
            {
                isErupting[index] = true;
                
                var chosenEvent = availableEvents[index];
                chosenEvent.Go();
                
                if (!isSecondEruption && Random.value < doMultipleEruptionsChance)
                {
                    StartCoroutine(DoEruption(true));
                }
                
                yield return new WaitForSeconds(3f);
                isErupting[index] = false;
            }

            yield break;
        }

        private DelayEvent[] availableEvents;
        
        public List<DelayEvent> events = new List<DelayEvent>();

        public List<bool> isErupting = new List<bool>();

        public float doMultipleEruptionsChance = 0.2f;
    }
}