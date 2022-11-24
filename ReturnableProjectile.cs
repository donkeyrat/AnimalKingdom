using System.Collections;
using UnityEngine;
using Landfall.TABS;

namespace AnimalKingdom
{
    public class ReturnableProjectile : MonoBehaviour
    {
        private void Start()
        {
            weapon = GetComponent<TeamHolder>().spawnerWeapon;
            returnObject = weapon.transform.FindChildRecursive(objectToReturnTo);
        }
        
        public void Return()
        {
            StartCoroutine(DoReturn());
        }

        public IEnumerator DoReturn()
        {
            if (GetComponent<ProjectileStick>()) Destroy(GetComponent<ProjectileStick>());
            
            var t = 0f;
            var beginPosition = transform.position;
            var beginRotation = transform.rotation;
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(beginPosition,
                    returnObject.position, t);
                transform.rotation = Quaternion.Lerp(beginRotation,
                    returnObject.rotation, t);
                t += Time.deltaTime * speed;
                yield return null;
            }
            weapon.GetComponent<DelayEvent>().Go();
            Destroy(gameObject);
        }

        private Transform returnObject;

        private GameObject weapon;

        public string objectToReturnTo;

        public float speed = 1f;
    }
}