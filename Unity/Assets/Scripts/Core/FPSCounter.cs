using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Core.Unity
{
	public class FPSCounter : MonoBehaviour
	{
		public UnityEngine.UI.Text fpsText;

		private Queue<float> intervals_ = new Queue<float>( );
		public int maxIntervals = 100;
		public int minIntervals = 50;
		public float accum = 0f;
		public int displayInterval = 40;
		private int sinceDisplay = 0;

		private bool active_ = true;
		public void ToggleActive( )
		{
			SetActive( !active_ );
		}

		public void SetActive( bool b)
		{
			if (active_ != b)
			{
				active_ = b;
				if (!active_)
				{
					intervals_.Clear( );
					if (fpsText != null)
					{
						fpsText.text = "FPS";
					}
				}
			}
		}

		public System.Action<float> SendFPS;

		private void Awake()
		{
			SetActive( SettingsStore.retrieveSetting<bool>( SettingsIds.showFPSId ) );
			MessageBus.instance.onShowFPSChanged += SetActive;
		}

		private void Update( )
		{
			if (active_)
			{
				intervals_.Enqueue( Time.deltaTime );
				accum += Time.deltaTime;

				while (intervals_.Count > maxIntervals)
				{
					float f = intervals_.Dequeue( );
					accum -= f;
				}
				if (intervals_.Count >= minIntervals)
				{
					sinceDisplay--;
					if (sinceDisplay < 0)
					{
						sinceDisplay = displayInterval;
						float meanDeltaTime = (accum / intervals_.Count);
						float fps = 1f / meanDeltaTime;
						if (SendFPS != null)
						{
							SendFPS( fps );
						}
						if (fpsText != null)
						{
							fpsText.text = fps.ToString( "F1" );
						}
					}
				}
			}

		}

	}



}

public partial class MessageBus : MonoBehaviour
{
	public System.Action<bool> onShowFPSChanged;
	public void dispatchOnShowFPSChanged( bool b )
	{
		if (onShowFPSChanged != null)
		{
			onShowFPSChanged( b );
		}
	}
}

