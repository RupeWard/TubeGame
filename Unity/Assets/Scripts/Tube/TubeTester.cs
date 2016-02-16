using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeTester: MonoBehaviour
	{
		public Transform pos1 = null;
		public Transform pos2 = null;
		public int num = 5;
		public float startRadius = 5f;
		public float endRadius = 8f;

		public Material tubeWallMaterial;

		public Transform testTubeContainer;

		// Use this for initialization
		void Start( )
		{
			StartCoroutine( TestCR( ) );

		}

		private IEnumerator TestCR()
		{
			yield return null;
//			TubeSection TS = TubeSection.CreateLinear( pos1.position, null, pos2.position, null, num, startRadius, endRadius, tubeWallMaterial ); 

			if (testTubeContainer != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder( );

				TubeSectionDefinition tsd = new TubeSectionDefinition( );

				sb.Append( "Building Tube Section" );

                SpinePointSource[] spinePointSources = testTubeContainer.GetComponentsInChildren<SpinePointSource>( );
				sb.Append( "\nFound " ).Append( spinePointSources.Length ).Append( " sources" );

				for (int i = 0; i < spinePointSources.Length; i++)
				{
					SpinePointSource sps = spinePointSources[i];
					sb.Append( "\n " ).Append( i ).Append(": ").DebugDescribe(sps );

					Vector3 pos = sps.transform.position;
					Vector3? rot = null;
					if (sps.fixRotation)
					{
						rot = sps.transform.rotation.eulerAngles;
					}
					float rad = sps.radius;

					SpinePointDefinition spd = new SpinePointDefinition( pos, rot, rad );
					tsd.AddSpinePointDefn( spd );

					sb.Append( "\n  Added as " ).DebugDescribe( spd );
				}

				Debug.Log( sb.ToString( ) );

				TubeSection TS = TubeSection.Create( tsd, tubeWallMaterial );
				testTubeContainer.gameObject.SetActive( false );
			}
		}

		// Update is called once per frame
		void Update( )
		{

		}
	}

}
