using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class Game_Constant : Game_Base
	{
		public RandLinearSectionDefn sectionDefn = new RandLinearSectionDefn( );
		
		public override RandLinearSectionDefn GetNextTubeSectionDefn( TubeSection_Linear ts )
		{
			return sectionDefn;
		}

		public override void Reset( )
		{
		}

		public Game_Constant(RandLinearSectionDefn sd )
		{
			sectionDefn = sd;
		}
	}
}

