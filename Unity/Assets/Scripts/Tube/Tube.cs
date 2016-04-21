﻿using UnityEngine;
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
			Debug.Log( "Appending to end" );
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
						lastHoopOfPrevious.spinePoint.fixRotation( );
						firstHoop.transform.rotation = Quaternion.identity;
						ts.ConnectAfterSpinePoint( lastHoopOfPrevious.spinePoint );
						yield return null;
						while (ts.isMeshDirty)
						{
							yield return null;
						}

						Debug.Log( "Moving to match prev section, rotation is "+ts.transform.rotation);				
						Vector3 prevHoopRot = lastHoopOfPrevious.spinePoint.transform.rotation.eulerAngles;

						ts.transform.rotation = lastHoopOfPrevious.spinePoint.transform.rotation; // SEEMS RIGHT
						yield return null;
//						ts.transform.rotation = Quaternion.Euler( lastHoopOfPrevious.spinePoint.transform.rotation.eulerAngles - firstHoop.spinePoint.transform.rotation.eulerAngles) * ts.transform.rotation;
						ts.transform.position = lastHoopOfPrevious.spinePoint.transform.position  ;

						// need to rotate ts so first hoop at same orientation as previous

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

		public void RecalculateInterpolatorsLinear( )
		{
			System.Text.StringBuilder debugsb = new System.Text.StringBuilder( );
			debugsb.Append( "Recalculating interpolators" );

			List<SpinePoint_Linear> spinePoints = new List<SpinePoint_Linear>( );
			List<Vector3> spinePointPositions = new List<Vector3>( );

			if (tubeSections_.Count > 0)
			{
				SpinePoint_Linear spinePoint = tubeSections_[0].FirstHoop().spinePoint;
				while (spinePoint != null)
				{
					spinePoints.Add( spinePoint );
					spinePointPositions.Add( spinePoint.transform.position );
					spinePoint = spinePoint.nextSpinePoint;
				}
			}
			int numSpinePoints = spinePoints.Count;
			debugsb.Append( "\nFound " + numSpinePoints + " spine points" );

			List<RJWard.Core.CatMullRom3D> spinePointInterpolators = new List<Core.CatMullRom3D>( );
			RJWard.Core.CatMullRom3D.InterpolateFixedNumCentripetal( spinePointPositions, 1, spinePointInterpolators );

			for (int i = 0; i < numSpinePoints; i++)
			{
				if (i == spinePointPositions.Count - 1)
				{
					spinePoints[i].forwardInterpolator = null;
					debugsb.Append( "\nNo interpolator for last point" );
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
			if (debugsb.Length >0)
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

