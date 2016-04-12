using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class SpinePointDefinition : RJWard.Core.IDebugDescribable
	{
		private HoopDefinition_Base hoopDefn_ = null;
		public HoopDefinition_Base hoopDefn
		{
			get { return hoopDefn_; }
		}

		//		private Vector3 position_ = Vector3.zero;
		//		private Vector3? rotation_ = null;
		//		private float hoopRadius_ = 0f;
		//		private int numHoopPoints_ = 10;

		public Vector3 position
		{
			get { return hoopDefn_.position;  }
		}

		public Vector3? rotation
		{
			get { return hoopDefn_.rotation; }
		}

//		public float radius
//		{
//			get { return hoopRadius_;  }
//		}

		public int numHoopPoints
		{
			get { return hoopDefn_.numHoopPoints; }
		}

		public SpinePointDefinition( HoopDefinition_Base hdb )
		{
			hoopDefn_ = hdb;
		}

		private SpinePointDefinition( ) { }

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "SPD (" );
			hoopDefn_.DebugDescribe( sb );
			sb.Append( ")" );
		}

	}
}

