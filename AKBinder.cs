using System.Collections;
using UnityEngine;

namespace AnimalKingdom
{
    public class AKBinder : MonoBehaviour
    {

        public static void UnitGlad()
        {
            if (!instance)
            {
                instance = new GameObject
                {
                    hideFlags = HideFlags.HideAndDontSave
                }.AddComponent<AKBinder>();
            }
            instance.StartCoroutine(StartUnitgradLate());
        }

        private static IEnumerator StartUnitgradLate()
        {
            yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);
            new AKMain();
            yield break;
        }

        private static AKBinder instance;
    }
}