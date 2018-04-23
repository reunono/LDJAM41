using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.SoccerRacing
{
	public class StartScreenInputManager : MonoBehaviour 
	{
		public string NextLevel;

		protected float _delayAfterClick = 1f;

		protected virtual void Update()
		{
			if (Input.anyKeyDown)
			{
				if (Input.GetMouseButtonDown(0) 
					|| Input.GetMouseButtonDown(1)
					|| Input.GetMouseButtonDown(2))
					return; //Do Nothing
				
				GUIManager.Instance.FaderOn (true, _delayAfterClick);
				StartCoroutine (LoadNextLevel ());
			}
		}
		protected virtual IEnumerator LoadNextLevel()
		{
			yield return new WaitForSeconds (_delayAfterClick);
			GameManager.Instance.SelfDestruction ();
			LoadingSceneManager.LoadScene (NextLevel);
		}
	}
}
