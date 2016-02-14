using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeSection : MonoBehaviour
	{
		public Spine spine = null;

		public static TubeSection CreateLinear( Vector3 start, Vector3? startRotation, Vector3 end, Vector3? endRotation, int num, float radius )
		{
			GameObject tsGo = new GameObject( "TubeSection" );
			TubeSection result = tsGo.AddComponent<TubeSection>( );

			GameObject spineGO = new GameObject( "Spine" );
			result.spine = spineGO.AddComponent<Spine>( );

			for (int i = 0; i < num; i++)
			{
				Vector3? rot = null;
				if (i == 0)
				{
					rot = startRotation;
				}
				else if (i == num-1)
				{
					rot = endRotation;
				}
				result.spine.AddSpinePoint( Vector3.Lerp(start, end, (float)i/(num-1) ) , rot, radius );
			}

			return result;
		}
	}

}
