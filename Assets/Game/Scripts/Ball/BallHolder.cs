using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.SoccerRacing
{
	public class BallHolder : MonoBehaviour 
	{
		public AgentController Agent { get; set; }
		protected Rigidbody _rigidbody;
		protected Ball _ball;

		protected virtual void Start()
		{
			_rigidbody = this.gameObject.GetComponent<Rigidbody> ();
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (collider.tag == "Ball")
			{
				if (Agent.Ragdolling)
				{
					return;
				}

				_ball = collider.gameObject.GetComponentNoAlloc<Ball> ();

				if (_ball.Holder != null)
				{
					return;
				}

				_ball.Attach (_rigidbody);
				_ball.Catch ();

				Agent.HoldingBall = true;
			}
		}
	}
}
