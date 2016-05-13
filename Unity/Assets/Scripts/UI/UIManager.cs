using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RJWard.Core.UI;

namespace RJWard.Tube.UI
{
	public class UIManager : RJWard.Core.Singleton.SingletonApplicationLifetime<UIManager>
	{
		public RectTransform permanentPanel;
		public UnityEngine.UI.Text cameraButtonText;
		public UnityEngine.UI.Text versionText;

		private Rect viewPort_ = new Rect( );
		public RectTransform bottomPanelRT;
		public RectTransform viewPortRT;

		public Canvas mainCanvas;
		private RectTransform mainCanvasRT_ = null;

		public GameObject externalCamPanel;
		public GameObject debugPanel;
		public GameObject mainGamePanel;

		private enum EMode
		{
			UNKNOWN,
			Debug,
			ExternalCam,
			MainGame
		}

		private Dictionary<EMode, GameObject> panelsByMode_ = new Dictionary<EMode, GameObject>( );

		private EMode currentMode_ = EMode.UNKNOWN;

		public void SetCameraToViewport(UnityEngine.Camera camera)
		{
			camera.rect = viewPort_;
		}

		private void ChangeMode(EMode newMode)
		{
			if (currentMode_ != newMode)
			{
				EMode previousMode = currentMode_;
				currentMode_ = EMode.UNKNOWN;
				foreach ( EMode mode in panelsByMode_.Keys )
				{
					if (mode == newMode)
					{
						Debug.Log( "Changing mode from " + previousMode + " to " + newMode );
						currentMode_ = mode;
						panelsByMode_[mode].SetActive( true );
					}
					else
					{
						panelsByMode_[mode].SetActive( false );
					}
				}
				if (currentMode_ == EMode.UNKNOWN)
				{
					Debug.LogError( "Couldn't find panel to display for mode = " + newMode );
				}
			}
			else
			{
				Debug.LogWarning( "Mode is already" + currentMode_ );
			}
		}

		protected override void PostAwake( )
		{
			panelsByMode_.Add( EMode.Debug, debugPanel );
			panelsByMode_.Add( EMode.ExternalCam, externalCamPanel );
			panelsByMode_.Add( EMode.MainGame, mainGamePanel );

			mainCanvasRT_ = mainCanvas.GetComponent<RectTransform>( );

			versionText.text = RJWard.Core.Version.versionNumber.DebugDescribe( );

			viewPortRT.SetHeight( viewPortRT.GetWidth( ) );

			viewPort_.x = 0f;
			viewPort_.width = 1f;

			viewPort_.height = viewPortRT.GetHeight() / mainCanvasRT_.GetHeight( );
			viewPort_.y = 1f - viewPort_.height;

			bottomPanelRT.SetHeight( mainCanvasRT_.GetHeight( ) - viewPortRT.GetHeight( ) );

			ChangeMode( EMode.Debug );

		}

		public void HandleQuitButton()
		{
			Application.Quit( );
		}

		public void HandleExtCamHookButton()
		{
			TestSceneManager.Instance.ToggleExtCamOnHook( );
			
		}

		public void HandleDebugButton()
		{
			TestSceneManager.Instance.HandleDebugButtonPressed( );
		}

		public void HandleExtCamForwardButtonDown()
		{
			TestSceneManager.Instance.HandleCameraForwardDown( );
		}

		public void HandleExtCamForwardButtonUp( )
		{
			TestSceneManager.Instance.HandleCameraMotionButtonUp( );
		}

		public void HandleExtCamBackButtonDown( )
		{
			TestSceneManager.Instance.HandleCameraBackDown( );
		}

		public void HandleExtCamBackButtonUp( )
		{
			TestSceneManager.Instance.HandleCameraMotionButtonUp( );
		}

		public void HandleStopExtCameraButton()
		{
			TestSceneManager.Instance.HandleCameraStopPressed( );
		}

		public void HandleModeButton()
		{
			switch (currentMode_)
			{
				case EMode.Debug:
					{
						ChangeMode( EMode.ExternalCam );
						break;
					}
				case EMode.ExternalCam:
					{
						ChangeMode( EMode.MainGame);
						TestSceneManager.Instance.StartPlayMode( );
						break;
					}
				case EMode.MainGame:
					{
						TestSceneManager.Instance.EndPlayMode( );
						ChangeMode( EMode.Debug);
						break;
					}
			}
		}

		public void HandleStartPlayerButton()
		{
			TestSceneManager.Instance.StartPlayer( );
		}
	}

}
