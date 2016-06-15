using UnityEngine;
using System.Collections.Generic;
using System;

namespace RJWard.Tube
{
	public class SpinePoint_Linear : SpinePoint_Base
	{
		private Hoop preHoop_ = null;

		private Hoop hoop_ = null;
		public Hoop hoop
		{
			get { return hoop_; }
		}

		private FlowZone_LinearBase flowZone_ = null;
		public FlowZone_LinearBase flowZone
		{
			get
			{
				if (flowZoneProxy != null)
				{
					return flowZoneProxy.flowZone;
				}
				return flowZone_;
			}
			set { flowZone_ = value; }
		}

		public SpinePoint_Linear flowZoneProxy = null;

		override public bool isFirst( )
		{
			return previousSpinePoint_ == null;
		}

		override public bool isLast( )
		{
			return nextSpinePoint_ == null;
		}

		override public int MinSpinePointsToEnd(ref SpinePoint_Base endSpinePoint)
		{
			int result = 0;
			SpinePoint_Linear next = nextSpinePoint_;
			endSpinePoint = next;
			while (next != null)
			{
				next = next.nextSpinePoint_;
				if (next != null)
				{
					endSpinePoint = next;
				}
				result++;
			}

			return result;
		}

		override public int MinSpinePointsToEnd( )
		{
			int result = 0;
			SpinePoint_Linear next = nextSpinePoint_;
			while (next != null)
			{
				next = next.nextSpinePoint_;
				result++;
			}
			return result;
		}

		private SpinePointConnection connectionOut_ = null;
		private SpinePointConnection connectionIn_ = null;
		private bool connectionsDirty = false;
		private bool MakeConnectionsIfDirty()
		{
			if (connectionsDirty)
			{
				connectionOut_ = new SpinePointConnection( this, nextSpinePoint_, forwardInterpolator );
				connectionIn_ = new SpinePointConnection( previousSpinePoint_, this, previousSpinePoint_.forwardInterpolator );
				connectionsDirty = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override SpinePointConnection GetConnectionOut( SpinePointPathChooser chooser )
		{
			/*
			SpinePointConnection connection = null;
            if (nextSpinePoint_ != null)
			{
				connection = new SpinePointConnection( this, nextSpinePoint_, forwardInterpolator );
			}
			return connection;
			*/
			MakeConnectionsIfDirty( );
			if (connectionOut_ == null)
			{
				connectionOut_ = new SpinePointConnection( this, nextSpinePoint_, forwardInterpolator );
			}
			return connectionOut_;
		}

		public override SpinePointConnection GetConnectionIn( SpinePointPathChooser chooser )
		{
			/*
			SpinePointConnection connection = null;
			if (previousSpinePoint_ != null)
			{
				connection = new SpinePointConnection( previousSpinePoint_, this, previousSpinePoint_.forwardInterpolator );
			}
			return connection;
			*/
			MakeConnectionsIfDirty( );
			if (connectionIn_ == null)
			{
				connectionIn_ = new SpinePointConnection( previousSpinePoint_, this, previousSpinePoint_.forwardInterpolator );
			}
			return connectionIn_;
		}

		override public void DisconnnectFront( )
		{
			if (nextSpinePoint_ != null)
			{
				nextSpinePoint_.previousSpinePoint = this;
			}
			previousSpinePoint = null;
		}


		private SpinePoint_Linear nextSpinePoint_ = null;
		public SpinePoint_Linear nextSpinePoint
		{
			get { return nextSpinePoint_;  }
			set
			{
				if (value == this)
				{
					Debug.LogError( "Can't set nextspinepoint to " + value.gameObject.name + " on " + this.gameObject.name );
					return;
				}
				if (value != nextSpinePoint_)
				{
					connectionsDirty = true;
					SetRotationDirty();
				}
				nextSpinePoint_ = value;
				if (nextSpinePoint_ != null)
				{
					nextSpinePoint_.SetRotationDirty( );
					nextSpinePoint.connectionsDirty = true;
					if (nextSpinePoint_.nextSpinePoint != null)
					{
						nextSpinePoint_.nextSpinePoint.SetRotationDirty( );
					}
				}
				else
				{
					Debug.LogWarning( "Why setting nextSpinePoint to null?" );
				}
			}
		}

		private SpinePoint_Linear previousSpinePoint_ = null;
		public SpinePoint_Linear previousSpinePoint
		{
			get { return previousSpinePoint_;  }
			set
			{
				if (value == this)
				{
					Debug.LogError( "Can't set previousspinepoint to " + value.gameObject.name + " on " + this.gameObject.name );
					return;
				}
				if (value != previousSpinePoint_)
				{
					SetRotationDirty();
					connectionsDirty = true;
				}
				previousSpinePoint_ = value;
				if (previousSpinePoint_ != null)
				{
					previousSpinePoint_.connectionsDirty = true;
					previousSpinePoint_.SetRotationDirty( );
					if (previousSpinePoint_.previousSpinePoint != null)
					{
						previousSpinePoint_.previousSpinePoint.SetRotationDirty( );
					}
				}
				else
				{
//					Debug.LogWarning( "Why setting previousSpinePoint to null?" );
				}
			}
		}

		public RJWard.Core.CatMullRom3D forwardInterpolator = null;
		public RJWard.Core.CatMullRom3D backInterpolator = null;
		
		override public bool InterpolateForwardWorld(SpinePointPathChooser chooser, float t, ref Vector3 result)
		{
			bool success = false;
			result = Vector3.zero;

			if (t < 0f || t > 1f)
			{
				Debug.LogWarning( "t = " + t );
//				return false;
			}
			t = Mathf.Clamp01( t );

			if (t==0f)
			{
				result = cachedTransform_.position;
				success = true;
			}
			else
			{
				if (forwardInterpolator != null)
				{
					if (nextSpinePoint_ == null)
					{
						Debug.LogWarning( "Spine_Linear pt has a forward interpolator but no next point!" );
					}
					result = forwardInterpolator.Interpolate( t );
					success = true;
				}
				else
				{
					if (nextSpinePoint_ == null)
					{
						Debug.LogWarning( "Spine_Linear pt has no forward interpolator and no next point, can't interpolate!" );
					}
					else
					{
						result = Vector3.Lerp( cachedTransform_.position, nextSpinePoint_.cachedTransform_.position, t );
						success = true;
					}
				}
			}
			return success;
		}

		override public bool InterpolateBackwardWorld(SpinePointPathChooser chooser, float t, ref Vector3 result )
		{
			bool success = false;
			result = Vector3.zero;

			if (t < 0f || t > 1f)
			{
				Debug.LogWarning( "t = " + t );
			}
			t = Mathf.Clamp01( t );

			if (t==0)
			{
				result = cachedTransform_.position;
				success = true;
			}
			else
			{
				if (backInterpolator != null)
				{
					if (previousSpinePoint_ == null)
					{
						Debug.LogWarning( "Spine_Linear pt has a back interpolator but no prev point!" );
					}
					result = backInterpolator.Interpolate( 1 - t );
					success = true;
				}
				else
				{
					if (previousSpinePoint_ == null)
					{
						Debug.LogWarning( "Spine_Linear pt has no back interpolator and no previous point, can't interpolate!" );
					}
					else
					{
						result = Vector3.Lerp( cachedTransform_.position, previousSpinePoint_.cachedTransform_.position, t );
						success = true;
					}
				}
			}
			return success;
		}

		/*
		private bool rotationIsDirty_ = false;
		public void SetRotationDirty()
		{
//			if (!fixedRotation_)
			{
				rotationIsDirty_ = true;
			}
//			else
//			{
//				Debug.LogWarning( "Setting rotation dirty when fixed" );
//			}
		}
		*/

		private float rotationPositionFraction = 0.1f;

		System.Text.StringBuilder debugSB = new System.Text.StringBuilder( );
		private bool DEBUG_ROTATIONS = false;

		private void Update()
		{
			if (rotationIsDirty_)
			{
				if (fixedRotation_)
				{
					if (DEBUG_ROTATIONS)
					{
						Debug.LogWarning( "Rotation dirty but fixed on " + gameObject.name );
					}
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

					if (previousSpinePoint_ != null && previousSpinePoint_.previousSpinePoint != null && nextSpinePoint_ != null)
					{

						RJWard.Core.CatMullRom3D interpolator = RJWard.Core.CatMullRom3D.CreateCentripetal
							( previousSpinePoint_.previousSpinePoint.cachedTransform_.position,
								previousSpinePoint_.cachedTransform_.position,
								cachedTransform_.position,
								nextSpinePoint_.cachedTransform_.position );

						posBefore = interpolator.Interpolate( 1f - rotationPositionFraction );

						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosBefore from cmspline = " ).Append( (Vector3)posBefore );
						}
					}
					else if (previousSpinePoint_ != null)
					{
						posBefore = cachedTransform_.position - rotationPositionFraction * (cachedTransform_.position - previousSpinePoint_.cachedTransform_.position);
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosBefore linear from previous = " ).Append( (Vector3)posBefore );
						}
					}
					else if (nextSpinePoint_ != null)
					{
						posBefore = cachedTransform_.position - rotationPositionFraction * (nextSpinePoint_.cachedTransform_.position - transform.position);
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
							(	previousSpinePoint_.cachedTransform_.position,
								cachedTransform_.position,
								nextSpinePoint_.cachedTransform_.position,
								nextSpinePoint_.nextSpinePoint.cachedTransform_.position );

						posAfter = interpolator.Interpolate( rotationPositionFraction );

						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosAfter from cmspline = " ).Append( (Vector3)posAfter);
						}
					}
					else if (nextSpinePoint_ != null)
					{
						posAfter = cachedTransform_.position + rotationPositionFraction * (nextSpinePoint_.cachedTransform_.position - cachedTransform_.position);
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosAfter linear from next = " ).Append( (Vector3)posAfter );
						}
					}
					else if (previousSpinePoint_ != null)
					{
						posAfter = cachedTransform_.position + rotationPositionFraction * (cachedTransform_.position - previousSpinePoint_.cachedTransform_.position);
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosAfter linear from previous = " ).Append( (Vector3)posAfter );
						}
					}

					if (posBefore != null && posAfter != null)
					{
						Vector3 dirn = (Vector3)posAfter - (Vector3)posBefore;

						cachedTransform_.LookAt( cachedTransform_.position + dirn );
						rotationIsDirty_ = false;
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nUpdated rotation " );
							Debug.Log( debugSB.ToString() );
							debugSB.Length = 0;
						}
					}
					else if (posAfter != null)
					{
						cachedTransform_.LookAt( (Vector3)posAfter );
						rotationIsDirty_ = false;
						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nUpdated rotation using after only" );
							Debug.Log( debugSB.ToString( ) );
							debugSB.Length = 0;
						}
					}
					else
					{
						if (DEBUG_ROTATIONS)
						{
							rotationIsDirty_ = false;
							debugSB.Append( "\n!! Unable to update rotation " );
							Debug.LogWarning( "Unable to " + debugSB );
							debugSB.Length = 0;
						}
					}
				}
				/*
				if (flowZone  != null)
				{
					flowZone.HandleDirectionChange( );
					if (previousSpinePoint_ != null && previousSpinePoint_.flowZone != null)
					{
						previousSpinePoint_.flowZone.HandleDirectionChange( );
                    }
				}
				*/
				if (debugSB.Length > 0)
				{
					Debug.Log( debugSB );
				}
				rotationIsDirty_ = false;
			}
		}

		public void Init( Spine_Linear sp,  Vector3 pos, Vector3? rot)
		{
			spine_ = sp;
			cachedTransform_.localPosition = pos;
			if (rot != null)
			{
				cachedTransform_.localRotation = Quaternion.Euler( (Vector3)rot );
			}
			else
			{
				cachedTransform_.localRotation = Quaternion.identity;
			}
			if (hoop_ == null)
			{
				GameObject hoopGo = new GameObject( "Hoop" );
				hoopGo.layer = TubeFactory.Instance.buildLayerMask;
				hoop_ = hoopGo.AddComponent<Hoop>( );
			}
			hoop_.Init( this );
			SetRotationDirty( ) ;
		}

		public void InitCircular( Spine_Linear sp, Vector3 pos, Vector3? rot, int num, float rad )
		{
			Init( sp, pos, rot );
			MakeHoopCircular( num, rad );
		}

		public void InitExplicit( Spine_Linear sp, HoopDefinition_Explicit hde )
		{
			Init( sp, hde.position, hde.rotation);
			MakeHoopExplicit( hde);
		}

		private void MakeHoopExplicit( HoopDefinition_Explicit hde )
		{
			hoop.CreateHoopPointsExplicit( hde );
		}

		private void MakeHoopCircular( int numPoints, float rad )
		{
			hoop.CreateHoopPointsCircular(numPoints, rad);
		}

		public void AddPreHoopVertices( List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, float v )
		{
			if (preHoop_ != null)
			{
				GameObject.Destroy( preHoop_.gameObject );
			}
			GameObject hoopGo = new GameObject( "PreHoop" );
			hoopGo.layer = TubeFactory.Instance.buildLayerMask;
			preHoop_ = hoopGo.AddComponent<Hoop>( );
			HoopDefinition_Explicit defn = hoop_.ExplicitDefinition( );
			
			preHoop_.CreateHoopPointsExplicit( defn );
			preHoop_.Init( this );
			preHoop_.MoveAllHoopPoints( 2f, 1f );

			preHoop_.ExtractAllVertexInfo( verts, normals, uvs, v );
			// TODO Add vertices
		}

		public void AddAllVertices( List<Vector3> verts, List< Vector3 > normals, List<Vector2> uvs, float v )
		{
			if (hoop_ != null)
			{
				hoop_.ExtractAllVertexInfo( verts, normals, uvs, v );
			}
		}

		public void AddPreSectionTriInfoToList( List<int> triVerts )
		{
			Hoop.ExtractConnectingTriVerts( preHoop_, hoop_, triVerts, false );
		}

		override protected void DebugDescribeDetails( System.Text.StringBuilder sb )
		{
			sb.Append( "Linear " ).Append( gameObject.name ).Append( " @" ).Append( cachedTransform_.position );
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
