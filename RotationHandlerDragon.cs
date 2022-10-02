using Landfall.MonoBatch;
using UnityEngine;

public class RotationHandlerDragon : BatchedMonobehaviour
{
	public float rotationForce;

	public bool ignoreX;

	public bool lockXToZero;

	public bool rotateTowardsMovement;

	private Rigidbody torso;

	private Rigidbody hip;

	private DataHandler data;

	[HideInInspector]
	public float rotationMultiplier = 1f;

	protected override void Start()
	{
		base.Start();
		data = GetComponent<DataHandler>();
		torso = GetComponentInChildren<Torso>().GetComponent<Rigidbody>();
		hip = GetComponentInChildren<Hip>().GetComponent<Rigidbody>();
	}

	public override void BatchedFixedUpdate()
	{
		if (!data.Dead && data.isGrounded && base.enabled)
		{
			Vector3 forward = hip.transform.forward;
			Vector3 forward2 = torso.transform.forward;
			Vector3 forward3 = data.characterForwardObject.forward;
			if (ignoreX)
			{
				forward.z = 0f;
				forward2.z = 0f;
				forward3.z = 0f;
			}
			if (lockXToZero)
			{
				forward3.z = 0f;
			}
			if (rotateTowardsMovement)
			{
				forward3 = data.groundedMovementDirectionObject.forward;
			}
			hip.AddTorque(-Vector3.Cross(forward3, forward).normalized * rotationMultiplier * data.muscleControl * Mathf.Clamp(Vector3.Angle(forward3, forward), 0f, 15f) * rotationForce * data.torsoAngleMultiplier, ForceMode.Acceleration);
			torso.AddTorque(-Vector3.Cross(forward3, forward2).normalized * rotationMultiplier * data.muscleControl * Mathf.Clamp(Vector3.Angle(forward3, forward2), 0f, 15f) * rotationForce * data.torsoAngleMultiplier, ForceMode.Acceleration);
		}
	}
}
