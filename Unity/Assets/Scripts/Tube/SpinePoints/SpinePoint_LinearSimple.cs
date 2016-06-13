using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class SpinePoint_LinearSimple : SpinePoint_Base
	{
		private Hoop hoop_ = null;
		public Hoop hoop
		{
			get { return hoop_; }
		}

		private List<SpinePointConnection> connectionsOut = new List<SpinePointConnection>( );
		private List<SpinePointConnection> connectionsIn = new List<SpinePointConnection>( );

		override public bool isFirst(  )
		{
			return connectionsIn.Count == 0;
		}

		override public bool isLast( )
		{
			return connectionsOut.Count == 0;
		}

		public override SpinePointConnection GetConnectionOut( SpinePointPathChooser chooser )
		{
			SpinePointConnection connection = null;
			if (connectionsOut.Count == 0)
			{
				Debug.Log( "No connectionns out " + this.DebugDescribe( ) );
			}
			else
			{
				connection = connectionsOut[0]; // TODO use chooser
			}
			return connection;
		}

		public override SpinePointConnection GetConnectionIn( SpinePointPathChooser chooser )
		{
			SpinePointConnection connection = null;
			if (connectionsIn.Count == 0)
			{
				Debug.Log( "No connectionns in " + this.DebugDescribe( ) );
			}
			else
			{
				connection = connectionsIn[0]; // TODO use chooser
			}
			return connection;
		}

		override public void DisconnnectFront( )
		{
			// FIXME implement
			Debug.LogError( "Not implemented" );
		}


		override public int MinSpinePointsToEnd( ref SpinePoint_Base endSpinePoint )
		{
			Debug.LogError( "Not implemented" );
			int result = 0;
			/*
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
			*/
			return result;
		}

		override public int MinSpinePointsToEnd( )
		{
			Debug.LogError( "Not implemented" );
			int result = 0;
			/*
			SpinePoint_Linear next = nextSpinePoint_;
			while (next != null)
			{
				next = next.nextSpinePoint_;
				result++;
			}
			*/
			return result;
		}

		/*
		override public bool InterpolateForwardWorld( SpinePointPathChooser chooser, float t, ref Vector3 result )
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
				SpinePointConnection connection = GetConnectionOut( chooser );
				if (connection != null)
				{
					result = connection.InterpolatePosition(t );
					success = true;
				}
				else
				{
					Debug.LogWarning( "Spine_Linear pt has no out connection "+this.DebugDescribe() );
				}
			}
			return success;
		}

		override public bool InterpolateBackwardWorld( SpinePointPathChooser chooser, float t, ref Vector3 result )
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
				SpinePointConnection connection = GetConnectionIn( chooser );
				if (connection != null)
				{
					result = connection.InterpolatePosition( 1 - t );
					success = true;
				}
				else
				{
					Debug.LogWarning( "Spine_Linear pt has no in connection " + this.DebugDescribe( ) );
				}
			}
			return success;
		}

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

					SpinePointConnection connectionIn = GetConnectionIn( null );
					if (connectionIn != null)
					{
						posBefore = connectionIn.InterpolatePosition( 1f - rotationPositionFraction );

						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosBefore from connection = " ).Append( (Vector3)posBefore );
						}
					}

					// Pos after - if 2 pts after use catmullrom
					//             if 1 pt after use linear
					//              if 0 pt before use reverse linear wrt previous
					SpinePointConnection connectionOut = GetConnectionOut( null );
					Vector3? posAfter = null;
					if (connectionOut != null)
					{
						posAfter = connectionOut.InterpolatePosition( rotationPositionFraction );

						if (DEBUG_ROTATIONS)
						{
							debugSB.Append( "\nPosAfter from connection = " ).Append( (Vector3)posAfter);
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
				hoop_ = hoopGo.AddComponent<Hoop>( );
			}
			hoop_.Init( this );
			SetRotationDirty();
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

		public void AddAllVertices( List<Vector3> verts, List< Vector3 > normals, List<Vector2> uvs, float v )
		{
			if (hoop_ != null)
			{
				hoop_.ExtractAllVertexInfo( verts, normals, uvs, v );
			}
		}

		override protected void DebugDescribeDetails( System.Text.StringBuilder sb )
		{
			sb.Append( "Linear " ).Append( gameObject.name ).Append( " @" ).Append( cachedTransform_.position );
			sb.Append( " IN = (" ).Append(connectionsIn.Count ).Append(":");
			for (int i = 0; i<connectionsIn.Count; i++)
			{
				sb.Append(" ").Append( connectionsIn[i].startPoint.gameObject.name );
			}
			sb.Append( ") OUT = (" ).Append(connectionsOut.Count).Append(":");
			for (int i = 0; i < connectionsOut.Count; i++)
			{
				sb.Append( " " ).Append( connectionsOut[i].startPoint.gameObject.name );
			}
			sb.Append( ")" );
		}
	}
}
