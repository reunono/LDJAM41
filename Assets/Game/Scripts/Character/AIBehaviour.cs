using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;


namespace MoreMountains.SoccerRacing
{
	[RequireComponent(typeof(AgentController))]
	public class AIBehaviour : MonoBehaviour 
	{
		[Range(0f, 1f)]
		public float FullThrottle = 1f; 

		[Range(0f, 1f)]
		public float SmallThrottle = 1f; 

		[Header("Intervention Zone")]
		public float InterventionDistance = 25f;
		public float ChangeTargetEvery = 3f;
		public float ChangeTargetEveryNoBall = 1.5f;
		public float ShootEvery = 5f;

		protected const float _largeAngleDistance = 90f; 
		protected const float _smallAngleDistance = 5f;  
		protected const float _minimalSpeedForBrakes = 0.5f; 
		protected const float _maximalDistanceStuck = 0.5f; 

		public AgentController Controller { get { return _agentController; }}

		protected float _targetAngleAbsolute;
		protected int _newDirection;

		protected Transform _target;
		protected AgentController _player;
		protected Ball _ball;


		protected AgentController _agentController;
		protected float _distanceToTarget;
		protected float _distanceFromOrigin;

		protected float _direction = 0f;
		protected float _acceleration = 0f;

		protected Vector3 _newForwardForce;
		protected Vector3 _newTorque;

		protected Vector3 _leftRaycast;
		protected Vector3 _rightRaycast;

		protected float _shootDelay = 5f;
		protected float _lastPickedTargetAt = 0f;
		protected Vector3 _newTargetPosition;
		protected Vector3 _initialPosition;


		protected virtual void Start()
		{
			Initialization ();
		}

		protected virtual void Initialization()
		{
			_agentController = GetComponent<AgentController> ();


			_player = GameManager.Instance.Player;
			_ball = GameManager.Instance.GameBall;
			_initialPosition = this.transform.position;
		}

		protected virtual void FixedUpdate()
		{
			if (GameManager.Instance.GameState.CurrentState == GameStates.GameInProgress)
			{
				if (!_agentController.Ragdolling)
				{
					PickTarget ();
					EvaluateDirection ();
					EvalutateAccelerationAndDirection();
					RequestMovement ();		
				}
			}
			else
			{
				KillMovement ();
			}
		}

		protected virtual void PickTarget()
		{
			if (!_agentController.HoldingBall)
			{
				// if our AI is not holding the ball
				if (Time.time - _lastPickedTargetAt > ChangeTargetEveryNoBall)
				{					
					if (_player.HoldingBall)
					{
						_target = _player.transform;
						_newTargetPosition = _target.position;
					} else
					{
						_target = _ball.transform;
						_newTargetPosition = _target.position;
					}	


					if (_distanceFromOrigin > InterventionDistance)
					{
						_newTargetPosition = _initialPosition;
					}
					_lastPickedTargetAt = Time.time;
				}
			}
			else
			{
				_shootDelay -= Time.deltaTime;

				if (_shootDelay <= 0)
				{
					_agentController.Shoot ();
					_shootDelay = ShootEvery;
				}

				// if the AI has the ball
				if (Time.time - _lastPickedTargetAt > ChangeTargetEvery)
				{
					_newTargetPosition.x = UnityEngine.Random.Range (-58f, 150f);
					_newTargetPosition.y = 0f;
					_newTargetPosition.z = UnityEngine.Random.Range (-140f, 140f);
					_lastPickedTargetAt = Time.time;
				}
			}

		}

		protected virtual void KillMovement()
		{
			_newForwardForce = Vector3.zero;
			_newTorque = Vector3.zero;
			_agentController.SetForwardForce (_newForwardForce);
			_agentController.SetTorque (_newTorque);
		}

		protected virtual void EvaluateDirection()
		{
			Vector3 targetVector =_newTargetPosition - transform.position;
			_distanceToTarget = targetVector.magnitude;
			targetVector.y = 0;
			Vector3 transformForwardPlane = transform.forward;
			transformForwardPlane.y = 0;
			_targetAngleAbsolute = Vector3.Angle(transformForwardPlane, targetVector);
			Vector3 cross = Vector3.Cross(transformForwardPlane, targetVector);
			_newDirection = cross.y >= 0 ? 1 : -1;
			Vector3 targetLocal = transform.InverseTransformPoint(_newTargetPosition);
			if (targetLocal.z < 0)
			{
				_newDirection = (targetLocal.x < 0) ? 1 : -1;
			}
		}

		protected virtual void EvalutateAccelerationAndDirection()
		{
			if (_targetAngleAbsolute > _largeAngleDistance)
			{
				_direction = -_newDirection;
				if (_agentController.Velocity.magnitude > _minimalSpeedForBrakes)
				{
				}
				else
				{
					_acceleration = SmallThrottle;
				}
			}
			else if (_targetAngleAbsolute > _smallAngleDistance)
			{
				_direction = _newDirection;
				_acceleration = SmallThrottle;
			}
			else
			{
				_direction = 0f;
				_acceleration = FullThrottle;
			}

			_leftRaycast = transform.forward;
			_leftRaycast = Quaternion.AngleAxis (-25, Vector3.up) * _leftRaycast;
			_rightRaycast = transform.forward;
			_rightRaycast = Quaternion.AngleAxis(25,Vector3.up) * _rightRaycast;

			RaycastHit raycast3DLeft = MMDebug.Raycast3D(transform.position,_leftRaycast,25f,1<<LayerMask.NameToLayer("Ground"),Color.blue,true);
			RaycastHit raycast3DRight = MMDebug.Raycast3D(transform.position,_rightRaycast,25f,1<<LayerMask.NameToLayer("Ground"),Color.blue,true);
			if (raycast3DLeft.transform != null)
			{
				if (raycast3DLeft.distance < _distanceToTarget)
				{
					_direction = 1;
				}
			}
			if (raycast3DRight.transform != null)
			{
				if (raycast3DRight.distance < _distanceToTarget)
				{
					_direction = -1;
				}
			}
		}

		protected virtual void RequestMovement()
		{			
			_newForwardForce = transform.forward * Mathf.Abs(_acceleration) ;
			_newTorque = transform.up * _direction ;

			if ((_distanceToTarget > InterventionDistance) && (InterventionDistance != 0f) && !_agentController.HoldingBall)
			{
				_newForwardForce = Vector3.zero;
			}

			_agentController.SetForwardForce (_newForwardForce);
			_agentController.SetTorque (_newTorque);
		}

		protected virtual void OnDrawGizmos()
		{
			MMDebug.DrawGizmoPoint (this.transform.position, InterventionDistance, Color.yellow);
		}

	}
}