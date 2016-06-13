using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class SpinePointPool : RJWard.Core.Singleton.SingletonApplicationLifetimeLazy<SpinePointPool>, RJWard.Core.IDebugDescribable 
	{
		private List<SpinePoint_Linear> activeSpinePoints_ = new List<SpinePoint_Linear>( );
		private List<SpinePoint_Linear> inactiveSpinePoints_ = new List<SpinePoint_Linear>( );


		private void OnLevelWasLoaded(int l)
		{
			
		}

		public SpinePoint_Linear GetSpinePoint()
		{
			SpinePoint_Linear result = null;
			if (inactiveSpinePoints_.Count == 0)
			{
				result = inactiveSpinePoints_[0];
				inactiveSpinePoints_.RemoveAt( 0 );
				activeSpinePoints_.Add( result );
			}
			else
			{
				
			}
			return result;
		}
		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "[SP_pool: " + activeSpinePoints_.Count + " active, " + inactiveSpinePoints_.Count + " inactive" );
		}
	}

}
