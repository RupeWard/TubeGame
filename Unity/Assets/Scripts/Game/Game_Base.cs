using UnityEngine;
using System.Collections;

abstract public class Game_Base
{
	abstract public RJWard.Tube.TubeSectionDefinition_Linear GetNextTubeSectionDefn( );
	abstract public void Reset( );

}
