using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class HoopDefinition_Circular : HoopDefinition_Base
	{
		private float hoopRadius_ = 0f;

		public float radius
		{
			get { return hoopRadius_;  }
		}

		public HoopDefinition_Circular( Vector3 p, Vector3? ro, int nh, float ra )
			: base( p, ro, nh )
		{
			hoopRadius_ = ra;
		}

		private HoopDefinition_Circular( )  { }

		protected override void DebugDescribeDetails(System.Text.StringBuilder sb)
		{
			sb.Append( "CIRC,R=" ).Append( radius );
		}


		override public void AddToSpine( Spine sp, bool fixedRotation )
		{
			sp.AddCircularSpinePoint( this, fixedRotation );
		}

	}
}

