using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class SpinePointDefinition : RJWard.Core.IDebugDescribable
	{
		private Vector3 position_ = Vector3.zero;
		private Vector3? rotation_ = null;
		private float radius_ = 0f;

		public Vector3 position
		{
			get { return position_;  }
		}

		public Vector3? rotation
		{
			get { return rotation_; }
		}

		public float radius
		{
			get { return radius_;  }
		}

		public SpinePointDefinition( Vector3 p, Vector3? ro, float ra )
		{
			position_ = p;
			rotation_ = ro;
			radius_ = ra;
		}

		private SpinePointDefinition( ) { }

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "SPD @" ).Append( position ).Append( " RO=" );
			if (rotation != null)
			{
				sb.Append( rotation );
			}
			else
			{
				sb.Append( "free" );
			}
			sb.Append( " RA=" ).Append( radius );
		}

	}
}

