using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour
{
	public UnityEngine.UI.Text timeText;

	private void UpdateTime(float secs)
	{
		int wholeSecs = Mathf.FloorToInt( secs );
		int ms = Mathf.FloorToInt((secs - wholeSecs) * 1000f);
		System.TimeSpan ts = new System.TimeSpan(0, 0, 0, wholeSecs, ms );
		timeText.text = ts.ToString( );
	}

	private void Start()
	{
		MessageBus.instance.onGameTimeUpdate += UpdateTime;
	}
}


