using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeFactory :  RJWard.Core.Singleton.SingletonApplicationLifetimeLazy< TubeFactory>
	{
		[System.Serializable]
		public class RandLinearSectionDefn : RJWard.Core.IDebugDescribable
		{
			public int numHoops = 10;
			public float separation = 3f;
			public float maxAngleD = 10f;
			public float initialRad = 1f;
			public Vector2 radRange = new Vector2( 0.5f, 3f);
			public float maxRadD = 0.5f;
			public int numHoopPoints = 10;

			public void DebugDescribe(System.Text.StringBuilder sb)
			{
				sb.Append( "[RLSD: n=" ).Append( numHoops ).Append( "x" ).Append( numHoopPoints );
				sb.Append( " s=" ).Append( separation );
				sb.Append( "]" );
			}
        }

		public Material tubeWallMaterial;


		public void CreateRandomLinearSection( RandLinearSectionDefn settings, System.Action<TubeSection_Linear> onCreatedAction )
		{
			StartCoroutine( CreateRandomLinearSectionCR( settings, onCreatedAction ) );
		}

		public IEnumerator CreateRandomLinearSectionCR( RandLinearSectionDefn settings, System.Action<TubeSection_Linear > onCreatedAction )
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder( );
			sb.Append( "Creating Random Linear Section \n from " ).DebugDescribe( settings );
			if (settings.numHoops < 1)
			{
				throw new System.InvalidOperationException( "No hoops" );
			}
			TubeSectionDefinition_Linear defn = new TubeSectionDefinition_Linear( );

			float radius = settings.initialRad;

			HoopDefinition_Circular hdc = new HoopDefinition_Circular( Vector3.zero, null, settings.numHoopPoints, radius );
			defn.AddHoopDefn( hdc );
			yield return null;

			sb.Append( "\n created first hoop defn :" ).DebugDescribe( hdc );
			Vector3 direction = Vector3.forward;

			HoopDefinition_Circular previous = hdc;
			for (int i = 1; i < settings.numHoops; i++)
			{
				Vector3 pos = previous.position + direction * settings.separation;
				float radD = UnityEngine.Random.Range( -1f * settings.maxRadD, 1f * settings.maxRadD );
				radius += radD;

				HoopDefinition_Circular hdcnew = new HoopDefinition_Circular( pos, null, settings.numHoopPoints, radius );
				defn.AddHoopDefn( hdcnew );
				previous = hdcnew;
				sb.Append( "\n created hoop defn ").Append(i).Append(":" ).DebugDescribe( hdc );
			}
			yield return StartCoroutine( CreateSectionFromDefinitionCR( defn, onCreatedAction, sb ) );
			Debug.Log( sb.ToString( ) );
		}


		public IEnumerator CreateCircular( string n, TubeSectionDefinition_Linear tsd, Material mat, System.Action< TubeSection_Linear > onCreatedAction )
		{
			if (tmpTubeSection_ != null)
			{
				Debug.LogError( "tmpTubeSection != null" );
			}
			GameObject tsGo = new GameObject( n );
			tmpTubeSection_ = tsGo.AddComponent<TubeSection_Linear>( );
			tmpTubeSection_.InitCircular( n, tsd, mat );// TOD CR
			yield return null;
			if (onCreatedAction != null)
			{
				onCreatedAction( tmpTubeSection_ );
			}
		}

		public IEnumerator CreateSplinar( string n, TubeSection_Linear ts, int numPerSection, Material mat, System.Action<TubeSection_Linear> onCreatedAction )
		{
			if (tmpTubeSection_ != null)
			{
				Debug.LogError( "tmpTubeSection != null" );
			}
			GameObject tsGo = new GameObject( n );
			tmpTubeSection_= tsGo.AddComponent<TubeSection_Linear>( );
			yield return StartCoroutine(tmpTubeSection_.InitSplinarCR( n, ts, numPerSection, mat )); // TOD CR
			yield return null;
			if (onCreatedAction != null)
			{
				onCreatedAction( tmpTubeSection_ );
			}
		}

		private TubeSection_Linear tmpTubeSection_ = null;
		private int tsNumber = 0;

		public void CreateFromSourcesInContainer( Transform container, int numHoopPoints, Material tubeWallMaterial, System.Action<TubeSection_Linear> onTubeSectionMadeAction )
		{

			StartCoroutine( CreateFromSourcesInContainerCR( container, numHoopPoints, tubeWallMaterial, onTubeSectionMadeAction ) );
		}

		private IEnumerator CreateSectionFromDefinitionCR( TubeSectionDefinition_Linear defn, System.Action<TubeSection_Linear> onTubeSectionMadeAction, System.Text.StringBuilder sb )
		{
			yield return null;

//			if (container != null)
			{
	
				TubeSectionDefinition_Linear tsd = new TubeSectionDefinition_Linear( );

				sb.Append( "Building Tube Section" );

				for (int i = 0; i < defn.NumSpinePoints; i++)
				{
					HoopDefinition_Base hdb = defn.GetHoopDefn(i);
					sb.Append( "\n " ).Append( i ).Append( ": " ).DebugDescribe( hdb );

//					Vector3 pos = hdb.position;
	//				Vector3? rot = hdb.rotation;
		//			float rad = hdb.radius;
			//		HoopDefinition_Circular hdb = new HoopDefinition_Circular( pos, rot, numHoopPoints, rad );
                    tsd.AddHoopDefn(hdb);
					
					sb.Append( "\n  Added as " ).DebugDescribe( hdb );
				}

				yield return StartCoroutine(CreateCircular( "TS" + tsNumber.ToString( ), tsd, tubeWallMaterial, null ));
				//container.gameObject.SetActive( false );
				tsNumber++;

				if (tmpTubeSection_ != null)
				{
					yield return null;
					TubeSection_Linear firstTubeSection = tmpTubeSection_;
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


		private IEnumerator CreateFromSourcesInContainerCR( Transform container, int numHoopPoints, Material tubeWallMaterial, System.Action<TubeSection_Linear> onTubeSectionMadeAction )
		{
			yield return null;

			if (container != null)
			{
				container.gameObject.SetActive( true );

				System.Text.StringBuilder sb = new System.Text.StringBuilder( );

				TubeSectionDefinition_Linear tsd = new TubeSectionDefinition_Linear( );

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
					HoopDefinition_Circular hdb = new HoopDefinition_Circular( pos, rot, numHoopPoints, rad );
					tsd.AddHoopDefn( hdb );

					sb.Append( "\n  Added as " ).DebugDescribe( hdb );
				}

				Debug.Log( sb.ToString( ) );

				yield return StartCoroutine( CreateCircular( "TS" + tsNumber.ToString( ), tsd, tubeWallMaterial, null ) );
				container.gameObject.SetActive( false );
				tsNumber++;

				if (tmpTubeSection_ != null)
				{
					yield return null;
					TubeSection_Linear firstTubeSection = tmpTubeSection_;
					tmpTubeSection_ = null;
					yield return StartCoroutine( CreateSplinar( "SPLINAR", firstTubeSection, 5, tubeWallMaterial, null ) );
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
