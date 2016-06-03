using UnityEngine;
using System.Collections;

public class GameManager : RJWard.Core.Singleton.SingletonSceneLifetime< GameManager>
{
	private static readonly bool DEBUG_LOCAL = false;

	public RJWard.Tube.Player.Player player;
	private RJWard.Tube.Tube tube_ = null;

	private Rect viewPort_;
	public void SetViewPort(Rect r)
	{
		viewPort_ = r;
	}

	public RJWard.Tube.TubeFactory.RandLinearSectionDefn randLinearSectionDefn;

	public void SetCameraToViewport( UnityEngine.Camera camera )
	{
		camera.rect = viewPort_;
	}

	private float startTime_ = 0f;

	private bool isPlaying_ = false;

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

	public RJWard.Tube.SpinePoint_Base GetFirstSpinePoint( )
	{
		RJWard.Tube.SpinePoint_Base result = null;
		if (tube_ != null)
		{
			result = tube_.FirstSpinePoint( );
		}
		return result;
	}


	public void CreateRandomSection( )
	{
		randLinearSectionDefn.firstHoop = null;
		if (tube_ != null)
		{
			RJWard.Tube.Hoop lastHoop = tube_.LastHoop( );
			if (lastHoop != null)
			{
				if (DEBUG_LOCAL)
				{
					Debug.Log( "Creating random section to append" );
				}
				randLinearSectionDefn.firstHoop = lastHoop.ExplicitDefinition( );
				RJWard.Tube.TubeFactory.Instance.CreateRandomLinearSection( tube_, randLinearSectionDefn, HandleTubeSectionMade );
			}
			else
			{
				if (DEBUG_LOCAL)
				{
					Debug.Log( "Creating random section to start" );
				}
				RJWard.Tube.TubeFactory.Instance.CreateRandomLinearSection( tube_, randLinearSectionDefn, HandleFirstTubeSectionMade);
			}
		}
	}

	public void ExtendTubeSection(RJWard.Tube.TubeSection_Linear ts)
	{
		ts.ExtendSection( randLinearSectionDefn, tube_.AddToEnd );
	}

	private void StartPlayer( )
	{
		if (DEBUG_LOCAL)
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
		GameObject tubeGO = new GameObject( "Tube" );
		tubeGO.transform.position = Vector3.zero;
		tubeGO.transform.localScale = Vector3.one;
		tube_ = tubeGO.AddComponent<RJWard.Tube.Tube>( );
	}

	public void StartGame()
	{
		if (isPlaying_)
		{
			Debug.LogError( "Already playing" );
		}
		else
		{
			startTime_ = Time.time;
			isPlaying_ = true;
			if (tube_.FirstSection( ) == null)
			{
				CreateRandomSection( );
			}
		}
	}

	private float storedTimeScale_ = 0f;

	private bool isPaused_ = false;

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
			MessageBus.instance.dispatchGamePaused( false );
		}
	}

	public float FlowZone_defaultWeight = 1f;
	public float FlowZone_defaultSpeed = 1f;

	public float controlForceMultiplier = 1f;

	private Vector2 currentControlForce_ = Vector2.zero;
	public Vector2 currentControlForce
	{
		get { return controlForceMultiplier * currentControlForce_;  }
	}

	public void SetControlForce( Vector2 force )
	{
		currentControlForce_ = force;
	}

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
}


