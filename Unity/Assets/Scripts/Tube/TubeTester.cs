using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeTester: MonoBehaviour
	{

		// Use this for initialization
		void Start( )
		{
			GameObject spGo = new GameObject( "SpinePoint" );
			SpinePoint spinePoint = spGo.AddComponent< SpinePoint >( );
			spGo.transform.position = Vector3.zero;
			DebugBlob.AddToObject( spGo, 0.15f, Color.green );
			GameObject hoopGo = new GameObject( "Hoop" );
			Hoop hoop = hoopGo.AddComponent<Hoop>( );
			hoop.Init( spinePoint, 10, 1 );
			
		}

		// Update is called once per frame
		void Update( )
		{

		}
	}

}
