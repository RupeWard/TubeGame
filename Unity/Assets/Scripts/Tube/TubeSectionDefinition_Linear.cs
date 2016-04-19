using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeSectionDefinition_Linear
	{
		static private int s_counter = 0;

		public TubeSectionDefinition_Linear()
		{
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
					Debug.LogError( "mismacth" );
				}
			}
			hoopDefns_.Add( hdb );
		}
	}
	/*
	public class TubeSectionDefinition_Linear
	{
		private List< SpinePointDefinition > spinePointDefns_ = new List< SpinePointDefinition >( );

		public int NumSpinePoints
		{
			get { return spinePointDefns_.Count;  }
		}

		public SpinePointDefinition GetSpinePointDefn(int index)
		{
			SpinePointDefinition result = null;
			if (index >= 0 && index < NumSpinePoints)
			{
				result = spinePointDefns_[index];
			}
			return result;
		}

		public void AddSpinePointDefn( SpinePointDefinition spd )
		{
			spinePointDefns_.Add( spd );
		}
	}
	*/

}
