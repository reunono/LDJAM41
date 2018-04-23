using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
namespace MoreMountains.SoccerRacing
{
	public class Bumper : MonoBehaviour 
	{
		public float PushbackForce = 10000f;

		protected virtual void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.GetComponentNoAlloc<AgentController>() != null)
			{
				Vector3 direction = collision.contacts [0].point - collision.transform.position;
				direction = -direction.normalized;
				collision.gameObject.GetComponentNoAlloc<AgentController> ().PushBack (PushbackForce * direction);
			}
		}
	}
}
