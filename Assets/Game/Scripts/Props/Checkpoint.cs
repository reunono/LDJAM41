using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.MMInterface;
using System;

namespace MoreMountains.SoccerRacing
{
	public class Checkpoint : MonoBehaviour 
	{
		public bool Reached = false;
		public GameObject Arrow;
		public ParticleSystem ArrowExplosion;
		public GameObject GreenLight;
		public GameObject RedLight;
		protected Animator _animator;

		protected virtual void Start()
		{
			_animator = GetComponent<Animator> ();
			GreenLight.SetActive (false);
			RedLight.SetActive (true);
			ArrowExplosion.Stop ();
		}


		protected virtual void Reach()
		{
			Reached = true;
			_animator.SetBool ("Reached", Reached);
			GreenLight.SetActive (true);
			RedLight.SetActive (false);
			ArrowExplosion.Play ();
			Arrow.SetActive (false);
			if (GameManager.Instance.CurrentCheckpoint == GameManager.Instance.Checkpoints.Length -1)
			{
				SfxManager.Instance.FinalCheckpoint ();
			}
			else
			{
				SfxManager.Instance.Checkpoint ();	
			}
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (Reached) { return; }
			if (collider.tag == "Ball")
			{
				if (GameManager.Instance.ValidateCheckpoint(this))
				{
					Reach ();
				}
			}
		}
	}
}
