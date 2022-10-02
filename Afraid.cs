using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

public class Afraid : UnitEffectBase
{
	public UnityEvent pingEvent;

	private DataHandler data;

	private Unit unit;

	private float amount;

	public float multiplier = 1f;

	public float torque = 1f;

	public float force = 1f;

	private float counter;

	private Vector3 direction;

	private bool done;

	private void Init()
	{
		unit = base.transform.root.GetComponent<Unit>();
		data = unit.GetComponentInChildren<DataHandler>();
	}

	public override void DoEffect()
	{
		Init();
		if ((bool)data && !(data.targetData == null))
		{
			amount += 1.5f / Mathf.Clamp(data.maxHealth * 0.02f, 0.1f, float.PositiveInfinity);
			direction = -(data.targetData.mainRig.position - data.mainRig.position).normalized;
			direction.y = 0.2f;
			direction = direction.normalized;
			counter = 0f;
		}
	}

	public override void Ping()
	{
		Init();
		if ((bool)data && !(data.targetData == null))
		{
			amount += 1.5f / Mathf.Clamp(data.maxHealth * 0.02f, 0.1f, float.PositiveInfinity);
			direction = -(data.targetData.mainRig.position - data.mainRig.position).normalized;
			direction.y = 0.2f;
			direction = direction.normalized;
			counter = 0f;
			pingEvent.Invoke();
			done = false;
		}
	}

	private void Update()
	{
		if (!data)
		{
			return;
		}
		counter += Time.deltaTime;
		if (counter < 2.5f)
		{
			unit.unitConfusion = amount * multiplier * direction;
			counter += Time.deltaTime;
			if (unit.data.weaponHandler != null)
			{
				unit.data.weaponHandler.StopAttacksFor(1f);
			}
			Vector3 forward = unit.data.mainRig.transform.forward;
			unit.data.mainRig.AddTorque(torque * Vector2.Angle(forward, direction) * Time.deltaTime * Vector3.up, ForceMode.Acceleration);
			Rigidbody[] allRigs = unit.data.allRigs.AllRigs;
			for (int i = 0; i < allRigs.Length; i++)
			{
				if ((bool)allRigs[i])
				{
					allRigs[i].AddForce(force * Time.deltaTime * Mathf.Clamp(allRigs[i].drag, 0f, 1f) * direction, ForceMode.Acceleration);
				}
			}
			data.mainRig.AddForce(force * Mathf.Clamp(data.mainRig.drag, 0f, 1f) * Time.deltaTime * direction, ForceMode.Acceleration);
		}
		else if (!done)
		{
			done = true;
			unit.unitConfusion = Vector3.zero;
		}
	}
}
