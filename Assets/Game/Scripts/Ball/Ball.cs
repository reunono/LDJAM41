using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.SoccerRacing;

namespace MoreMountains.SoccerRacing
{
	public class Ball : MonoBehaviour 
	{
		public ParticleSystem CatchExplosion;
		public GameObject Holder = null;

		protected Rigidbody _rigidbody;
		protected FixedJoint _fixedJoint;

		protected SphereCollider _sphereCollider;

		protected virtual void Start()
		{
			_rigidbody = GetComponent<Rigidbody> ();
			_sphereCollider = GetComponent<SphereCollider> ();
			CatchExplosion.Stop ();
		}

		public virtual void Attach(Rigidbody attachRigidbody)
		{
			this.transform.position = attachRigidbody.transform.position;
			_fixedJoint = this.gameObject.AddComponent<FixedJoint> ();
			_fixedJoint.connectedBody = attachRigidbody;
			_fixedJoint.massScale = 1f;
			_fixedJoint.connectedMassScale = 1f;
			Holder = attachRigidbody.gameObject;
		}

		public virtual void Catch()
		{
			CatchExplosion.Play ();
			SfxManager.Instance.Catch ();
		}

		public virtual void Detach()
		{
			if (_fixedJoint != null)
			{
				_fixedJoint.connectedBody.gameObject.GetComponentNoAlloc<BallHolder> ().Agent.HoldingBall = false;
				Destroy (_fixedJoint);	
			}
			if (Holder != null)
			{
				Holder.GetComponentNoAlloc<BallHolder> ().Agent.HoldingBall = false;
				Holder = null;	
			}
		}

		public virtual void Shoot(Vector3 direction)
		{
			GameManager.Instance.DisableEnemyHolders ();
			Detach ();
			_rigidbody.AddForce (direction, ForceMode.Impulse);
		}
	}
}
