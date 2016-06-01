using UnityEngine;
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

		public GameObject settingsPanel;

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
			settingsPanel.SetActive( false );
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
			settingsPanel.SetActive( true );
		}

		public void HandleSettingsBackButtonClicked( )
		{
			settingsPanel.SetActive( false );
		}

		public void HandleDebugButton( )
		{
			MessageBus.instance.dispatchToggleDebugObjects( );
		}

		public void HandleLeftButtonPressed( )
		{
			GameManager.Instance.SetControlForce( Vector2.left );
		}

		public void HandleLeftButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}

		public void HandleRightButtonPressed( )
		{
			GameManager.Instance.SetControlForce( Vector2.right );
		}

		public void HandleRightButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}

		public void HandleUpButtonPressed( )
		{
			GameManager.Instance.SetControlForce( Vector2.up);
		}

		public void HandleUpButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}

		public void HandleDownButtonPressed( )
		{
			GameManager.Instance.SetControlForce( Vector2.down );
		}

		public void HandleDownButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}


	}
}
