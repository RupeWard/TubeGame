using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour
{
	public UnityEngine.UI.Text timeText;
	public UnityEngine.UI.Text levelText;

	public GameObject[] fpsObjects = new GameObject[0];

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
		MessageBus.instance.setLevelText += SetLevelText;
		MessageBus.instance.setTimeText += SetTimeText;
		MessageBus.instance.onShowFPSChanged += OnShowFPSChanged;
	}

	private void OnShowFPSChanged(bool b)
	{
		foreach(GameObject go in fpsObjects)
		{
			go.SetActive( b );
		}
	}

	private void SetLevelText(string s)
	{
		levelText.text = s;
	}

	private void SetTimeText( string s )
	{
		timeText.text = s;
	}

}

partial class MessageBus
{

}


