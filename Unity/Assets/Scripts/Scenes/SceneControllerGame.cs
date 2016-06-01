using UnityEngine;
using System.Collections;

public class SceneControllerGame : SceneController_Base 
{
	static private readonly bool DEBUG_LOCAL = false;

	#region inspector hooks

	public Camera playerCamera;
	public float FlowZone_defaultWeight = 1f;
	public float FlowZone_defaultSpeed = 1f;

	public int numHoopPoints = 10;

//	public Material tubeWallMaterial;
	//public RJWard.Tube.TubeFactory.RandLinearSectionDefn randLinearSectionDefn;

	//	private RJWard.Tube.Tube tube_ = null;

	//	public RJWard.Tube.Player.Player player;

	#endregion inspector hooks

	/*
		public void ClearTube( )
		{
			tube_.DeleteAllSections( );
		}*/

	/*
private void HandleTubeSectionMade( RJWard.Tube.TubeSection_Linear ts )
{
	tube_.AddToEnd( ts );
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
	//		Debug.Log( "Randomising" );
	randLinearSectionDefn.firstHoop = null;
	if (tube_ != null)
	{
		RJWard.Tube.Hoop lastHoop = tube_.LastHoop( );
		if (lastHoop != null)
		{
			randLinearSectionDefn.firstHoop = lastHoop.ExplicitDefinition( );
		}
	}
	RJWard.Tube.TubeFactory.Instance.CreateRandomLinearSection( tube_, randLinearSectionDefn, HandleTubeSectionMade );
}
*/

	/*
public void EndPlayMode( )
{
	player.gameObject.SetActive( false );
}

public void StartPlayMode( )
{
	if (tube_.FirstSection( ) == null)
	{
		CreateRandomSection( );
	}
	player.gameObject.SetActive( true );
}

public void StartPlayer( )
{
	RJWard.Tube.SpinePoint_Base firstSpinePoint = GetFirstSpinePoint( );
	if (firstSpinePoint != null)
	{
		player.InitialiseAt( firstSpinePoint.transform );
		MessageBus.instance.dispatchPlayerRestarted( );
	}
	else
	{
		Debug.LogWarning( "Can't start player with no tube" );
	}
}
*/


	public void HandleDebugButtonPressed( )
	{
		MessageBus.instance.dispatchToggleDebugObjects( );
	}




	#region event handlers

	#endregion event handlers

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Game;
	}

	override protected void PostStart()
	{
	}

	override protected void PostAwake()
	{
	}

#endregion SceneController_Base

}

public partial class MessageBus : MonoBehaviour
{
	public System.Action toggleDebugObjectsAction;
	public void dispatchToggleDebugObjects( )
	{
		if (toggleDebugObjectsAction != null)
		{
			toggleDebugObjectsAction( );
		}
	}
}

