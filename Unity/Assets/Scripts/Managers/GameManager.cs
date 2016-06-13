using UnityEngine;
using System.Collections;

public class GameManager : RJWard.Core.Singleton.SingletonSceneLifetime< GameManager>
{
	private static readonly bool DEBUG_LOCAL = false;
	static private readonly bool DEBUG_GAMEFLOW = true;

	#region inspector hooks

	public RJWard.Tube.Player.Player player;

	#endregion inspector hooks

	#region inspector data

	public RJWard.Tube.RandLinearSectionDefn defaultRandLinearSectionDefn;
	public float FlowZone_defaultWeight = 1f;
	public float FlowZone_defaultSpeed = 1f;

	public float controlForceMultiplier = 1f;


	#endregion inspector data

	#region private data

	private RJWard.Tube.Tube tube_ = null;
	private Rect viewPort_;

	private RJWard.Tube.Game_Base game_ = null;
	public T GetGame<T>( ) where T : RJWard.Tube.Game_Base
	{
		return game_ as T;
	}

	private float startTime_ = 0f;
	private bool isPlaying_ = false;
	public bool isPlaying
	{
		get { return isPlaying_; }
	}

	private float storedTimeScale_ = 0f;

	private bool isPaused_ = false;
	public bool isPaused
	{
		get { return isPaused_;  }
	}

	private int numFirstGameSections_ = 0;
	private Vector2 currentControlForce_ = Vector2.zero;

	#endregion private data

	#region control

	public Vector2 currentControlForce
	{
		get { return controlForceMultiplier * currentControlForce_; }
	}

	public void SetControlForce( Vector2 force )
	{
		currentControlForce_ = force;
	}

	#endregion control

	#region viewport

	public void SetViewPort( Rect r )
	{
		viewPort_ = r;
	}

	public void SetCameraToViewport( UnityEngine.Camera camera )
	{
		camera.rect = viewPort_;
	}

	#endregion viewport

	#region tube accessors

	public RJWard.Tube.SpinePoint_Base GetFirstSpinePoint( )
	{
		RJWard.Tube.SpinePoint_Base result = null;
		if (tube_ != null)
		{
			result = tube_.FirstSpinePoint( );
		}
		return result;
	}

	#endregion tube accessors

	#region tube generation

	public void ExtendTubeSection( RJWard.Tube.TubeSection_Linear ts )
	{
		RJWard.Tube.RandLinearSectionDefn sectionDefn = null;
		if (game_ != null)
		{
			sectionDefn = game_.GetNextTubeSectionDefn( );
		}
		else
		{
			Debug.LogWarning( "No game object, using default from editor" );
			sectionDefn = defaultRandLinearSectionDefn;
		}
		ts.ExtendSection( sectionDefn, tube_.AddToEnd );
	}

	private void HandleFirstGameSectionCreated( RJWard.Tube.TubeSection_Linear ts )
	{
		if (DEBUG_GAMEFLOW)
		{
			Debug.Log( "Adding first section #" + numFirstGameSections_ + " (" + ts.gameObject.name + ")" );
		}
		tube_.AddToEnd( ts, CreateFirstGameSections );
	}

	private void HandleTubeSectionMade( RJWard.Tube.TubeSection_Linear ts )
	{
		if (DEBUG_LOCAL)
		{
			Debug.Log( "Adding new section" );
		}
		tube_.AddToEnd( ts, null );
	}

	private void HandleFirstTubeSectionMade( RJWard.Tube.TubeSection_Linear ts )
	{
		if (DEBUG_LOCAL)
		{
			Debug.Log( "Adding first section" );
		}
		tube_.AddToEnd( ts, StartPlayer );
	}

	#endregion tube generation

	#region game flow

	private void CreateFirstGameSections()
	{
		if (game_ == null)
		{
			Debug.LogError( "No game object" );
		}
		else
		{
			int numSpinePoints = 0;
			if (tube_.FirstSpinePoint( ) != null)
			{
				numSpinePoints = tube_.FirstSpinePoint( ).MinSpinePointsToEnd( );
			}
			if (numSpinePoints < player.maxSpinePointsToGap)
			{
				if (DEBUG_GAMEFLOW)
				{
					Debug.Log( "Created " + numFirstGameSections_ + " with maxD = " + numSpinePoints + " spine points" );
				}
				numFirstGameSections_++;
				RJWard.Tube.TubeFactory.Instance.CreateRandomLinearSection( tube_, defaultRandLinearSectionDefn, HandleFirstGameSectionCreated );
			}
			else
			{
				if (DEBUG_GAMEFLOW)
				{
					Debug.Log( "Created enough first sections (" + numFirstGameSections_ + ") with maxD = " + numSpinePoints + " spine points" );
				}
				StartPlayer( );
			}

		}
	}

	private void StartPlayer( )
	{
		if (DEBUG_LOCAL || DEBUG_GAMEFLOW)
		{
			Debug.Log( "Start Player" );
		}

		player.gameObject.SetActive( true );
		RJWard.Tube.SpinePoint_Base firstSpinePoint = GetFirstSpinePoint( );
		if (firstSpinePoint != null)
		{
			player.StartGame( tube_ );
		}
		else
		{
			Debug.LogWarning( "Can't start player with no tube" );
		}
	}

	private void Update()
	{
		if (isPlaying_)
		{
			float secs = Time.time - startTime_;
			MessageBus.instance.dispatchGameTimeUpdate( secs );
		}
	}

	protected override void PostAwake( )
	{
		//		Application.targetFrameRate = 60;

		Debug.Log( "GameManager waking." );// TFR = "+Application.targetFrameRate );
		
		GameObject tubeGO = new GameObject( "Tube" );
		tubeGO.transform.position = Vector3.zero;
		tubeGO.transform.localScale = Vector3.one;
		tube_ = tubeGO.AddComponent<RJWard.Tube.Tube>( );

		controlForceMultiplier = SettingsStore.retrieveSetting<float>( SettingsIds.controlForceMultiplierSettingId );
		player.speed = SettingsStore.retrieveSetting<float>( SettingsIds.playerSpeedMultiplierSettingId );
	}

	public void StartGame()
	{
		if (isPlaying_)
		{
			Debug.LogError( "Already playing" );
		}
		else
		{
			game_ = new RJWard.Tube.Game_Constant( defaultRandLinearSectionDefn );
			startTime_ = Time.time;
			isPlaying_ = true;
			if (tube_.FirstSection( ) == null)
			{
				numFirstGameSections_ = 0;
				CreateFirstGameSections( );
			}
			MessageBus.instance.dispatchResetFPS( );
		}
	}	

	public void PlayOrPause()
	{
		if (isPlaying_)
		{
			if (isPaused_)
			{
				if (DEBUG_LOCAL)
				{
					Debug.Log( "PlayOrPause when playing and paused" );
				}
				Time.timeScale = storedTimeScale_;
				isPaused_ = false;
				MessageBus.instance.dispatchGamePaused( false );
			}
			else
			{
				if (DEBUG_LOCAL)
				{
					Debug.Log( "PlayOrPause when playing" );
				}
				storedTimeScale_ = Time.timeScale;
				Time.timeScale = 0f;
				isPaused_ = true;
				MessageBus.instance.dispatchGamePaused( true );
			}
		}
		else
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "PlayOrPause when not playing" );
			}
			StartGame( );
//			MessageBus.instance.dispatchGamePaused( false );
			MessageBus.instance.dispatchGameStarted( );
		}
	}
	#endregion game flow


}

public partial class MessageBus : MonoBehaviour
{
	public System.Action<float> onGameTimeUpdate;
	public void dispatchGameTimeUpdate( float secs )
	{
		if (onGameTimeUpdate != null)
		{
			onGameTimeUpdate( secs );
		}
	}

	public System.Action<bool> gamePauseAction;
	public void dispatchGamePaused( bool b)
	{
		if (gamePauseAction != null)
		{
			gamePauseAction( b );
		}
	}

	public System.Action gameStartAction;
	public void dispatchGameStarted( )
	{
		if (gameStartAction != null)
		{
			gameStartAction(  );
		}
	}
}


