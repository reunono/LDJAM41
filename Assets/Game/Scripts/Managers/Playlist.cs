using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.SoccerRacing
{	
	public class Playlist : PersistentSingleton<Playlist>
	{
		public AudioClip FirstSong;
		public AudioClip[] SoundClip ;
		public bool Loop=false;
		protected AudioSource _source;
		public int _currentTrack=0;
		public bool _started=false;

		public List<AudioClip> NewList;

	    protected virtual void Start () 
		{
			if (_started)
			{
				return;
			}
			else
			{
				_started=true;

				NewList = new List<AudioClip> ();
				for (int k = 0; k < SoundClip.Length; k++) 
				{
					AudioClip temp = SoundClip[k];
					NewList.Add(temp);
				}

				_source = gameObject.AddComponent<AudioSource>() as AudioSource;	
				_source.playOnAwake=false;
				_source.spatialBlend=0;
				_source.rolloffMode = AudioRolloffMode.Logarithmic;
				_source.loop=Loop;			
				_source.clip=FirstSong;	
				SoundManager.Instance.PlayBackgroundMusic(_source,Loop);
			}

		}

		protected virtual void Update()
		{
			if (!SoundManager.Instance.IsPlaying) 
			{
				_currentTrack++;
				if (_currentTrack >= NewList.Count) 
				{
					_currentTrack = 0;
				}
				_source.playOnAwake=false;
				_source.spatialBlend=0;
				_source.rolloffMode = AudioRolloffMode.Logarithmic;
				_source.loop=Loop;			
				_source.clip=NewList[_currentTrack];	
				SoundManager.Instance.PlayBackgroundMusic(_source,Loop);
			}
		}

		public virtual void Play(int i)
		{
			
		}

		public virtual void OnDestroy()
		{
			
		}


	}
}