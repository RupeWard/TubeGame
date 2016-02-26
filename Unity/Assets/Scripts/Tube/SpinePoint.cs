using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class SpinePoint : MonoBehaviour, RJWard.Core.IDebugDescribable
	{
		private Spine spine_ = null;

		private Hoop hoop_ = null;
		public Hoop hoop
		{
			get { return hoop_; }
		}

//		private float radius_ = 0f;

		private bool fixedRotation_ = false;

		public void SetDirty()
		{
			spine_.SetDirty( );
		}

		private SpinePoint nextSpinePoint_ = null;
		public SpinePoint nextSpinePoint
		{
			get { return nextSpinePoint_;  }
			set
			{
				if (value != nextSpinePoint_)
				{
					SetRotationDirty();
				}
				nextSpinePoint_ = value;
				if (nextSpinePoint_ != null)
				{
					nextSpinePoint_.SetRotationDirty( );
					if (nextSpinePoint_.nextSpinePoint != null)
					{
						nextSpinePoint_.nextSpinePoint.SetRotationDirty( );
					}
				}
				else
				{
					Debug.LogWarning( "Why setting nextSpinePoint to null?" );
				}
				if (previousSpinePoint_ != null)
				{
					previousSpinePoint_.SetRotationDirty( );
				}
			}
		}

		private SpinePoint previousSpinePoint_ = null;
		public SpinePoint previousSpinePoint
		{
			get { return previousSpinePoint_;  }
			set
			{
				if (value != previousSpinePoint_)
				{
					SetRotationDirty();
				}
				previousSpinePoint_ = value;
				if (previousSpinePoint_ != null)
				{
					previousSpinePoint_.SetRotationDirty( );
					if (previousSpinePoint_.previousSpinePoint != null)
					{
						previousSpinePoint_.previousSpinePoint.SetRotationDirty( );
					}
				}
				else
				{
					Debug.LogWarning( "Why setting previousSpinePoint to null?" );
				}
				if (nextSpinePoint_ != null)
				{
					nextSpinePoint_.SetRotationDirty( );
				}
			}
		}

		public RJWard.Core.CatMullRom3D forwardInterpolator = null;
		public RJWard.Core.CatMullRom3D backInterpolator = null;
		
		public bool InterpolateForwardWorld(float t, ref Vector3 result)
		{
			bool success = false;
			result = Vector3.zero;

			if (t < 0f || t > 1f)
			{
				Debug.LogWarning( "t = " + t );
			}
			t = Mathf.Clamp01( t );

			if (false)// (forwardInterpolator != null)
			{
				if (nextSpinePoint_ == null)
				{
					Debug.LogWarning( "Spine pt has a forward interpolator but no next point!" );
				}
				result = forwardInterpolator.Interpolate( t );
				success = true;
			}
			else
			{
				if (nextSpinePoint_ == null)
				{
					Debug.LogWarning( "Spine pt has no forward interpolator and no next point, can't interpolate!" );
                }
				else
				{
					result = Vector3.Lerp( transform.position, nextSpinePoint_.transform.position, t );
					success = true;
				}

			}
			return success;
		}

		public bool InterpolateBackwardWorld( float t, out Vector3 result )
		{
			bool success = false;
			result = Vector3.zero;

			if (t < 0f || t > 1f)
			{
				Debug.LogWarning( "t = " + t );
			}
			t = Mathf.Clamp01( t );

			if (backInterpolator != null)
			{
				if (previousSpinePoint_ == null)
				{
					Debug.LogWarning( "Spine pt has a back interpolator but no prev point!" );
                }
				result = backInterpolator.Interpolate( 1-t );
				success = true;
			}
			else
			{
				if (previousSpinePoint_ == null)
				{
					Debug.LogWarning( "Spine pt has no forward interpolator and no next point, can't interpolate!" );
                }
				else
				{
					result = Vector3.Lerp( transform.position, previousSpinePoint_.transform.position, t );
					success = true;
				}

			}
			return success;
		}


		private bool rotationIsDirty_ = false;
		public void SetRotationDirty()
		{
			if (!fixedRotation_)
			{
				rotationIsDirty_ = true;
			}
		}

		private float rotationPositionFraction = 0.1f;

		System.Text.StringBuilder debugSB = new System.Text.StringBuilder( );
		private bool DEBUG_ROTATIONS = true;

		private void Update()
		{
			if (rotationIsDirty_)
			{
				if (fixedRotation_)
				{
					Debug.LogError( "Rotation dirty but fixed on " + gameObject.name );
				}
				else
				{
					// Pos before - if 2 pts before and 1 after use catmullrom
					//              if 1 pt before use linear
					//              if 0 pt before use reverse linear wrt next

					if (DEBUG_ROTATIONS)
					{
						debugSB.Length = 0;
						debugSB.Append( "Calculate rotation for " ).DebugDescribe( this );
					}
					Vector3? posBefore = null;

					if ( previousSpinePoint_ != null && previousSpinePoint_.previousSpinePoint != null && nextSpinePoint_ != null )
					{

						RJWard.Core.CatMullRom3D interpolator = RJWard.Core.CatMullRom3D.CreateCentripetal
							(	previousSpinePoint_.previousSpinePoint.transform.position,
								previousSpinePoint_.transform.position,
								transform.position,
								nextSpinePoint_.transform.position );

						posBefore = interpolator.Interpolate( 1f - rotationPositionFraction );

						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosBefore from cmspline = " ).Append( (Vector3)posBefore );
						}
					}
					else if (previousSpinePoint_ != null)
					{
						posBefore = transform.position - rotationPositionFraction * (transform.position - previousSpinePoint_.transform.position);
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosBefore linear from previous = " ).Append( (Vector3)posBefore );
						}
					}
					else if (nextSpinePoint_ != null)
					{
						posBefore = transform.position - rotationPositionFraction * (nextSpinePoint_.transform.position - transform.position);
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosBefore linear from next = " ).Append( (Vector3)posBefore );
						}
					}

					// Pos after - if 2 pts after use catmullrom
					//             if 1 pt after use linear
					//              if 0 pt before use reverse linear wrt previous
					Vector3? posAfter = null;
					if (previousSpinePoint_ != null && nextSpinePoint_ != null && nextSpinePoint_.nextSpinePoint != null)
					{
						RJWard.Core.CatMullRom3D interpolator = RJWard.Core.CatMullRom3D.CreateCentripetal
							(	previousSpinePoint_.transform.position,
								transform.position,
								nextSpinePoint_.transform.position,
								nextSpinePoint_.nextSpinePoint.transform.position );

						posAfter = interpolator.Interpolate( rotationPositionFraction );

						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosAfter from cmspline = " ).Append( (Vector3)posAfter);
						}
					}
					else if (nextSpinePoint_ != null)
					{
						posAfter = transform.position + rotationPositionFraction * (nextSpinePoint_.transform.position - transform.position);
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosAfter linear from next = " ).Append( (Vector3)posAfter );
						}
					}
					else if (previousSpinePoint_ != null)
					{
						posAfter = transform.position + rotationPositionFraction * (transform.position - previousSpinePoint_.transform.position);
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosAfter linear from previous = " ).Append( (Vector3)posAfter );
						}
					}

					if (posBefore != null && posAfter != null)
					{
						Vector3 dirn = (Vector3)posAfter - (Vector3)posBefore;
						transform.LookAt( transform.position + dirn );
						rotationIsDirty_ = false;
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nUpdated rotation " );
							Debug.Log( debugSB.ToString() );
						}
					}
					else
					{
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\n!! Unable to update rotation " );
							Debug.LogWarning( "Unable to " + debugSB );
						}
					}

				}
				spine_.SetDirty( );
			}
		}

		public void Init( Spine sp,  Vector3 pos, Vector3? rot)
		{
			spine_ = sp;
			transform.localPosition = pos;
			if (rot != null)
			{
				transform.localRotation = Quaternion.Euler( (Vector3)rot );
			}
			else
			{
				transform.localRotation = Quaternion.identity;
			}
			if (hoop_ == null)
			{
				GameObject hoopGo = new GameObject( "Hoop" );
				hoop_ = hoopGo.AddComponent<Hoop>( );
			}
			hoop_.Init( this );
			rotationIsDirty_ = true;
		}

		public void InitCircular( Spine sp, Vector3 pos, Vector3? rot, int num, float rad )
		{
			Init( sp, pos, rot );
			MakeHoopCircular( num, rad );
		}

		
		private void MakeHoopCircular( int numPoints, float rad )
		{
			hoop.CreateHoopPointsCircular(numPoints, rad);
		}

		public void AddAllVertices( List<Vector3> verts, List< Vector3 > normals, List<Vector2> uvs, float v )
		{
			if (hoop_ != null)
			{
				hoop_.AddAllVertices( verts, normals, uvs, v );
			}
		}

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "SP " ).Append( gameObject.name ).Append( " @" ).Append( transform.position );
			sb.Append( " P/N = (" );
			if (previousSpinePoint_ == null)
			{
				sb.Append( "NONE" );
			}
			else
			{
				sb.Append( previousSpinePoint_.gameObject.name );
			}
			sb.Append( " / " );
			if (nextSpinePoint_ == null)
			{
				sb.Append( "NONE" );
			}
			else
			{
				sb.Append( nextSpinePoint_.gameObject.name );
			}
			sb.Append( ")" );
		}
	}
}
