using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.SoccerRacing
{	
	public class BackgroundMusic : Singleton<BackgroundMusic>
	{
		public AudioClip SoundClip ;
	    protected AudioSource _source;
		public bool Loop;

	    protected virtual void Start () 
		{
			_source = gameObject.AddComponent<AudioSource>() as AudioSource;	
			_source.playOnAwake=false;
			_source.spatialBlend=0;
			_source.rolloffMode = AudioRolloffMode.Logarithmic;
			_source.loop=Loop;	
			_source.volume = 0.3f;
			_source.clip=SoundClip;
			
			SoundManager.Instance.PlayBackgroundMusic(_source);
		}
	}
}