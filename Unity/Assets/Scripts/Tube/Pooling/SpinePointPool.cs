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

		public SpinePoint_Linear GetSpinePoint(string n)
		{
			SpinePoint_Linear result = null;
			if (inactiveSpinePoints_.Count == 0)
			{
				result = inactiveSpinePoints_[0];
				result.gameObject.name = n;
				inactiveSpinePoints_.RemoveAt( 0 );
				activeSpinePoints_.Add( result );
			}
			else
			{
				GameObject go = new GameObject( n );
				result = go.AddComponent<SpinePoint_Linear>( );
				activeSpinePoints_.Add( result );
			}
			return result;
		}

		public void DeactivateSpinePoint(SpinePoint_Linear sp)
		{
#if UNITY_EDITOR
			if (!activeSpinePoints_.Contains(sp))
			{
				Debug.LogError( "Deactivating inactive spinepoint " );
			}
#endif
			activeSpinePoints_.Remove( sp );
#if UNITY_EDITOR
			if (inactiveSpinePoints_.Contains( sp ))
			{
				Debug.LogError( "Deactivating already inactive spinepoint " );
			}
#endif
			inactiveSpinePoints_.Add( sp );
		}

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "[SP_pool: " + activeSpinePoints_.Count + " active, " + inactiveSpinePoints_.Count + " inactive" );
		}
	}

}
