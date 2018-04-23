using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoreMountains.SoccerRacing
{
	public class RagdollHelper : MonoBehaviour 
	{
		public bool ragdolled
		{
			get
			{
				return state!=RagdollState.animated;
			}
			set
			{
				if (value==true)
				{
					if (state==RagdollState.animated) 
					{
						setKinematic(false);
						anim.enabled = false;
						state=RagdollState.ragdolled;
					} 
				}
				else 
				{
					if (state == RagdollState.ragdolled) 
					{
						setKinematic(true); 
						ragdollingEndTime=Time.time; 
						anim.enabled = true;
						state = RagdollState.blendToAnim;  

						foreach (BodyPart b in bodyParts)
						{
							b.storedRotation=b.transform.rotation;
							b.storedPosition=b.transform.position;
						}

						ragdolledFeetPosition=0.5f*(anim.GetBoneTransform(HumanBodyBones.LeftToes).position + anim.GetBoneTransform(HumanBodyBones.RightToes).position);
						ragdolledHeadPosition=anim.GetBoneTransform(HumanBodyBones.Head).position;
						ragdolledHipPosition=anim.GetBoneTransform(HumanBodyBones.Hips).position;
							
						if (anim.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0) 
						{
							anim.SetBool("GetUpFromBack",true);
						}
						else
						{					
							anim.SetBool("GetUpFromBelly",true);
						}
					} 
				}	
			} 
		} 

		enum RagdollState
		{
			animated,	 
			ragdolled,   
			blendToAnim  
		}

		RagdollState state=RagdollState.animated;

		public float ragdollToMecanimBlendTime=0.5f;
		float mecanimToGetUpTransitionTime=0.05f;

		float ragdollingEndTime=-100;

		public class BodyPart
		{
			public Transform transform;
			public Vector3 storedPosition;
			public Quaternion storedRotation;
		}
		Vector3 ragdolledHipPosition,ragdolledHeadPosition,ragdolledFeetPosition;

		List<BodyPart> bodyParts=new List<BodyPart>();

		Animator anim;

		void setKinematic(bool newValue)
		{
			Component[] components=GetComponentsInChildren(typeof(Rigidbody));

			foreach (Component c in components)
			{
				if (c.transform != this.transform)
				{
					(c as Rigidbody).isKinematic=newValue;	
				}
			}
		}

		void Start ()
		{
			setKinematic(true);

			Component[] components=GetComponentsInChildren(typeof(Transform));

			foreach (Component c in components)
			{
				if (c.transform != this.transform)
				{
					BodyPart bodyPart = new BodyPart ();
					bodyPart.transform = c as Transform;
					bodyParts.Add (bodyPart);
				}
			}

			anim=GetComponent<Animator>();
		}
		

		void Update ()
		{
		}
		
		void LateUpdate()
		{
			anim.SetBool("GetUpFromBelly",false);
			anim.SetBool("GetUpFromBack",false);

			if (state==RagdollState.blendToAnim)
			{
				if (Time.time<=ragdollingEndTime+mecanimToGetUpTransitionTime)
				{
					Vector3 animatedToRagdolled=ragdolledHipPosition-anim.GetBoneTransform(HumanBodyBones.Hips).position;
					Vector3 newRootPosition=transform.position + animatedToRagdolled;
						
					RaycastHit[] hits=Physics.RaycastAll(new Ray(newRootPosition,Vector3.down)); 
					newRootPosition.y=0;
					foreach(RaycastHit hit in hits)
					{
						if (!hit.transform.IsChildOf(transform))
						{
							newRootPosition.y=Mathf.Max(newRootPosition.y, hit.point.y);
						}
					}
					transform.position=newRootPosition;

					Vector3 ragdolledDirection=ragdolledHeadPosition-ragdolledFeetPosition;
					ragdolledDirection.y=0;

					Vector3 meanFeetPosition=0.5f*(anim.GetBoneTransform(HumanBodyBones.LeftFoot).position + anim.GetBoneTransform(HumanBodyBones.RightFoot).position);
					Vector3 animatedDirection=anim.GetBoneTransform(HumanBodyBones.Head).position - meanFeetPosition;
					animatedDirection.y=0;
											
					transform.rotation*=Quaternion.FromToRotation(animatedDirection.normalized,ragdolledDirection.normalized);
				}
				float ragdollBlendAmount=1.0f-(Time.time-ragdollingEndTime-mecanimToGetUpTransitionTime)/ragdollToMecanimBlendTime;
				ragdollBlendAmount=Mathf.Clamp01(ragdollBlendAmount);

				foreach (BodyPart b in bodyParts)
				{
					if (b.transform!=transform){ 
						if (b.transform==anim.GetBoneTransform(HumanBodyBones.Hips))
							b.transform.position=Vector3.Lerp(b.transform.position, b.storedPosition, ragdollBlendAmount);
						b.transform.rotation=Quaternion.Slerp(b.transform.rotation, b.storedRotation, ragdollBlendAmount);
					}
				}

				if (ragdollBlendAmount==0)
				{
					state=RagdollState.animated;
					return;
				}
			}
		}
	}
}
