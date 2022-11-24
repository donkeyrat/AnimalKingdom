using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Landfall.TABS;

namespace AnimalKingdom
{
    public class PlaguePhases : MonoBehaviour
    {
        private void Start()
        {
            unit = transform.root.GetComponent<Unit>();
            unit.data.healthHandler.willBeRewived = false;
        }

        public void BeginTransition()
        {
            StartCoroutine(DoSickening());
        }

        private IEnumerator DoSickening()
        {
            StartCoroutine(PlayPartWithDelay(sickenTime - 0.5f));
            
            var storedAliveMaterials = new List<Material>();
            foreach (var index in aliveMaterialIndexes)
            {
                storedAliveMaterials.Add(Instantiate(renderer.materials[index]));
            }
            
            var t = 0f;
            while (t < sickenTime && !unit.data.Dead)
            {
                for (int i = 0; i < aliveMaterialIndexes.Count; i++)
                {
                    renderer.materials[aliveMaterialIndexes[i]].Lerp(storedAliveMaterials[i], sicklyMaterials[i], t / sickenTime);
                }
                
                t += Time.deltaTime;
                yield return null;
            }

            if (unit.data.Dead)
            {
                part.Stop();
                Destroy(gameObject);
                yield break;
            }

            currentState = PlagueState.Sickly;
            
            SetRenderer();
            if (unit.GetComponentInChildren<EyeSpawner>())
            {
                foreach (var eyeSet in unit.GetComponentsInChildren<EyeSpawner>())
                {
                    foreach (var eye in eyeSet.spawnedEyes) Destroy(eye.gameObject);
                    eyeSet.spawnedEyes.Clear();
                    
                    eyeSet.eyeObject = reviveEye;
                    eyeSet.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(eyeSet, new object[] { });
                }
            }

            StartCoroutine(PlayPartWithDelay(0.5f));
        }

        public IEnumerator PlayPartWithDelay(float delay = 0f)
        {
            yield return new WaitForSeconds(delay);
            part.Play();
        }
        
        public void SetRenderer()
        {
            if (currentState == PlagueState.Alive)
            {
                renderer.gameObject.SetActive(true);
                sickRenderer.gameObject.SetActive(false);
                zombieRenderer.gameObject.SetActive(false);
            }
            else if (currentState == PlagueState.Sickly)
            {
                renderer.gameObject.SetActive(false);
                sickRenderer.gameObject.SetActive(true);
                zombieRenderer.gameObject.SetActive(false);
            }
            else if (currentState == PlagueState.Zombie)
            {
                renderer.gameObject.SetActive(false);
                sickRenderer.gameObject.SetActive(false);
                zombieRenderer.gameObject.SetActive(true);
            }
        }
        
        private Unit unit;
        
        public enum PlagueState
        {
            Alive,
            Sickly,
            Zombie
        }
        public PlagueState currentState;
        
        public Renderer renderer;
        public Renderer sickRenderer;
        public Renderer zombieRenderer;

        public ParticleSystem part;

        public List<Material> sicklyMaterials = new List<Material>();
        public List<int> aliveMaterialIndexes = new List<int>();

        public GameObject reviveEye;
        
        public float sickenTime = 6f;
    }
}