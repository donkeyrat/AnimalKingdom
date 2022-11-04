using UnityEngine;
using Landfall.TABS;

namespace AnimalKingdom
{
    public class ReturnableProjectile : MonoBehaviour
    {
        public void Start()
        {
            weapon = transform.GetComponentInParent<Weapon>() ? transform.GetComponentInParent<Weapon>() : transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon;
            returnObject = weapon.transform.FindChildRecursive(objectToReturnTo);
        }
        
        public void Update()
        {
            if (returning)
            {
                if (counter >= 1f)
                {
                    weapon.GetComponent<DelayEvent>().Go();
                    Destroy(gameObject);
                    returning = false;
                    return;
                }
                transform.position = Vector3.Lerp(beginPosition,
                    returnObject.position, counter);
                transform.rotation = Quaternion.Lerp(beginRotation,
                    returnObject.rotation, counter);
                counter += Time.deltaTime * speed;
            }
        }
        
        public void Return()
        {
            if (GetComponent<ProjectileStick>())
            {
                Destroy(GetComponent<ProjectileStick>());
            }

            beginPosition = transform.position;
            beginRotation = transform.rotation;
            
            returning = true;
            counter = 0f;
        }

        private float counter;
        
        private bool returning;

        private Vector3 beginPosition;

        private Quaternion beginRotation;

        private Transform returnObject;

        private Weapon weapon;

        public string objectToReturnTo;

        public float speed = 1f;
    }
}