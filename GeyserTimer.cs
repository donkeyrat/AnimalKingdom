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
            DoEruption();
        }

        public void DoEruption(bool isSecondEruption = false)
        {
            availableEvents = events.Where(x => !isErupting[events.IndexOf(x)]).ToArray();
            var index = Random.Range(0, availableEvents.Length - 1);
            if (availableEvents.Length > 0)
            {
                var chosenEvent = availableEvents[index];
                chosenEvent.Go();
                isErupting[index] = true;
                if (!isSecondEruption && Random.value < doMultipleEruptionsChance)
                {
                    DoEruption(true);
                }
            }
        }

        private DelayEvent[] availableEvents;
        
        public List<DelayEvent> events = new List<DelayEvent>();

        public List<bool> isErupting = new List<bool>();

        public float doMultipleEruptionsChance = 0.2f;
    }
}