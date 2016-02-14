using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeTester: MonoBehaviour
	{

		// Use this for initialization
		void Start( )
		{
			StartCoroutine( TestCR( ) );

		}

		private IEnumerator TestCR()
		{
			GameObject spineGO = new GameObject( "Spine" );
			Spine spine = spineGO.AddComponent<Spine>( );
			yield return null;

			for (int i = 0; i < 5; i++)
			{
				spine.AddSpinePoint( new Vector3( 0f, 0f, i * 0.5f ), new Vector3( 0f, 0f, 1f ) );
				yield return null;
			}
		}

		// Update is called once per frame
		void Update( )
		{

		}
	}

}
