using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeSectionDefinition_Linear
	{
		static readonly bool DEBUG_LOCAL = false;

		static private int s_counter = 0;

		public TubeSectionDefinition_Linear()
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "Creating TubesectionDefinition_Linear #" + s_counter );
			}
			id_ = s_counter;
			s_counter++;
		}

		private int numHoopPoints_ = int.MaxValue;
		
		private List< HoopDefinition_Base > hoopDefns_ = new List< HoopDefinition_Base >( );
		private int id_;
		public int id {  get { return id_; } }

		public int NumSpinePoints
		{
			get { return hoopDefns_.Count;  }
		}

		public HoopDefinition_Base GetHoopDefn(int index)
		{
			HoopDefinition_Base result = null;
			if (index >= 0 && index < NumSpinePoints)
			{
				result = hoopDefns_[index];
			}
			else
			{
				Debug.LogError( "Can't get hoop #" + index + " from " + NumSpinePoints );
			}
			return result;
		}

		public void AddHoopDefn( HoopDefinition_Base hdb )
		{
			if (hoopDefns_.Count == 0)
			{
				numHoopPoints_ = hdb.numHoopPoints;
			}
			else
			{
				if (numHoopPoints_ != hdb.numHoopPoints)
				{
					Debug.LogError( "mismatch" );
				}
			}
			hoopDefns_.Add( hdb );
		}
	}

}
