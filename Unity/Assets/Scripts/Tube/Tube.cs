using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{

	public class Tube : MonoBehaviour
	{
		public Material defaultWallMaterial_;

		public List<TubeSection_Linear> tubeSections_ = new List<TubeSection_Linear>();

		public void DeleteAllSections()
		{
			for( int i = 0; i<tubeSections_.Count; i++)
			{
				GameObject.Destroy( tubeSections_[i].gameObject );
			}
			tubeSections_.Clear( );
		}

		public SpinePoint_Base FirstSpinePoint()
		{
			SpinePoint_Base result = null;
			Hoop h = FirstHoop( );
			if (h != null)
			{
				result = h.spinePoint;
			}
			return result;
		}

		public SpinePoint_Base LastSpinePoint( )
		{
			SpinePoint_Base result = null;
			Hoop h = LastHoop( );
			if (h != null)
			{
				result = h.spinePoint;
			}
			return result;
		}


		public Hoop FirstHoop( )
		{
			Hoop result = null;
			TubeSection_Linear ts = FirstSection( );
			if (ts != null)
			{
				result = ts.FirstHoop( );
			}
			return result;
		}

		public Hoop LastHoop()
		{
			Hoop result = null;
			TubeSection_Linear ts = LastSection( );
			if (ts != null)
			{
				result = ts.LastHoop( );
			}
			return result;
		}

		public TubeSection_Linear FirstSection( )
		{
			TubeSection_Linear result = null;
			if (tubeSections_.Count > 0)
			{
				result = tubeSections_[0];
			}
			return result;
		}

		public TubeSection_Linear LastSection()
		{
			TubeSection_Linear result = null;
			if (tubeSections_.Count > 0)
			{
				result = tubeSections_[tubeSections_.Count - 1];
			}
			return result;
		}

		public void AddToEnd( TubeSection_Linear ts )
		{
			StartCoroutine( AddToEndCR( ts ) );
		}

		public IEnumerator AddToEndCR(TubeSection_Linear ts)
		{
//			Debug.Log( "Appending to end" );
			ts.transform.parent = transform;
			ts.transform.localScale = Vector3.one;
			ts.transform.localPosition = Vector3.zero;
			ts.transform.localRotation = Quaternion.identity;
			yield return null;
			ts.SetMeshDirty( true);
			while (ts.isMeshDirty)
			{
				yield return null;
			}
			float waitSeconds = 0f;
			if (waitSeconds > 0f)
			{
				Debug.Log( "Wait" + waitSeconds );
				yield return new WaitForSeconds( waitSeconds );
				Debug.Log( "Done" );
			}
			if (tubeSections_.Count > 0)
			{
				TubeSection_Linear lastSection = tubeSections_[tubeSections_.Count - 1];
                Hoop lastHoopOfPrevious = lastSection.LastHoop( );
				if (lastHoopOfPrevious == null)
				{
					Debug.LogWarning( "No last hoop of previous" );
				}
				else
				{
					Hoop firstHoop = ts.FirstHoop( );

					if (firstHoop == null)
					{
						Debug.LogWarning( "No firstHoop" );
					}
					else
					{
						lastHoopOfPrevious.spinePoint.fixRotation( true);
						firstHoop.transform.rotation = Quaternion.identity;

						// TODO multi
						if (lastHoopOfPrevious.spinePoint != null && lastHoopOfPrevious.spinePoint as SpinePoint_Linear == null)
						{
							Debug.Log( "Non-Simple SPinePoint not implemented" );
						}
						ts.ConnectAfterSpinePoint( lastHoopOfPrevious.spinePoint as SpinePoint_Linear);
						yield return null;
						while (ts.isMeshDirty)
						{
							yield return null;
						}

//						Debug.Log( "Moving to match prev section, rotation is "+ts.transform.rotation);				

						ts.transform.rotation = lastHoopOfPrevious.spinePoint.transform.rotation; // SEEMS RIGHT
						yield return null;
						ts.transform.position = lastHoopOfPrevious.spinePoint.transform.position  ;

						ts.FirstHoop( ).spinePoint.fixRotation( false );


						// This is a bit of hack to make sure the spine points get rotated properly.
						// Without it, SP_1 doesn't get properly rotated in its new position. 
						SpinePoint_Linear spinePoint = FirstHoop( ).spinePoint as SpinePoint_Linear;
						if (spinePoint != null)
						{
							while (spinePoint != null)
							{
								spinePoint.SetRotationDirty( );
								spinePoint = spinePoint.nextSpinePoint;
							}
							yield return null;
						}
						else
						{
							throw new System.Exception( "Not implemented spinepoint type " + FirstHoop( ).spinePoint.GetType( ).ToString( ) );
						}

//						Debug.Log( "Rotated by " + lastHoopOfPrevious.transform.rotation +" to give "+ ts.transform.rotation );
						//						ts.transform.rotation = Quaternion.Euler(-1f*firstHoopRot) * ts.transform.rotation;
						//						ts.transform.Translate( lastHoopOfPrevious.transform.position - firstHoop.transform.position );

					}

				}
				if (waitSeconds > 0f)
				{
					Debug.Log( "Wait" + waitSeconds );
					yield return new WaitForSeconds( waitSeconds );
					Debug.Log( "Done" );
				}
			}
			tubeSections_.Add( ts );
			yield return null;
			RecalculateInterpolatorsLinear( );
		}

		private static bool DEBUG_INTERPOLATORS = false;

		public void RecalculateInterpolatorsLinear( )
		{
			System.Text.StringBuilder debugsb = null;

			if (DEBUG_INTERPOLATORS)
			{
				debugsb = new System.Text.StringBuilder( );
				debugsb.Append( "Recalculating interpolators" );
			}

			List<SpinePoint_Linear> spinePoints = new List<SpinePoint_Linear>( );
			List<Vector3> spinePointPositions = new List<Vector3>( );

			if (tubeSections_.Count > 0)
			{
				SpinePoint_Base spb = tubeSections_[0].FirstHoop( ).spinePoint;
				SpinePoint_Linear spinePoint = spb as SpinePoint_Linear;
				while (spb != null)
				{ 
                    spinePoints.Add( spinePoint );
					spinePointPositions.Add( spinePoint.transform.position );
					spinePoint = spinePoint.nextSpinePoint;
					spb = spinePoint;
				}
			}
			int numSpinePoints = spinePoints.Count;
			if (debugsb != null)
			{
				debugsb.Append( "\nFound " + numSpinePoints + " spine points" );
			}

			List<RJWard.Core.CatMullRom3D> spinePointInterpolators = new List<Core.CatMullRom3D>( );
			RJWard.Core.CatMullRom3D.InterpolateFixedNumCentripetal( spinePointPositions, 1, spinePointInterpolators );

			for (int i = 0; i < numSpinePoints; i++)
			{
				if (i == spinePointPositions.Count - 1)
				{
					spinePoints[i].forwardInterpolator = null;
					if (debugsb != null)
					{
						debugsb.Append( "\nNo interpolator for last point" );
					}
				}
				else
				{
					if (i >= spinePointInterpolators.Count)
					{
						Debug.LogWarning( "Can't set forward interpolator for index " + i);
					}
					else
					{
						spinePoints[i].forwardInterpolator = spinePointInterpolators[i];
//						SpinePointConnection connection = new SpinePointConnection( spinePoints[i], spinePoints[i + 1], spinePointInterpolators[i] );
					}
					if (i> 0)
					{
						spinePoints[i].backInterpolator = spinePointInterpolators[i - 1];
					}
					else
					{
//						Debug.LogWarning( "Can't set back interpolator for index " + i );
						//									Debug.LogWarning( "Can't set back interpolator for first point" );
					}
				}
			}
			if (debugsb != null && debugsb.Length >0)
			{
				Debug.Log( debugsb.ToString( ) );
			}
		}


		// Use this for initialization
		void Start( )
		{

		}

		// Update is called once per frame
		void Update( )
		{

		}
	}

}

