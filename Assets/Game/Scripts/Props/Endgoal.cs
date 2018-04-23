using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.MMInterface;
using System;

namespace MoreMountains.SoccerRacing
{
	public class Endgoal : MonoBehaviour 
	{
		public GameObject Arrow;
		public ParticleSystem ArrowExplosion;
		public GameObject GreenLight;
		public GameObject RedLight;

		protected int _totalCheckpoints;

		protected virtual void Start()
		{
			GreenLight.SetActive (false);
			RedLight.SetActive (true);
			ArrowExplosion.Stop ();
			_totalCheckpoints = GameManager.Instance.Checkpoints.Length;
		}

		protected virtual void Reach()
		{
			GreenLight.SetActive (true);
			RedLight.SetActive (false);
			ArrowExplosion.Play ();
			Arrow.SetActive (false);
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{			
			if (collider.tag == "Ball")
			{

				if (GameManager.Instance.CurrentCheckpoint != _totalCheckpoints - 1)
				{
					return;
				}
				Reach ();
				GameManager.Instance.Score ();
			}
		}
	}
}
