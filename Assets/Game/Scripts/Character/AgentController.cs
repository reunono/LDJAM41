using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.SoccerRacing;

namespace MoreMountains.SoccerRacing
{
	public class AgentController : MonoBehaviour 
	{
		[Header("Setup")]
		public int PlayerID;
		
		[Header("Speed")]
		public float RunSpeed = 1000.0F;
		public float RotateSpeed = 200.0F;
		public bool ShouldAngleAgent = false;

		[Header("Dash")]
		public float DashMultiplier = 2f;
		public float DashDuration = 1f;
		public float HoldingBallMultiplier = 0.8f;

		[Header("Dependencies")]
		public DangerZone ChildDangerZone;
		public Animator CharacterAnimator;
		public GameObject ModelContainer;
		public ParticleSystem FootStepsParticleSystem;

		[Header("Binding")]
		public ParticleSystem TackleParticles;
		public ParticleSystem ShootParticles;
		public GameObject BallHolderContainer;
		public BallHolder ChildBallHolder;
		public RagdollHelper TargetRagdollHelper;
		public Rigidbody RagdollRigidBody;


		[Header("Debug")]
		public bool DrawDebug = true;

		[ReadOnly]
		public bool HoldingBall = false;
					
		[ReadOnly]
		public bool Ragdolling = false;

		[ReadOnly]
		public bool Tackling = false;

		public Vector3 Velocity { get { return _rigidBody.velocity; }}

		protected const float _groundRayLength = 5f;

		protected float _movementSpeed;

		protected Rigidbody _rigidBody;
		protected Animator _animator;

		protected List<string> _animatorParameters;

		protected Vector3 _forwardForce;
		protected Vector3 _torque; 

		protected float _currentSpeed;
		protected float _currentRotationSpeed;

		protected bool _grounded = false;

		protected float _maximumCharacterAngle = 20f;

		protected float LastTackleTimestamp = -20f;

		protected ParticleSystem.EmissionModule _footstepsEmissionModule;
		protected SphereCollider _sphereCollider;
		protected WaitForSeconds _shootDelay;

		protected float _lastDetachedAt = -10f;
		protected Vector3 _newAngle;

		protected virtual void Awake()
		{
			if (BallHolderContainer != null)
			{
				BallHolderContainer.transform.SetParent (null);	
			}
		}

		protected virtual void Start ()
		{
			Initialization ();
		}

		public virtual void Initialization()
		{
			_shootDelay = new WaitForSeconds (0.15f);
			_rigidBody = GetComponent<Rigidbody>();
			_sphereCollider = GetComponent<SphereCollider> ();
			ChildDangerZone.ParentAgentController = this;
			ShootParticles.Stop ();
			TackleParticles.Stop ();
			if (ChildBallHolder != null)
			{
				ChildBallHolder.Agent = this;	
			}
			_animator = CharacterAnimator;
			_footstepsEmissionModule = FootStepsParticleSystem.emission;
			InitializeAnimatorParameters ();
			_animator.SetInteger ("PlayerID", PlayerID);
		}

		protected virtual void Update()
		{
			HandleTackle ();
			UpdateAnimators ();
			AngleAgent ();
			HandleFootsteps ();
			HandleHolder ();
			HandleRagdolling ();
		}

		protected virtual void HandleRagdolling ()
		{
			if (Ragdolling)
			{
				_rigidBody.isKinematic = true;
				_sphereCollider.enabled = false;
			}
			else
			{
				_rigidBody.isKinematic = false;
				_sphereCollider.enabled = true;
			}
		}

		protected virtual void HandleHolder()
		{
			if (ChildBallHolder == null)
			{
				return;
			}
			if (Time.time - _lastDetachedAt > 1.5f)
			{
				ChildBallHolder.enabled = true;
				ChildDangerZone.enabled = true;
			}
		}

		public virtual void SetHitAnimator()
		{
			_animator.SetTrigger ("Hit");
		}

		public virtual void DisableHolderAndDangerZone()
		{
			ChildDangerZone.enabled = false;
			ChildBallHolder.enabled = false;
			_lastDetachedAt = Time.time;
		}

		protected virtual void HandleTackle()
		{
			if (Time.time - LastTackleTimestamp > DashDuration)
			{
				Tackling = false;
			}
			else
			{
				Tackling = true;
			}
			
			if (Tackling)
			{
				_movementSpeed = RunSpeed * DashMultiplier;
				if (this.tag == "Player")
				{
					ChildDangerZone.radius = 6f;	
				}
			}
			else
			{
				_movementSpeed = RunSpeed;
				ChildDangerZone.radius = 3f;
			}

			if (HoldingBall)
			{
				_movementSpeed = _movementSpeed * HoldingBallMultiplier;
			}
		}

		protected virtual void FixedUpdate ()
		{
			MoveCharacter ();
		}

		public virtual void SetForwardForce(Vector3 forwardForce)
		{
			_forwardForce = forwardForce;
		}

		public virtual void SetTorque(Vector3 torque)
		{
			_torque = torque;
		}

		protected virtual void MoveCharacter()
		{			
			if (Ragdolling) { return; }

			_rigidBody.AddTorque(_torque * RotateSpeed);
			_rigidBody.AddForce (_forwardForce * _movementSpeed);	 

			_currentSpeed = _rigidBody.velocity.magnitude;
			_currentRotationSpeed = _torque.magnitude;
		}

		public virtual void RotateToFaceDirection()
		{
			transform.rotation = Quaternion.LookRotation (-_rigidBody.velocity);
		}

		public virtual void Action()
		{
			if (tag == "Player")
			{
				if (GameManager.Instance.Energy < 100f)
				{
					GUIManager.Instance.BumpEnergyBar ();
					return;
				}
				GameManager.Instance.UseEnergy (100f);
			}

			if (HoldingBall)
			{
				Shoot ();
			}
			else
			{
				Tackle ();
			}
		}

		public virtual void Shoot()
		{
			MMAnimator.UpdateAnimatorTrigger (_animator, "Shoot", _animatorParameters);
			StartCoroutine (ShootCoroutine ());
		}

		protected virtual IEnumerator ShootCoroutine()
		{
			yield return _shootDelay;
			ShootParticles.Play ();
			SfxManager.Instance.Shoot ();
			MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 50f));
			GameManager.Instance.GameBall.Shoot (this.transform.forward * 100f + Vector3.up * 35f);
		}

		public virtual void Tackle()
		{
			SfxManager.Instance.TackleStart ();
			TackleParticles.Play ();
			LastTackleTimestamp = Time.time;
			MMAnimator.UpdateAnimatorTrigger (_animator, "Dash", _animatorParameters);
		}

		public virtual void PushBack(Vector3 direction)
		{
			_rigidBody.AddForce (direction, ForceMode.Impulse);
			RotateToFaceDirection ();
		}

		protected virtual void AngleAgent()
		{
			if (!ShouldAngleAgent)
			{
				_newAngle = Vector3.forward;
			}
			else
			{
				_newAngle = -Mathf.Clamp (_torque.y, -1f, 1f)
					* _maximumCharacterAngle
					* Mathf.InverseLerp(0f, 12f, Mathf.Abs(_currentSpeed))
					* Vector3.forward;	
			}
			ModelContainer.transform.localEulerAngles = _newAngle;
		}

		public virtual void Ragdoll()
		{
			TargetRagdollHelper.ragdolled=true;
			Ragdolling = true;
			RagdollRigidBody.transform.SetParent (this.transform);
			RagdollRigidBody.AddForce (RagdollRigidBody.transform.rotation * new Vector3 (0f, 8000f, 20000f), ForceMode.Acceleration);
			StartCoroutine(GetBackUpCoroutine());
		}

		protected virtual IEnumerator GetBackUpCoroutine()
		{
			yield return new WaitForSeconds (3f);
			GetBackUp ();
			yield return new WaitForSeconds (3.5f);

			Ragdolling = false;
		}

		public virtual void GetBackUp()
		{
			RagdollRigidBody.transform.SetParent (ModelContainer.transform);
			TargetRagdollHelper.ragdolled=false;

		}

		protected virtual void HandleFootsteps ()
		{
			_footstepsEmissionModule.enabled = (_currentSpeed > 5f);
			if (Tackling || Ragdolling)
			{
				_footstepsEmissionModule.enabled = false;
			}
		}

		protected virtual void DetermineIfGrounded ()
		{
			RaycastHit raycast3D = MMDebug.Raycast3D(transform.position,Vector3.down,_groundRayLength,1<<LayerMask.NameToLayer("Ground"),Color.green,true);
			if (raycast3D.transform != null)
			{
				//TODO grounded
			}
		}

		protected virtual void OnDrawGizmos()
		{
			if (DrawDebug)
			{
				MMDebug.DebugDrawArrow (this.transform.position, this.transform.forward, Color.blue, 5f);	
			}
		}

		protected virtual void InitializeAnimatorParameters()
		{
			if (_animator == null) { return; }
			if (!_animator.isActiveAndEnabled) { return; }

			_animatorParameters = new List<string>();

			MMAnimator.AddAnimatorParamaterIfExists(_animator,"Dash",AnimatorControllerParameterType.Trigger,_animatorParameters);
			MMAnimator.AddAnimatorParamaterIfExists(_animator,"Shoot",AnimatorControllerParameterType.Trigger,_animatorParameters);
			MMAnimator.AddAnimatorParamaterIfExists(_animator,"Speed",AnimatorControllerParameterType.Float,_animatorParameters);
		}

		protected virtual void UpdateAnimators()
		{	
			if (_animator != null)
			{ 
				MMAnimator.UpdateAnimatorFloat (_animator, "Speed", _currentSpeed, _animatorParameters);
			}
		}
	}
}