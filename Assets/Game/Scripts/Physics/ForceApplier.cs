using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class ForceApplier : MonoBehaviour 
{
	public Vector3 Force;

	[InspectorButton("AddForceAcceleration")]
	public bool AddForceAccelerationButton;

	[InspectorButton("AddForceForce")]
	public bool AddForceForceButton;

	[InspectorButton("AddForceImpulse")]
	public bool AddForceImpulseButton;

	[InspectorButton("AddForceVelocityChange")]
	public bool AddForceVelocityChangeButton;

	[InspectorButton("ResetPosition")]
	public bool ResetPositionButton;

	protected Rigidbody _rigidbody;
	protected Vector3 _initialPosition;
	protected Quaternion _initialRotation;

	protected virtual void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_initialPosition = this.transform.position;
		_initialRotation = this.transform.rotation;
	}

	public virtual void AddForceAcceleration()
	{
		_rigidbody.AddForce(this.transform.rotation * Force, ForceMode.Acceleration);
	}

	public virtual void AddForceForce()
	{
		_rigidbody.AddForce(this.transform.rotation * Force, ForceMode.Force);
	}

	public virtual void AddForceImpulse()
	{
		_rigidbody.AddForce(this.transform.rotation * Force, ForceMode.Impulse);
	}

	public virtual void AddForceForceVelocityChange()
	{
		_rigidbody.AddForce(this.transform.rotation * Force, ForceMode.VelocityChange);
	}

	public virtual void ResetPosition()
	{
		_rigidbody.velocity = Vector3.zero;
		this.transform.position = _initialPosition;
		this.transform.rotation = _initialRotation;
	}
}
