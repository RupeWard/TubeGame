using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeSectionDefinition
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
	/*
	public class TubeSectionDefinition
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
