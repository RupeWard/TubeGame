using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeFactory :  RJWard.Core.Singleton.SingletonSceneLifetime< TubeFactory>
	{
		public float flowZoneConvexAdjust = 0.2f;

		public GameObject tubeEndPrefab;
		public GameObject CreateTubeEnd()
		{
			return GameObject.Instantiate<GameObject>( tubeEndPrefab );
		}

		private int tubeWallLayerMask_ = 0;
		public int tubeWallLayerMask
		{
			get { return tubeWallLayerMask_; }
		}

		public int tubeWallLayer
		{
			get { return 1 << tubeWallLayerMask_; }
		}

		private int buildLayerMask_ = 0;
		public int buildLayerMask
		{
			get { return buildLayerMask_; }
		}

		public int buildLayer
		{
			get { return 1 << buildLayerMask_; }
		}

		protected override void PostAwake( )
		{
			tubeWallPhysics = Resources.Load( "PhysicsMaterials/TubeWallPhys" ) as PhysicMaterial;
			tubeWallLayerMask_ = LayerMask.NameToLayer( "TubeWall" );
			buildLayerMask_ = LayerMask.NameToLayer( "Build" );
		}

		public void SetLayerRecursively(GameObject g)
		{
			g.layer = tubeWallLayerMask_;
			foreach (Transform t in g.transform)
			{
				SetLayerRecursively( t.gameObject );
			}
		}

		public Material tubeWallMaterial;

		public PhysicMaterial tubeWallPhysics = null;

		public void CreateRandomLinearSection( Tube t, RandLinearSectionDefn settings, System.Action<TubeSection_Linear> onCreatedAction )
		{
			StartCoroutine( CreateRandomLinearSectionCR( t, settings, onCreatedAction ) );
		}

		public IEnumerator CreateRandomLinearSectionCR(Tube t, RandLinearSectionDefn settings, System.Action<TubeSection_Linear > onCreatedAction )
		{
			System.Text.StringBuilder sb = null;

			TubeSectionDefinition_Linear defn = CreateLinearSectionDefn( settings );
			
			yield return StartCoroutine( CreateSectionFromDefinitionCR(t, defn, settings.numHoopsPerSection, onCreatedAction, sb ) );
		}

		public TubeSectionDefinition_Linear CreateLinearSectionDefn( RandLinearSectionDefn settings)
		{
			System.Text.StringBuilder sb = null;

			if (TubeSection_Linear.DEBUG_MESH)
			{
				sb = new System.Text.StringBuilder( );
				sb.Append( "Creating Random Linear Section Defn\n from " ).DebugDescribe( settings );
			}
			if (settings.numSections < 1)
			{
				throw new System.InvalidOperationException( "No hoops" );
			}
			TubeSectionDefinition_Linear defn = new TubeSectionDefinition_Linear( );

			float radius = settings.initialRad;

			HoopDefinition_Base previous = null;
			if (settings.firstHoop != null)
			{
				defn.AddHoopDefn( settings.firstHoop );
				previous = settings.firstHoop;
				radius = settings.firstHoop.radius( );
				if (sb != null)
				{
					sb.Append( "\n COPIED first hoop defn :" ).DebugDescribe( settings.firstHoop );
				}
			}
			else
			{
				HoopDefinition_Circular hdc = new HoopDefinition_Circular( Vector3.zero, Vector3.forward, GameManager.Instance.numHoopPoints, radius );
				defn.AddHoopDefn( hdc );
				previous = hdc;
				if (sb != null)
				{
					sb.Append( "\n CREATED first hoop defn :" ).DebugDescribe( hdc );
				}
			}

			Vector3 direction = Vector3.forward;

			for (int i = 1; i < (settings.numSections+1); i++) // 1 more hoop than sections
			{
				Vector3 pos = previous.position + direction * settings.sectionSeparation;
				float radD = 0f;

				if (radius < settings.radRange.x)
				{
					radD = settings.maxRadD;
				}
				else if (radius > settings.radRange.y)
				{
					radD = -1f * settings.maxRadD;
				}
				else
				{
					radD = UnityEngine.Random.Range( -1f * settings.maxRadD, 1f * settings.maxRadD );
				}
				radius += radD;

				HoopDefinition_Circular hdcnew = new HoopDefinition_Circular( pos, null, GameManager.Instance.numHoopPoints, radius );
				defn.AddHoopDefn( hdcnew );
				previous = hdcnew;
				if (sb != null)
				{
					sb.Append( "\n created circular hoop defn " ).Append( i ).Append( ":" ).DebugDescribe( hdcnew );
				}
				float xAngle = UnityEngine.Random.Range( settings.xAngleChangeRange.x, settings.xAngleChangeRange.y );
				float yAngle = UnityEngine.Random.Range( settings.yAngleChangeRange.x, settings.yAngleChangeRange.y );

				Quaternion rot = Quaternion.Euler( new Vector3( xAngle, yAngle, 0f ) );
				direction = rot * direction;

			}
			if (sb != null)
			{
				Debug.Log( sb.ToString( ) );
			}
			return defn;
		}

		public IEnumerator CreateCircularCR(Tube t, string n, TubeSectionDefinition_Linear tsd, Material mat, System.Action< TubeSection_Linear > onCreatedAction )
		{
			if (tmpTubeSection_ != null)
			{
				Debug.LogError( "tmpTubeSection != null" );
			}
			GameObject tsGo = new GameObject( n );
			tmpTubeSection_ = tsGo.AddComponent<TubeSection_Linear>( );
			yield return StartCoroutine(tmpTubeSection_.InitCircularCR(t, n, tsd, mat ));
			yield return null;
			if (onCreatedAction != null)
			{
				onCreatedAction( tmpTubeSection_ );
			}
		}

		public IEnumerator CreateSplinarCR(Tube t, string n, TubeSection_Linear ts, int numPerSection, Material mat, System.Action<TubeSection_Linear> onCreatedAction )
		{
			if (tmpTubeSection_ != null)
			{
				Debug.LogError( "tmpTubeSection != null" );
			}
			GameObject tsGo = new GameObject( n );
			tmpTubeSection_= tsGo.AddComponent<TubeSection_Linear>( );
			yield return StartCoroutine(tmpTubeSection_.InitSplinarCR(t, n, ts, numPerSection, mat ));
			yield return null;
			if (onCreatedAction != null)
			{
				onCreatedAction( tmpTubeSection_ );
			}
		}

		private TubeSection_Linear tmpTubeSection_ = null;
		private int tsNumber = 0;

		public void CreateFromSourcesInContainer(Tube t, Transform container, int numHoopPoints, Material tubeWallMaterial, System.Action<TubeSection_Linear> onTubeSectionMadeAction )
		{

			StartCoroutine( CreateFromSourcesInContainerCR(t, container, numHoopPoints, tubeWallMaterial, onTubeSectionMadeAction ) );
		}

		private IEnumerator CreateSectionFromDefinitionCR(Tube t, TubeSectionDefinition_Linear defn, int numPerSection, System.Action<TubeSection_Linear> onTubeSectionMadeAction, System.Text.StringBuilder sb )
		{
			yield return null;

//			if (container != null)
			{
	
				TubeSectionDefinition_Linear tsd = new TubeSectionDefinition_Linear( );

				if (sb != null)
				{
					sb.Append( "Building Tube Section" );
				}

				for (int i = 0; i < defn.NumSpinePoints; i++)
				{
					HoopDefinition_Base hdb = defn.GetHoopDefn(i);
					if (sb != null)
					{
						sb.Append( "\n " ).Append( i ).Append( ": " ).DebugDescribe( hdb );
					}

					//					Vector3 pos = hdb.position;
					//				Vector3? rot = hdb.rotation;
					//			float rad = hdb.radius;
					//		HoopDefinition_Circular hdb = new HoopDefinition_Circular( pos, rot, numHoopPoints, rad );
					tsd.AddHoopDefn(hdb);
					
					if (sb != null)
					{
						sb.Append( "\n  Added as " ).DebugDescribe( hdb );
					}
				}

				yield return StartCoroutine(CreateCircularCR(t, "TS" + tsNumber.ToString( ), tsd, tubeWallMaterial, null ));
				//container.gameObject.SetActive( false );
				tsNumber++;

				if (tmpTubeSection_ != null)
				{
					yield return null;
					TubeSection_Linear firstTubeSection = tmpTubeSection_;
					tmpTubeSection_ = null;
					yield return StartCoroutine(CreateSplinarCR(t, "SPLINAR", firstTubeSection, numPerSection, tubeWallMaterial, null ));
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


		private IEnumerator CreateFromSourcesInContainerCR( Tube t, Transform container, int numHoopPoints, Material tubeWallMaterial, System.Action<TubeSection_Linear> onTubeSectionMadeAction )
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

				yield return StartCoroutine( CreateCircularCR(t, "TS" + tsNumber.ToString( ), tsd, tubeWallMaterial, null ) );
				container.gameObject.SetActive( false );
				tsNumber++;

				if (tmpTubeSection_ != null)
				{
					yield return null;
					TubeSection_Linear firstTubeSection = tmpTubeSection_;
					tmpTubeSection_ = null;
					yield return StartCoroutine( CreateSplinarCR( t, "SPLINAR", firstTubeSection, 5, tubeWallMaterial, null ) );
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
