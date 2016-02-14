using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeSection : MonoBehaviour
	{
		public Spine spine = null;

		public static TubeSection CreateLinear( Vector3 start, Vector3 end, int num, float radius )
		{
			GameObject tsGo = new GameObject( "TubeSection" );
			TubeSection result = tsGo.AddComponent<TubeSection>( );

			GameObject spineGO = new GameObject( "Spine" );
			result.spine = spineGO.AddComponent<Spine>( );

			for (int i = 0; i < num; i++)
			{
				result.spine.AddSpinePoint( Vector3.Lerp(start, end, (float)i/(num-1) ) , new Vector3( 0f, 0f, 1f ), radius );
			}

			return result;
		}
	}

}
