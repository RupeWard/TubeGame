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

		public int numHoopPoints = 10;

		public Material tubeWallMaterial;

		public Transform testTubeContainer;

		private TubeSection tubeSection_ = null;

		// Use this for initialization
		void Start( )
		{
			StartCoroutine( CreateFromSourcesInContainerCR( testTubeContainer ) );

		}

		private int tsNumber = 0;

		private IEnumerator CreateFromSourcesInContainerCR(Transform container)
		{
			yield return null;
//			TubeSection TS = TubeSection.CreateLinear( pos1.position, null, pos2.position, null, num, startRadius, endRadius, tubeWallMaterial ); 

			if (testTubeContainer != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder( );

				TubeSectionDefinition tsd = new TubeSectionDefinition( );

				sb.Append( "Building Tube Section" );

                SpinePointSource[] spinePointSources = container.GetComponentsInChildren<SpinePointSource>( );
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

					SpinePointDefinition spd = new SpinePointDefinition( pos, rot, numHoopPoints, rad );
					tsd.AddSpinePointDefn( spd );

					sb.Append( "\n  Added as " ).DebugDescribe( spd );
				}

				Debug.Log( sb.ToString( ) );

				tubeSection_ = TubeSection.CreateCircular("TS"+tsNumber.ToString(), tsd, tubeWallMaterial );
				testTubeContainer.gameObject.SetActive( false );
				tsNumber++;
			}

			for (int i = 4; i>-1; i--)
			{
				Debug.Log( "Waiting for " + i );
				yield return new WaitForSeconds( 1f );
			}
			TubeSection newTs = TubeSection.CreateSplinar( "SPLINAR", tubeSection_, 5, tubeWallMaterial );
		}

		// Update is called once per frame
		void Update( )
		{

		}
	}

}
