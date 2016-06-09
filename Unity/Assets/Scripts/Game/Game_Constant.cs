using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class Game_Constant : Game_Base
	{
		private RandLinearSectionDefn sectionDefn_ = new RandLinearSectionDefn( );

		public override RandLinearSectionDefn GetNextTubeSectionDefn(  )
		{
			return sectionDefn_;
		}

		public override void Reset( )
		{
		}

		public Game_Constant(RandLinearSectionDefn sd )
		{
			sectionDefn_ = sd;
		}
	}
}

