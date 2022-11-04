using UnityEngine;

public class PlagueBite : MonoBehaviour
{
	public enum BodyTarget
	{
		Head,
		RightFoot,
		LeftFoot,
		RightHand,
		LeftHand
	}

	public BodyTarget bodyTarget;

	private CollisionWeapon newWeapon;

	private void Start()
	{
		GameObject gameObject = null;
		if (bodyTarget == BodyTarget.Head)
		{
			gameObject = base.transform.root.GetComponentInChildren<Head>().gameObject;
		}
		else if (bodyTarget == BodyTarget.RightFoot)
		{
			gameObject = base.transform.root.GetComponentInChildren<KneeRight>().gameObject;
		}
		else if (bodyTarget == BodyTarget.LeftFoot)
		{
			gameObject = base.transform.root.GetComponentInChildren<KneeLeft>().gameObject;
		}
		else if (bodyTarget == BodyTarget.RightHand)
		{
			gameObject = base.transform.root.GetComponentInChildren<HandRight>().gameObject;
		}
		else if (bodyTarget == BodyTarget.LeftHand)
		{
			gameObject = base.transform.root.GetComponentInChildren<HandLeft>().gameObject;
		}
		CollisionWeapon component = GetComponent<CollisionWeapon>();
		newWeapon = gameObject.AddComponent<CollisionWeapon>();
		newWeapon.damage = component.damage;
		newWeapon.impactMultiplier = component.impactMultiplier;
		newWeapon.onImpactForce = component.onImpactForce;
		newWeapon.massCap = component.massCap;
		newWeapon.ignoreTeamMates = component.ignoreTeamMates;
		newWeapon.staticDamageValue = component.staticDamageValue;
		newWeapon.onlyOncePerData = component.onlyOncePerData;
		newWeapon.cooldown = component.cooldown;
		newWeapon.onlyCollideWithRigs = true;
		newWeapon.dealDamageEvent = component.dealDamageEvent;
		CollisionSound component2 = GetComponent<CollisionSound>();
		if ((bool)component2)
		{
			CollisionSound collisionSound = gameObject.AddComponent<CollisionSound>();
			collisionSound.SoundEffectRef = component2.SoundEffectRef;
			collisionSound.multiplier = component2.multiplier;
			Object.Destroy(component2);
		}

		if (GetComponent<MeleeWeaponAddEffect>())
		{
			var effect = gameObject.AddComponent<MeleeWeaponAddEffect>();
			effect.EffectPrefab = GetComponent<MeleeWeaponAddEffect>().EffectPrefab;
			effect.ignoreTeamMates = GetComponent<MeleeWeaponAddEffect>().ignoreTeamMates;
			Destroy(GetComponent<MeleeWeaponAddEffect>());
		}
		Object.Destroy(component);
	}

	private void OnDestroy()
	{
		if ((bool)newWeapon)
		{
			Object.Destroy(newWeapon);
		}
	}
}
