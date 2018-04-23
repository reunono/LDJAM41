using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.SoccerRacing
{
	public class StartScreen : MonoBehaviour
	{
		public string NextLevel;
		public float AutoSkipDelay = 0f;

		public Button SoundButtonOn;
		public Button SoundButtonOff;

		protected float _initialMusicVolume;
		protected float _initialSfxVolume;

		protected float _delayAfterClick = 1f;

		protected virtual void Start()
		{	
			GUIManager.Instance.SetHUDActive (false);
			GUIManager.Instance.FaderOn (false, 1f);
			Application.targetFrameRate = 300;

			if (AutoSkipDelay >= 1f)
			{
				GUIManager.Instance.FaderOn (true, _delayAfterClick);
				_delayAfterClick = AutoSkipDelay;
				StartCoroutine (LoadFirstLevel ());
			}

			_initialMusicVolume = SoundManager.Instance.MusicVolume;
			_initialSfxVolume = SoundManager.Instance.SfxVolume;
		}

		public virtual void ButtonPressed()
		{
			GUIManager.Instance.FaderOn (true, _delayAfterClick);



			StartCoroutine (LoadFirstLevel ());
		}

		protected virtual IEnumerator LoadFirstLevel()
		{
			yield return new WaitForSeconds (_delayAfterClick);

			GameManager.Instance.SelfDestruction ();

			LoadingSceneManager.LoadScene (NextLevel);
		}

		public virtual void SoundOn()
		{
			SoundButtonOff.gameObject.SetActive(false);
			SoundButtonOn.gameObject.SetActive (true);

			SoundManager.Instance.MusicVolume = _initialMusicVolume;
			SoundManager.Instance.SfxVolume = _initialSfxVolume;

		}

		public virtual void SoundOff()
		{
			SoundButtonOn.gameObject.SetActive (false);
			SoundButtonOff.gameObject.SetActive(true);

			SoundManager.Instance.MusicVolume = 0;
			SoundManager.Instance.SfxVolume = 0;
		}
	}
}