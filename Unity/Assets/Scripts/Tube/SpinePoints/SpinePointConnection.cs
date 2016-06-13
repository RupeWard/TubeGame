using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class SpinePointConnection :  RJWard.Core.IDebugDescribable
	{
		SpinePoint_Base startPoint_ = null;
		SpinePoint_Base endPoint_ = null;

		RJWard.Core.CatMullRom3D interpolator_ = null;

		public SpinePoint_Base startPoint
		{
			get { return startPoint_; }
		}

		public SpinePoint_Base endPoint
		{
			get { return endPoint_; }
		}

		private float totalDistance_ = -1f;
		public float TotalDistance
		{
			get
			{
				if (totalDistance_ == -1f)
				{
					ComputeTotalDistance( );
				}
				return totalDistance_;
			}
		}

		public SpinePointConnection(SpinePoint_Base start, SpinePoint_Base end)
		{
			startPoint_ = start;
			endPoint_ = end;
		}

		public SpinePointConnection( SpinePoint_Base start, SpinePoint_Base end, RJWard.Core.CatMullRom3D i )
		{
			startPoint_ = start;
			endPoint_ = end;
			SetInterpolator( i );
		}

		public void SetInterpolator(RJWard.Core.CatMullRom3D i)
		{
			interpolator_ = i;
			ComputeTotalDistance( );
		}

		static private readonly bool DEBUG_DISTANCE = false;
		static private int s_numSectionsForDistance = 10;
		private void ComputeTotalDistance()
		{
			System.Text.StringBuilder sb = null;
			if (DEBUG_DISTANCE)
			{
				sb = new System.Text.StringBuilder( );
				sb.Append( "Computing interpolated distance between "
					+startPoint.gameObject.name+" (" + startPoint.cachedTransform.position + ") and "
					+ endPoint.gameObject.name + " (" + endPoint_.cachedTransform.position +")");
			}
			if (interpolator_ == null)
			{
				float total = Vector3.Distance( startPoint_.cachedTransform.position, endPoint_.cachedTransform.position );
				if (sb != null)
				{
					sb.Append( "\nNO INTERPOLATOR, D= " + total + " (was " + totalDistance_ + ")" );
				}
				totalDistance_ = total;
				Debug.LogWarning( "No interpolator for " + this.DebugDescribe( ) );
			}
			else
			{
				List<Vector3> spinePointPositions = new List<Vector3>( );
				spinePointPositions.Add( startPoint.cachedTransform.position );
				float step = 1f / s_numSectionsForDistance;
				for (int i=1; i<s_numSectionsForDistance; i++)
				{
					spinePointPositions.Add( interpolator_.Interpolate( i * step ) );
				}
				spinePointPositions.Add( endPoint.cachedTransform.position );

				if (DEBUG_DISTANCE)
				{
					sb.Append( "\nInterpolated, now have " + spinePointPositions.Count + " positions" );
				}
				float total = 0f;
				for (int i = 0; i < (spinePointPositions.Count - 1); i++)
				{
					Vector3 sp0 = spinePointPositions[i];
					Vector3 sp1 = spinePointPositions[i + 1];
					float dist = Vector3.Distance( sp0, sp1 );
					if (DEBUG_DISTANCE)
					{
						sb.Append( "\n " + dist + " between " + sp0 + " and " + sp1 );
					}
					total += dist;
				}
				if (DEBUG_DISTANCE)
				{
					sb.Append( "\nTOTAL DIST = " + total + " (was " + totalDistance_ + ")" );
				}
				totalDistance_ = total;

			}
			if (DEBUG_DISTANCE && sb.Length >0)
			{
				Debug.Log( sb.ToString( ) );
			}
		}

		public Vector3 InterpolatePosition(float t)
		{
			if (t < 0f || t > 1f)
			{
				Debug.LogError( "InterpolateForward with t = " + t + " on connection "+this.DebugDescribe() );
			}
			t = Mathf.Clamp01( t );
			if (t == 0f)
			{
				return startPoint_.cachedTransform.position;
			}
			else if (t==1f)
			{
				return endPoint_.cachedTransform.position;
			}
			else
			{
				if (interpolator_ != null)
				{
					return interpolator_.Interpolate( t );
				}
			}
			return Vector3.Lerp( startPoint_.cachedTransform.position, endPoint_.cachedTransform.position, t );
		}

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "[SPConn: " );
			sb.Append( startPoint_.gameObject.name );
			sb.Append( "->" );
			sb.Append( endPoint_.gameObject.name );
			sb.Append( " D=" ).Append( totalDistance_ );
			sb.Append( "]" );
		}
	}

}
