﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RJWard.Core.UI;

namespace RJWard.Tube.UI
{
	public class GameSceneUIManager : RJWard.Core.Singleton.SingletonSceneLifetime<GameSceneUIManager>
	{
		private Rect viewPort_ = new Rect( );
		public RectTransform bottomPanelRT;
		public RectTransform viewPortRT;

		public Canvas mainCanvas;
		private RectTransform mainCanvasRT_ = null;

		public UnityEngine.UI.Text playButtonText;

		protected override void PostAwake( )
		{
			mainCanvasRT_ = mainCanvas.GetComponent<RectTransform>( );

			viewPortRT.SetHeight( viewPortRT.GetWidth( ) );

			viewPort_.x = 0f;
			viewPort_.width = 1f;

			viewPort_.height = viewPortRT.GetHeight( ) / mainCanvasRT_.GetHeight( );
			viewPort_.y = 1f - viewPort_.height;

			bottomPanelRT.SetHeight( mainCanvasRT_.GetHeight( ) - viewPortRT.GetHeight( ) );

			MessageBus.instance.gamePauseAction += HandleGamePaused;
		}

		private void HandleGamePaused(bool paused)
		{
			playButtonText.text = (paused) ? ("Play") : ("Pause");
		}

		private void Start()
		{
			GameManager.Instance.SetViewPort( viewPort_ );
		}

		public void HandleBackButtonClicked( )
		{
			SceneManager.Instance.SwitchScene( SceneManager.EScene.DevSetup );
		}

		public void HandlePlayButtonClicked()
		{
			GameManager.Instance.PlayOrPause( );
		}

		public void HandleSettingsButtonClicked()
		{
			
		}

		public void HandleDebugButton( )
		{
			MessageBus.instance.dispatchToggleDebugObjects( );
		}

	}
}
