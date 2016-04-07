using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeFactory :  RJWard.Core.Singleton.SingletonApplicationLifetimeLazy< TubeFactory>
	{

		public IEnumerator CreateCircular( string n, TubeSectionDefinition tsd, Material mat, System.Action< TubeSection > onCreatedAction )
		{
			if (tmpTubeSection_ != null)
			{
				Debug.LogError( "tmpTubeSection != null" );
			}
			GameObject tsGo = new GameObject( n );
			tmpTubeSection_ = tsGo.AddComponent<TubeSection>( );
			tmpTubeSection_.InitCircular( n, tsd, mat );// TOD CR
			yield return null;
			if (onCreatedAction != null)
			{
				onCreatedAction( tmpTubeSection_ );
			}
		}

		public IEnumerator CreateSplinar( string n, TubeSection ts, int numPerSection, Material mat, System.Action<TubeSection> onCreatedAction )
		{
			if (tmpTubeSection_ != null)
			{
				Debug.LogError( "tmpTubeSection != null" );
			}
			GameObject tsGo = new GameObject( n );
			tmpTubeSection_= tsGo.AddComponent<TubeSection>( );
			yield return StartCoroutine(tmpTubeSection_.InitSplinarCR( n, ts, numPerSection, mat )); // TOD CR
			yield return null;
			if (onCreatedAction != null)
			{
				onCreatedAction( tmpTubeSection_ );
			}
		}

		private TubeSection tmpTubeSection_ = null;
		private int tsNumber = 0;

		public void CreateFromSourcesInContainer( Transform container, int numHoopPoints, Material tubeWallMaterial, System.Action<TubeSection> onTubeSectionMadeAction )
		{
			StartCoroutine( CreateFromSourcesInContainerCR( container, numHoopPoints, tubeWallMaterial, onTubeSectionMadeAction ) );
		}

        private IEnumerator CreateFromSourcesInContainerCR(Transform container, int numHoopPoints, Material tubeWallMaterial, System.Action<TubeSection> onTubeSectionMadeAction )
		{
			yield return null;

			if (container != null)
			{
				container.gameObject.SetActive( true );

				System.Text.StringBuilder sb = new System.Text.StringBuilder( );

				TubeSectionDefinition tsd = new TubeSectionDefinition( );

				sb.Append( "Building Tube Section" );

				SpinePointSource[] spinePointSources = container.GetComponentsInChildren<SpinePointSource>( );
				sb.Append( "\nFound " ).Append( spinePointSources.Length ).Append( " sources" );

				for (int i = 0; i < spinePointSources.Length; i++)
				{
					SpinePointSource sps = spinePointSources[i];
					sb.Append( "\n " ).Append( i ).Append( ": " ).DebugDescribe( sps );

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

				yield return StartCoroutine(CreateCircular( "TS" + tsNumber.ToString( ), tsd, tubeWallMaterial, null ));
				container.gameObject.SetActive( false );
				tsNumber++;

				if (tmpTubeSection_ != null)
				{
					yield return null;
					TubeSection firstTubeSection = tmpTubeSection_;
					tmpTubeSection_ = null;
					yield return StartCoroutine(CreateSplinar( "SPLINAR", firstTubeSection, 5, tubeWallMaterial, null ));
					GameObject.Destroy( firstTubeSection.gameObject );
					yield return null;
					if (tmpTubeSection_ != null)
					{
						if (onTubeSectionMadeAction != null)
						{
							onTubeSectionMadeAction( tmpTubeSection_ );
						}
						tmpTubeSection_ = null;
					}
					else
					{
						Debug.LogError( "No TS after CreateSplinar" );
					}
				}
				else
				{
					Debug.LogError( "No TS after CreateCircular" );
				}
			}

		}
		


	}
}
