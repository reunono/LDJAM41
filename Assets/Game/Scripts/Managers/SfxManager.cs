using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.SoccerRacing
{
	public class SfxManager : Singleton<SfxManager> 
	{
		public AudioClip TackleStartSfx;
		public AudioClip GoalSfx;
		public AudioClip ShootSfx;
		public AudioClip PushSfx;
		public AudioClip CatchSfx;
		public AudioClip TackleSfx;
		public AudioClip CheckpointSfx;
		public AudioClip FinalCheckpointSfx;

		public virtual void TackleStart()
		{
			SoundManager.Instance.PlaySound (TackleStartSfx, Vector3.zero, true);
		}
		public virtual void Goal()
		{
			SoundManager.Instance.PlaySound (GoalSfx, Vector3.zero, true);
		}
		public virtual void Shoot()
		{
			SoundManager.Instance.PlaySound (ShootSfx, Vector3.zero, true);
		}
		public virtual void Push()
		{
			SoundManager.Instance.PlaySound (PushSfx, Vector3.zero, true);
		}
		public virtual void Catch()
		{
			SoundManager.Instance.PlaySound (CatchSfx, Vector3.zero, true);
		}
		public virtual void Tackle()
		{
			SoundManager.Instance.PlaySound (TackleSfx, Vector3.zero, true);
		}
		public virtual void Checkpoint()
		{
			SoundManager.Instance.PlaySound (CheckpointSfx, Vector3.zero, true);
		}
		public virtual void FinalCheckpoint()
		{
			SoundManager.Instance.PlaySound (FinalCheckpointSfx, Vector3.zero, true);
		}
	}
}
