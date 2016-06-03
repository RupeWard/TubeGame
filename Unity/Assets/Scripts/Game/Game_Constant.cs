using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class Game_Constant : Game_Base
	{
		private TubeFactory.RandLinearSectionDefn sectionDefn_ = new TubeFactory.RandLinearSectionDefn( );

		public override TubeFactory.RandLinearSectionDefn GetNextTubeSectionDefn(  )
		{
			return sectionDefn_;
		}

		public override void Reset( )
		{
		}

		public Game_Constant(TubeFactory.RandLinearSectionDefn sd )
		{
			sectionDefn_ = sd;
		}
	}
}

