using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	abstract public class HoopDefinition_Base : RJWard.Core.IDebugDescribable
	{
		private Vector3 position_ = Vector3.zero;
		private Vector3? rotation_ = null;
		
		private int numHoopPoints_ = 10;

		public Vector3 position
		{
			get { return position_;  }
		}

		public Vector3? rotation
		{
			get { return rotation_; }
		}

		public int numHoopPoints
		{
			get { return numHoopPoints_; }
		}

		public HoopDefinition_Base( Vector3 p, Vector3? ro, int nh)
		{
			position_ = p;
			rotation_ = ro;
			numHoopPoints_ = nh;
			if (numHoopPoints_ == 0)
			{
				Debug.LogError( "no hp" );
			}
		}

		protected HoopDefinition_Base( ) { }

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "[HOOP @" ).Append( position ).Append( " R=" );
			if (rotation != null)
			{
				sb.Append( rotation );
			}
			else
			{
				sb.Append( "-" );
			}
			sb.Append( " N=" ).Append( numHoopPoints_ );
			DebugDescribeDetails( sb );
			sb.Append( "]" );
		}

		abstract protected void DebugDescribeDetails( System.Text.StringBuilder sb );

		abstract public void AddToSpine( Spine_Linear sp, bool fixedRotation );
	}
}

