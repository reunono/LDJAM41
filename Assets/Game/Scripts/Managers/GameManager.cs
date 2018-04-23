using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.MMInterface;
using System;
using Cinemachine;

namespace MoreMountains.SoccerRacing
{
	public enum GameStates
	{
		StartScreen,
		LevelStart,
		GameInProgress,
		Score,
		Pause,
		UnPause,
		PlayerDeath,
		GameOverScreen
	}

	[Serializable]
	public class FirstUse  
	{
		public bool Used;
	}

	public class GameManager : Singleton<GameManager>
	{
		public MMStateMachine<GameStates> GameState;

		[Header("Binding")]
		public AgentController Player;
		public Ball GameBall;
		public Checkpoint[] Checkpoints;
		public Indicator CheckpointIndicator;
		public GameObject EndGoal;
		public CinemachineBrain TargetBrain;
		public List<CinemachineVirtualCamera> Cameras;

		public AgentController[] Agents;
		public GameObject[] AIs;
		public List<AgentController> Enemies;

		[Header("Framerate")]
		public int TargetFrameRate = 300;

		[Header("Countdown")]
		public int StartCountdown = 3;
		public string StartCountdownEndedText = "Go!";

		[Header("Score")]
		public int GoalPoints = 5;
		public long CurrentScore = 0;

		public long CurrentLevel = 0;

		public bool GamePaused = false;

		public MMPopup HowToPlayPopup;


		[ReadOnly]
		public int CurrentCheckpoint = -1;

		[ReadOnly]
		public float TimeSinceStart = 0f;
		public float Energy = 100f;
		public float EnergyPerSecond = 25f;

		protected int _currentCameraID = 1;
		protected float _lastCycledCamerasAt = 0f;
		protected const float _timescale = 1f;

		protected override void Awake()
		{
			Time.timeScale = _timescale;
			Time.fixedDeltaTime = _timescale * 0.02f;

			base.Awake ();
			Initialization ();
		}

		protected virtual void Update()
		{
			if (GameState.CurrentState == GameStates.StartScreen)
			{
				CycleCameras ();
			}

			Energy += EnergyPerSecond * Time.deltaTime;
			Energy = Mathf.Clamp(Energy, 0f, 100f);

			if (GameState.CurrentState == GameStates.GameInProgress)
			{
				TimeSinceStart += Time.deltaTime;
				UpdateCheckpointIndicator ();
			}
		}

		protected virtual void CycleCameras ()
		{
			if (Time.time - _lastCycledCamerasAt > 3f)
			{
				_currentCameraID++;
				if (_currentCameraID == 5)
				{
					_currentCameraID = 1;
				}
				SetCamera (_currentCameraID);
				_lastCycledCamerasAt = Time.time;

			}
		}

		protected virtual void SetCamera(int id)
		{
			foreach (CinemachineVirtualCamera cam in Cameras)
			{
				cam.Priority = 0;
			}
			Cameras [id].Priority = 10;
		}

		protected virtual void UpdateCheckpointIndicator()
		{
			if (CurrentCheckpoint < Checkpoints.Length - 1)
			{
				CheckpointIndicator.IndicatorTarget = Checkpoints [CurrentCheckpoint + 1].transform;
			}
			else
			{
				CheckpointIndicator.IndicatorTarget = EndGoal.transform;
			}
		}

		public virtual bool ValidateCheckpoint(Checkpoint check)
		{
			int id = -1;
			for (int i=0; i<Checkpoints.Length; i++)
			{
				if (Checkpoints[i] == check)
				{
					id = i;
				}
			}
			if (id == -1) { return false; }
			if (id == 0) 
			{ 
				CurrentCheckpoint = 0; 
				MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 50f));
				return true; 				
			}
			if (id > 0)
			{
				if (Checkpoints[id - 1].Reached)
				{
					CurrentCheckpoint = id;
					MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 50f));
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}

		public virtual void SetCurrentCheckpoint(int id)
		{
			
		}

		public virtual void UseEnergy(float energy)
		{
			Energy -= energy;
		}

		public virtual void DisableEnemyHolders()
		{
			foreach (AgentController agent in Enemies)
			{
				agent.DisableHolderAndDangerZone ();
			}
		}

		protected virtual void Initialization()
		{
			Energy = 100f;
			Application.targetFrameRate = TargetFrameRate;
			GameState = new MMStateMachine<GameStates> (gameObject,true);
			GUIManager.Instance.FaderOn (false, 1f, false);
			GUIManager.Instance.SetStartScreen (true);

			Enemies = new List<AgentController> ();
			AIs = GameObject.FindGameObjectsWithTag ("AI");
			for (int i=0; i<AIs.Length; i++)
			{
				Enemies.Add (AIs [i].GetComponent<AgentController> ());
			}
			Agents = FindObjectsOfType<AgentController> ();
			for (int i = 0; i < Agents.Length; i++)
			{
				Agents [i].gameObject.GetComponentNoAlloc<Animator> ().SetBool ("StartScreen", true);
			}
			SetCamera (1);
		}

		public virtual void StartGame()
		{
			CurrentScore = 0;
			CurrentLevel = 0;
			SetCamera (0);
			GUIManager.Instance.SetPause (false);
			GamePaused = false;
			CurrentLevel = 1 + CurrentScore / 5;
			GUIManager.Instance.SetGUILevel ();
			GUIManager.Instance.FaderOn (false, 1f, false);
			GameState.ChangeState (GameStates.LevelStart);
			ResetPlayer ();
			StartCoroutine(Countdown ());

			GUIManager.Instance.SetStartScreen (false);
			GUIManager.Instance.SetGameOverScreen (false);

			for (int i = 0; i < Agents.Length; i++)
			{
				Agents [i].gameObject.GetComponentNoAlloc<Animator> ().SetBool ("StartScreen", false);
			}
		}

		public virtual void BackToMenu()
		{
			Time.timeScale = _timescale;
			GUIManager.Instance.SetPause (false);
			GamePaused = false;
			GUIManager.Instance.SetGameOverScreen (false);
			GUIManager.Instance.SetStartScreen (true);
			Player.gameObject.SetActive (false);
			GUIManager.Instance.FaderOn (false, 1f, false);
		}

		protected virtual IEnumerator Countdown ()
		{
			float CountDownDelay = 0.25f;

			if (StartCountdown == 0)
			{
				GUIManager.Instance.SetCountdownTextStatus (false);
				GameState.ChangeState (GameStates.GameInProgress);
				yield break;
			}

			GUIManager.Instance.SetCountdownTextStatus (false);
			yield return new WaitForSeconds (CountDownDelay);
			GUIManager.Instance.SetCountdownTextStatus (true);

			int countdown = StartCountdown;
			GUIManager.Instance.SetCountdownTextStatus (true);
			while (countdown > 0)
			{
				GUIManager.Instance.SetCountdownText (countdown.ToString ());
				countdown--;
				yield return new WaitForSeconds (CountDownDelay);
			}
			if (countdown == 0)
			{
				GUIManager.Instance.SetCountdownText (StartCountdownEndedText);
				yield return new WaitForSeconds (CountDownDelay);
			}
			GUIManager.Instance.SetCountdownTextStatus (false);

			GameState.ChangeState (GameStates.GameInProgress);
			TimeSinceStart = 0f;
		}

		public virtual void TriggerPause()
		{
			if (GamePaused)
			{
				Time.timeScale = _timescale;
				GUIManager.Instance.SetPause (false);
				GamePaused = false;
			}
			else
			{
				Time.timeScale = 0f;
				GUIManager.Instance.SetPause (true);
				GamePaused = true;
			}
		}

		public virtual void Score()
		{
			SfxManager.Instance.Goal ();
			MMEventManager.TriggerEvent (new MMCameraShakeEvent (0.1f, 2f, 50f));
			GameState.ChangeState (GameStates.Score);
			StartCoroutine (GameOver ());
		}

		protected virtual IEnumerator GameOver()
		{
			yield return new WaitForSeconds (2.5f);
			GUIManager.Instance.FaderOn (true, 1f, false);
			yield return new WaitForSeconds (1f);

			GUIManager.Instance.SetGameOverScreen (true);
		}

		protected virtual void ResetPlayer()
		{
			GameState.ChangeState (GameStates.LevelStart);
			//TODO
		}

		public virtual void SelfDestruction()
		{
			Destroy (this.gameObject);
		}	

		public virtual void Restart()
		{
			LoadingSceneManager.LoadScene ("Main");
			SelfDestruction ();
		}		
	}
}
