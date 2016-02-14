using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class Spine : MonoBehaviour
	{
		private List< SpinePoint > spinePoints_ = new List< SpinePoint >( );

		public void AddSpinePoint( Vector3 pos, Vector3 rot )
		{
			GameObject spGo = new GameObject( "SP_"+spinePoints_.Count.ToString() );
			SpinePoint spinePoint = spGo.AddComponent<SpinePoint>( );
			spGo.transform.parent = this.transform;
			spGo.transform.localPosition = pos;
			spGo.transform.localRotation = Quaternion.Euler( rot );

			spinePoints_.Add( spinePoint );

			DebugBlob.AddToObject( spGo, 0.15f, Color.green );
			GameObject hoopGo = new GameObject( "Hoop" );
			Hoop hoop = hoopGo.AddComponent<Hoop>( );
			hoop.Init( spinePoint, 10, 1 );

		}

	}


}
