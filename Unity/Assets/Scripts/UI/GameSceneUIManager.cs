using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RJWard.Core.UI.Extensions;

namespace RJWard.Tube.UI
{
	public class GameSceneUIManager : RJWard.Core.Singleton.SingletonSceneLifetime<GameSceneUIManager>
	{
		private Rect viewPort_ = new Rect( );

		#region inspector hooks

		public RectTransform bottomPanelRT;
		public RectTransform viewPortRT;

		public Canvas mainCanvas;

		public UnityEngine.UI.Text playButtonText;

		public GameObject settingsPanel;

		public RectTransform bottomBackground;

		public UnityEngine.UI.Image debugObjectsButtonImage;
		public UnityEngine.UI.Image fpsButtonImage;
		public UnityEngine.UI.Image controlForceMarkerButtonImage;

		public UnityEngine.UI.Text speedMultButtonText;
		public UnityEngine.UI.Text controlMultButtonText;

		public FloatSettingPanel floatSettingPanel;
		public RandLinearSectionDefnSettingPanel randLinearTubeDefnSettingPanel;

		public GameObject editorControlPanel;
		public GameObject deviceControlPanel;

		#endregion inspector hooks

		#region inspector data

		public Color toggleButtonOnColour = new Color( 9f / 255f, 92f / 255f, 40f / 244f );
		public Color toggleButtonOffColour = new Color( 80f / 255f, 80f / 255f, 80f / 244f );

		#endregion inspector data

		#region private hooks

		private RectTransform mainCanvasRT_ = null;

		#endregion private hooks

		private void SetButtonColourById( UnityEngine.UI.Image image, string id)
		{
			bool active = SettingsStore.retrieveSetting<bool>( id );
			SetButtonColour( image, active );
		}

		private void SetButtonColour( UnityEngine.UI.Image image, bool active)
		{
			if (active)
			{
				image.color = toggleButtonOnColour;
			}
			else
			{
				image.color = toggleButtonOffColour;
			}
		}

		protected override void PostAwake( )
		{
#if UNITY_EDITOR
			deviceControlPanel.SetActive( false );
			editorControlPanel.SetActive( true );
#else
			deviceControlPanel.SetActive( true);
			editorControlPanel.SetActive( false );
#endif
			mainCanvasRT_ = mainCanvas.GetComponent<RectTransform>( );

			viewPortRT.SetHeight( viewPortRT.GetWidth( ) );

			viewPort_.x = 0f;
			viewPort_.width = 1f;

			viewPort_.height = viewPortRT.GetHeight( ) / mainCanvasRT_.GetHeight( );
			viewPort_.y = 1f - viewPort_.height;

			bottomBackground.SetHeight( mainCanvasRT_.GetHeight( ) - viewPortRT.GetHeight( ) );
			bottomPanelRT.SetHeight( mainCanvasRT_.GetHeight( ) - viewPortRT.GetHeight( ) );

			MessageBus.instance.gamePauseAction += HandleGamePaused;
			MessageBus.instance.gameStartAction += HandleGameStarted;

			SetButtonColourById( debugObjectsButtonImage, SettingsIds.showDebugObjectsSettingId );
			SetButtonColourById( controlForceMarkerButtonImage, SettingsIds.showControlForceMarkerSettingId );
			SetButtonColourById( fpsButtonImage, SettingsIds.showFPSId );

			playButtonText.text = "Start";

			settingsPanel.SetActive( false );
		}

		private void HandleGameStarted(  )
		{
			playButtonText.text = "Pause";
			SetSpeedMultText( );
			SetControlMultText( );
		}

		private void HandleGamePaused(bool paused)
		{
			playButtonText.text = (paused) ? ("Continue") : ("Pause");
			if (!paused )
			{
				SetSpeedMultText( );
			}
		}

		private void SetSpeedMultText()
		{
			string text = "Speed Mult: ";
			if (GameManager.Instance.player != null)
			{
				text += GameManager.Instance.player.speed.ToString( );
			}
			else
			{
				text += "???";
			}
			speedMultButtonText.text = text;
		}

		private void SetControlMultText( )
		{
			string text = "Control Mult: ";
			text += GameManager.Instance.controlForceMultiplier.ToString( );
			controlMultButtonText.text = text;
		}


		private void Start()
		{
			GameManager.Instance.SetViewPort( viewPort_ );
			SetSpeedMultText( );
			SetControlMultText( );

		}

		#region button handlers

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
			if (GameManager.Instance.isPlaying &&  !GameManager.Instance.isPaused)
			{
				GameManager.Instance.PlayOrPause( );
			}
		}

		public void HandleSettingsBackButtonClicked( )
		{
			settingsPanel.SetActive( false );
		}

		public void HandleDebugButton( )
		{
			MessageBus.instance.dispatchToggleDebugObjects( );
			bool showThem = SettingsStore.retrieveSetting<bool>( SettingsIds.showDebugObjectsSettingId );
			SetButtonColour( debugObjectsButtonImage,  showThem);
			DebugBlob.ActivateAllDebugBlobs( showThem);
		}

		public void HandleControlMarkersButton()
		{
			MessageBus.instance.dispatchToggleControlMarkers( );
			SetButtonColourById( controlForceMarkerButtonImage, SettingsIds.showControlForceMarkerSettingId);
		}

		public void HandleFPSButton( )
		{
			bool currentSetting = SettingsStore.retrieveSetting<bool>( SettingsIds.showFPSId );
			bool newSetting = !currentSetting;
			SettingsStore.storeSetting( SettingsIds.showFPSId, newSetting );
			MessageBus.instance.dispatchOnShowFPSChanged( newSetting);
			SetButtonColour( fpsButtonImage, newSetting );
		}

		public void HandleTubeDefnButton()
		{
			Game_Constant game = GameManager.Instance.GetGame<Game_Constant>( );
			if (game != null)
			{
				randLinearTubeDefnSettingPanel.Init( "Current Linear Tube Defn",
					game.sectionDefn,
					HandleTubeDefnChanged );
			}
			else
			{
				randLinearTubeDefnSettingPanel.Init( "Default Linear Tube Defn",
					GameManager.Instance.defaultRandLinearSectionDefn,
					HandleTubeDefnChanged );

				/*
				Game_Base gameb = GameManager.Instance.GetGame<Game_Base>( );
				if (gameb != null)
				{
					Debug.LogWarning( "Game is of unhandled type '" + gameb.GetType( ).ToString( ) + "'" );
                }
				else
				{
					Debug.LogWarning( "Game is null!" );
                }
				*/
			}
		}

		public void HandleTubeDefnChanged(RandLinearSectionDefn newdefn)
		{
			Game_Constant game = GameManager.Instance.GetGame<Game_Constant>( );
			if (game != null)
			{
				game.sectionDefn.CopyValuesFrom( newdefn );
			}
			else
			{
				GameManager.Instance.defaultRandLinearSectionDefn.CopyValuesFrom( newdefn );
				/*
				Game_Base gameb = GameManager.Instance.GetGame<Game_Base>( );
				if (gameb != null)
				{
					Debug.LogWarning( "Game is of unhandled type '" + gameb.GetType( ).ToString( ) + "'" );
				}
				else
				{
					Debug.LogWarning( "Game is null!" );
				}
				*/
			}

		}

		public float fourButtonControlMultiplier = 0.1f;

		public void HandleLeftButtonPressed( )
		{
			GameManager.Instance.SetControlForce( fourButtonControlMultiplier * Vector2.left );
		}


		public void HandleLeftButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}

		public void HandleRightButtonPressed( )
		{
			GameManager.Instance.SetControlForce( fourButtonControlMultiplier * Vector2.right );
		}

		public void HandleRightButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}

		public void HandleUpButtonPressed( )
		{
			GameManager.Instance.SetControlForce( fourButtonControlMultiplier * Vector2.up);
		}

		public void HandleUpButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}

		public void HandleDownButtonPressed( )
		{
			GameManager.Instance.SetControlForce( fourButtonControlMultiplier * Vector2.down );
		}

		public void HandleDownButtonReleased( )
		{
			GameManager.Instance.SetControlForce( Vector2.zero );
		}

		public void HandleSpeedMultButtonClicked()
		{
			floatSettingPanel.Init( "Speed multiplier", GameManager.Instance.player.speed, new Vector2( 0f, 100f ), OnSpeedMultChanged );
		}

		public void HandleControlMultButtonClicked( )
		{
			floatSettingPanel.Init( "Control Force multiplier", GameManager.Instance.controlForceMultiplier, new Vector2( 0f, float.MaxValue ), OnControlMultChanged );
		}

#endregion button handlers

#region event handlers

		public void OnSpeedMultChanged(float f)
		{
			SettingsStore.storeSetting( SettingsIds.playerSpeedMultiplierSettingId, f );
			GameManager.Instance.player.speed = f;
			SetSpeedMultText( );
		}

		public void OnControlMultChanged( float f )
		{
			SettingsStore.storeSetting( SettingsIds.controlForceMultiplierSettingId, f );
			GameManager.Instance.controlForceMultiplier = f;
			SetControlMultText( );
		}


#endregion event handlers
	}
}
