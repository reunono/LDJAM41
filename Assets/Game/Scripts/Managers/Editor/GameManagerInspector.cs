using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.SoccerRacing
{
	[CustomEditor(typeof(GameManager))]
	public class GameManagerInspector : Editor 
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GameManager gameManager = (GameManager)target;

			// adds movement and condition states
			if (gameManager.GameState!=null)
			{
				EditorGUILayout.LabelField("Game State",gameManager.GameState.CurrentState.ToString());
			}

			DrawDefaultInspector();

			serializedObject.ApplyModifiedProperties();
		}		
	}
}
