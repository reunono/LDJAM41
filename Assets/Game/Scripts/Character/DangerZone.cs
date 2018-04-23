using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.SoccerRacing;

namespace MoreMountains.SoccerRacing
{
	public class DangerZone : MonoBehaviour 
	{
		public float DeathForce;
		public AudioClip ImpactSfx;
		public AgentController ParentAgentController;
		public float radius;

		[Header("Explosions")]
		public ParticleSystem Explosion;
		public ParticleSystem SmallExplosion;

		protected Vector3 _impactForce;
		protected SphereCollider _sphereCollider;

		protected virtual void Start()
		{
			_sphereCollider = GetComponent<SphereCollider> ();
			Explosion.Stop ();
		}

		protected virtual void Update()
		{
			_sphereCollider.radius = radius;
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			Collision (other.gameObject);
		}			

		protected virtual void Collision(GameObject collidingObject)
		{
			if (collidingObject.tag != "Player" && collidingObject.tag != "AI")
			{
				return;
			}

			// if we shoot at an enemy
			if (this.tag == "Ball")
			{			
				return;
			}

			if (ParentAgentController.tag != "Player" && ParentAgentController.tag != "AI")
			{
				return;
			}

			if (ParentAgentController.Ragdolling)
			{
				return;
			}

			if (ParentAgentController.HoldingBall)
			{
				return;
			}

			if (ParentAgentController.tag == "Player")
			{			
				if (collidingObject.tag != "AI")
				{
					return;
				}

				if (ParentAgentController.Tackling)
				{
					// player tackles AI

					_impactForce = DeathForce * (collidingObject.transform.position - this.transform.position);
					_impactForce.y = 5000f;
					collidingObject.GetComponentNoAlloc<Rigidbody> ().AddForce (_impactForce);

					GameManager.Instance.GameBall.Detach ();
					collidingObject.GetComponentNoAlloc<AgentController> ().Ragdoll();
					collidingObject.GetComponentNoAlloc<AgentController> ().DisableHolderAndDangerZone();
					Explosion.Play ();
					MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 50f));
					SfxManager.Instance.Tackle ();
				}
				else
				{
					// player pushes AI

					_impactForce = DeathForce * (collidingObject.transform.position - this.transform.position);
					_impactForce.y = 5000f;
					collidingObject.GetComponentNoAlloc<Rigidbody> ().AddForce (_impactForce);

					ParentAgentController.SetHitAnimator ();
					collidingObject.GetComponentNoAlloc<AgentController> ().SetHitAnimator();

					GameManager.Instance.GameBall.Detach ();
					collidingObject.GetComponentNoAlloc<AgentController> ().DisableHolderAndDangerZone();
					SmallExplosion.Play ();
					MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 25f));
					SfxManager.Instance.Push ();
				}

			}

			if (ParentAgentController.tag == "AI")
			{
				if (GameManager.Instance.GameState.CurrentState != GameStates.GameInProgress)
				{
					return;
				}

				if (collidingObject.tag != "Player")
				{
					return;
				}

				if (ParentAgentController.Tackling)
				{
					// AI tackles player

					_impactForce = DeathForce * (collidingObject.transform.position - this.transform.position);
					_impactForce.y = 5000f;
					collidingObject.GetComponentNoAlloc<Rigidbody> ().AddForce (_impactForce);

					GameManager.Instance.GameBall.Detach ();
					collidingObject.GetComponentNoAlloc<AgentController> ().Ragdoll();
					collidingObject.GetComponentNoAlloc<AgentController> ().DisableHolderAndDangerZone();
					Explosion.Play ();
					MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 50f));
					SfxManager.Instance.Tackle ();
				}
				else
				{
					// AI pushes player

					_impactForce = DeathForce * (collidingObject.transform.position - this.transform.position);
					_impactForce.y = 5000f;
					collidingObject.GetComponentNoAlloc<Rigidbody> ().AddForce (_impactForce);

					ParentAgentController.SetHitAnimator ();
					collidingObject.GetComponentNoAlloc<AgentController> ().SetHitAnimator();

					GameManager.Instance.GameBall.Detach ();
					collidingObject.GetComponentNoAlloc<AgentController> ().DisableHolderAndDangerZone();
					SmallExplosion.Play ();
					MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 25f));
					SfxManager.Instance.Push ();
				}
			}
		}
	}
}
