using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;


namespace MoreMountains.SoccerRacing
{
	public class InputManager : MonoBehaviour 
	{
		[Header("Input Buttons")]
		public MMTouchButton LeftButton;
		public MMTouchButton RightButton;
		public float AxisSpeed = 4f;


		[Header("Energy Modifiers")]
		public float HighEnergySpeedMultiplier = 1.1f;
		public float LowEnergySpeedMultiplier = 0.8f;

		public AgentController TargetAgent;
		public AgentController TestAI;

		protected float _horizontalAxisInput;

		protected Vector3 _newForwardForce;
		protected Vector3 _newTorque;

		protected float _registeredHorizontalInput;
		protected bool _lerpHorizontal = false;

		protected bool _leftPressed = false;
		protected bool _rightPressed = false;

		protected float _lastPressLeft = 0;
		protected float _lastPressRight = 0;
		protected const float _doublePressDelay = 0.35f;


		protected virtual void Start()
		{
			BindToController ();
		}

		public virtual void BindToController()
		{
			//TODO
		}

		protected virtual void Update()
		{
			if (GameManager.Instance.GameState.CurrentState == GameStates.StartScreen)
			{
				if (Input.anyKey && !Input.GetKey(KeyCode.Mouse0))
				{
					GameManager.Instance.StartGame ();
				}
			}

			if ((GameManager.Instance.GameState.CurrentState == GameStates.GameInProgress)
				|| (GameManager.Instance.GameState.CurrentState == GameStates.Score))
			{
				GetInput ();
				HandleInput ();
			}
		}

		protected virtual void GetInput()
		{
			_registeredHorizontalInput = 0;
			_lerpHorizontal = false;
			_leftPressed = false;
			_rightPressed = false;



			/*if ((LeftButton.CurrentState == MMTouchButton.ButtonStates.ButtonDown) 
				|| (LeftButton.CurrentState == MMTouchButton.ButtonStates.ButtonPressed)
				|| Input.GetKey("left")
				|| Input.GetKeyDown("left"))
			{
				_leftPressed = true;
			}

			if ((RightButton.CurrentState == MMTouchButton.ButtonStates.ButtonDown) 
				|| (RightButton.CurrentState == MMTouchButton.ButtonStates.ButtonPressed)
				|| Input.GetKey("right")
				|| Input.GetKeyDown("right"))
			{
				_rightPressed = true;
			}

			if (_leftPressed && !_rightPressed)
			{
				_registeredHorizontalInput += -1;
				_lerpHorizontal = true;				
			}

			if (_rightPressed && !_leftPressed)
			{
				_registeredHorizontalInput += 1;
				_lerpHorizontal = true;				
			}

			if (_lerpHorizontal)
			{
				_horizontalAxisInput = Mathf.Lerp (_horizontalAxisInput, _registeredHorizontalInput, Time.deltaTime * AxisSpeed);
			}
			else
			{
				_horizontalAxisInput = Mathf.Lerp (_horizontalAxisInput, 0, Time.deltaTime * AxisSpeed);				
			}*/

			_registeredHorizontalInput = Input.GetAxis ("Horizontal");
			_horizontalAxisInput = Mathf.Lerp (_horizontalAxisInput, _registeredHorizontalInput, Time.deltaTime * AxisSpeed);

			if (Input.GetButtonDown("Action"))
			{
				TargetAgent.Action ();
			}
			/*
			if (Input.GetKeyDown(KeyCode.J))
			{
				TargetAgent.Ragdoll ();
			}
			if (Input.GetKey(KeyCode.K))
			{
				TestAI.Ragdoll ();
			}

			*/
		}

		protected virtual void HandleInput()
		{
			_newForwardForce = TargetAgent.transform.forward * 1f;
			_newTorque = TargetAgent.transform.up * _horizontalAxisInput ;

			_newForwardForce = Vector3.ClampMagnitude (_newForwardForce, 1f);

			TargetAgent.SetForwardForce (_newForwardForce);
			TargetAgent.SetTorque (_newTorque);	

			/*MMDebug.DebugLogTime (GameManager.Instance.GameState.CurrentState);
			if ((GameManager.Instance.GameState.CurrentState == GameStates.LevelStart)
				|| (GameManager.Instance.GameState.CurrentState == GameStates.GameInProgress))
			{
				TargetAgent.SetForwardForce (_newForwardForce);
				TargetAgent.SetTorque (_newTorque);	
			}
			else
			{
				TargetAgent.SetForwardForce (Vector3.zero);
				TargetAgent.SetTorque (_newTorque);
			}*/


		}
	}
}
