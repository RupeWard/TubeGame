using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	[System.Serializable]
	public class Game_Sequences: Game_Base
	{
		private static readonly bool DEBUG_LOCAL = true;

		[System.Serializable]
		public class Sequence
		{
			public int num;
			public RandLinearSectionDefn defn;
		}

		public Sequence[] sequences = new Sequence[1];

		public override RandLinearSectionDefn GetNextTubeSectionDefn( TubeSection_Linear ts )
		{
			if (currentSequence_ >= sequences.Length)
			{
				// return final one
				if (sequences.Length > 0)
				{
					if (DEBUG_LOCAL)
					{
						Debug.Log( "GS: providing FINAL DEFN as Seq = " + currentSequence_ +
							" (of " + sequences.Length + ")" );
					}
					return sequences[sequences.Length - 1].defn;
				}
				else
				{
					Debug.LogWarning( "No sequences! return default defn" );
					return new RandLinearSectionDefn( );
				}
			}
			else if (currentSequence_ == 0 )
			{
				if (currentNinSequence_ < sequences[0].num)
				{
					if (DEBUG_LOCAL)
					{
						Debug.Log( "GS: providing #"+currentNinSequence_+" of "+sequences[0].num+" in Seq = " + currentSequence_ +
							" (of " + sequences.Length + ")" );
					}
					currentNinSequence_++;
					if (currentNinSequence_ >= sequences[0].num)
					{
						currentNinSequence_ = 0;
						currentSequence_++;
						if (DEBUG_LOCAL)
						{
							Debug.Log( "GS: this was final case, moving on to sequence "+currentSequence_+" of " +sequences.Length);
						}
					}
					return sequences[0].defn;
				}
				else
				{
					Debug.LogWarning( "??? returning default" );
					return new RandLinearSectionDefn( );
				}
			}
			else
			{
				if (DEBUG_LOCAL)
				{
					Debug.Log( "GS: providing #" + currentNinSequence_ + " of " + sequences[currentSequence_].num + " in Seq = " + currentSequence_ +
						" (of " + sequences.Length + ")" );
				}

				RandLinearSectionDefn previousDefn = sequences[currentSequence_-1].defn;
				RandLinearSectionDefn targetDefn = sequences[currentSequence_].defn;

				float fraction = (float)currentNinSequence_ / sequences[currentSequence_].num;
				currentNinSequence_++;
				if (currentNinSequence_ >= sequences[currentSequence_].num)
				{
					currentNinSequence_ = 0;
					currentSequence_++;
					if (DEBUG_LOCAL)
					{
						Debug.Log( "GS: this was final case, moving on to sequence " + currentSequence_ + " of " + sequences.Length );
					}
				}

				return RandLinearSectionDefn.Lerp( previousDefn, targetDefn, fraction );
			}
		}

		private int currentSequence_ = 0;
		private int currentNinSequence_ = 0;

		public override void Reset( )
		{
			currentSequence_ = 0;
			currentNinSequence_ = 0;
		}

		public Game_Sequences()
		{
		}
	}
}

