using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.SoccerRacing
{	
	public struct RugbyBotsEvents
	{
		public GameStates EventType;
		public RugbyBotsEvents(GameStates eventType)
		{
			EventType = eventType;
		}
	} 
} 
