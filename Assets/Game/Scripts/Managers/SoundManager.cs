using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.SoccerRacing
{	
	public class SoundManager : PersistentSingleton<SoundManager>
	{	
		public bool MusicOn=true;
		public bool SfxOn=true;

		public bool VibrationsOn = true;
		[Range(0,1)]
		public float MusicVolume=0.3f;
		[Range(0,1)]
		public float SfxVolume=1f;

		protected AudioSource _backgroundMusic;	
		protected float _initialMusicVolume;
		protected float _initialSfxVolume;

		public bool IsPlaying
		{
			get
			{
				return _backgroundMusic.isPlaying;					
			}
		}

		protected virtual void Start()
		{
			_initialSfxVolume = SfxVolume;
			_initialMusicVolume = MusicVolume;
		}

		protected virtual void Update()
		{
			if ((_backgroundMusic!= null) && (_backgroundMusic.isPlaying))
			{
				_backgroundMusic.volume = MusicVolume;	
			}
		}

		public virtual void ToggleAllSounds()
		{
			MusicOn = !MusicOn;
			SfxOn = !SfxOn;

			SfxVolume = SfxOn ? _initialSfxVolume : 0f;
			MusicVolume = MusicOn ? _initialMusicVolume : 0f;
		}

		public virtual void ToggleVibrations()
		{
			VibrationsOn = !VibrationsOn;
		}

		public virtual void PlayBackgroundMusic(AudioSource Music, bool shouldLoop=true)
		{
			if (!MusicOn)
				return;
			if (_backgroundMusic!=null)
				_backgroundMusic.Stop();
			_backgroundMusic=Music;
			_backgroundMusic.volume=MusicVolume;
			_backgroundMusic.loop=shouldLoop;
			_backgroundMusic.Play();		
		}	

		public virtual AudioSource PlaySound(AudioClip sfx, Vector3 location, bool shouldDestroyAfterPlay=true)
		{
			if (!SfxOn)
				return null;
			GameObject temporaryAudioHost = new GameObject("TempAudio");
			temporaryAudioHost.transform.position = location;
			AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource; 
			audioSource.clip = sfx; 
			audioSource.volume = SfxVolume/3f;
			audioSource.Play(); 
			if (shouldDestroyAfterPlay)
			{
				Destroy(temporaryAudioHost, sfx.length);
			}
			return audioSource;
		}

		public virtual AudioSource PlayLoop(AudioClip Sfx, Vector3 Location)
		{
			if (!SfxOn)
				return null;
			GameObject temporaryAudioHost = new GameObject("TempAudio");
			temporaryAudioHost.transform.position = Location;
			AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource; 
			audioSource.clip = Sfx; 
			audioSource.volume = 0;
			audioSource.loop=true;
			audioSource.Play(); 
			return audioSource;
		}
	}
}