using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeSection_Linear : MonoBehaviour
	{
		static private readonly bool DEBUG_LOCAL = false;
		static private readonly bool DEBUG_CIRCULAR = false;
		static private readonly bool DEBUG_SPLINAR = false;
		static private readonly bool DEBUG_MESH = false;

		public void ConnectAfterSpinePoint(SpinePoint_Linear sp)
		{
			if (spine_ != null && spine_.NumSpinePoints > 0)
			{
				SpinePoint_Linear firstSp = spine_.GetSpinePoint( 0 );
				firstSp.previousSpinePoint = sp;
				
				SpinePoint_Linear mySp = spine_.GetSpinePoint( 1 );
				mySp.previousSpinePoint = sp;
				if (sp != null)
				{
					sp.nextSpinePoint = mySp;
				}
			}
			else
			{
				sp.nextSpinePoint = null;
			}
		}

		public bool remakeMeshWhenDirty = false;

		private Spine_Linear spine_ = null;
		private Material tubeWallMaterial_;

		private bool isMeshDirty_ = false;
		public void SetMeshDirty (bool force)
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "Setting mesh dirty on " + gameObject.name );
			}
			isMeshDirty_ = true;
			if (force)
			{
				remakeMeshWhenDirty = true; 
			}
		}

		public bool isMeshDirty
		{
			get { return isMeshDirty_; }
		}

		static int s_counter = 0;
		int id_;

		private void Awake()
		{
			id_ = s_counter;
			s_counter++;
		}

		public Hoop LastHoop()
		{
			Hoop result = null;
			if (spine_ != null)
			{
				SpinePoint_Linear sp = spine_.GetSpinePoint( spine_.NumSpinePoints - 1 );
				if (sp != null)
				{
					result = sp.hoop;
				}
				else
				{
					Debug.LogWarning( "No last spinepoint on " + this.gameObject.name );
				}
			}
			else
			{
				Debug.LogWarning( "No spine on " + this.gameObject.name );
			}
			return result;
		}

		public Hoop FirstHoop( )
		{
			Hoop result = null;
			if (spine_ != null)
			{
				SpinePoint_Linear sp = spine_.GetSpinePoint( 0 );
				if (sp != null)
				{
					result = sp.hoop;
				}
				else
				{
					Debug.LogWarning( "No first spinepoint on " + this.gameObject.name );
				}
			}
			else
			{
				Debug.LogWarning( "No spine on " + this.gameObject.name );
			}
			return result;
		}

		private void Init(string n, Material mat, System.Text.StringBuilder debugsb)
		{
			gameObject.name = id_.ToString()+"_" +n;
			tubeWallMaterial_ = mat;
			if (debugsb != null)
			{
				debugsb.Append( " #" + id_+" as "+gameObject.name );
			}
		}

		public IEnumerator InitCircularCR( string n, TubeSectionDefinition_Linear tsd, Material mat )
		{
            System.Text.StringBuilder debugSb = null;

			if (DEBUG_CIRCULAR)
			{
				debugSb = new System.Text.StringBuilder( );
				debugSb.Length = 0;
				debugSb.Append( "TubeSection_Linear.InitCircular" );
			}

			Init( n, mat, debugSb );

			GameObject spineGO = new GameObject( "SP"+n );
			spine_ = spineGO.AddComponent<Spine_Linear>( );
			spine_.Init( this );

			yield return null;

			if (DEBUG_CIRCULAR)
			{
				debugSb.Append( "\n Created spine, adding " ).Append( tsd.NumSpinePoints ).Append( " spinepoints" );
			}

			for (int i = 0; i < tsd.NumSpinePoints; i++)
			{
				HoopDefinition_Base hdb = tsd.GetHoopDefn( i );
				if (hdb != null)
				{
					spine_.AddSpinePoint( hdb, i==0 );
//					yield return null;
					if (DEBUG_CIRCULAR)
					{
						debugSb.Append( "\n  " ).Append( i ).Append( ": " ).DebugDescribe( hdb );
					}
				}
				else
				{
					Debug.LogError( "NULL SPD" );
					if (DEBUG_CIRCULAR)
					{
						debugSb.Append( "\n  " ).Append( i ).Append( ": NULL" );
					}
				}

			}
			if (DEBUG_CIRCULAR)
			{
				Debug.Log( debugSb.ToString( ) );
			}
		}

		/*
		public void InitSplinar( string n, TubeSection_Linear srcTs, int numPerSection, Material mat )
		{
			StartCoroutine( InitSplinarCR(n, srcTs, numPerSection, mat ) );
        }
		*/

		public IEnumerator InitSplinarCR(string n, TubeSection_Linear srcTs, int numPerSection, Material mat)
		{
			System.Text.StringBuilder debugSb = null;

			if (DEBUG_SPLINAR)
			{
				debugSb = new System.Text.StringBuilder( );
				debugSb.Length = 0;
				debugSb.Append( "TubeSection_Linear.InitSplinar " );
			}
			remakeMeshWhenDirty = false;
			
			Init( n + "_SPL", mat, debugSb );

			if (DEBUG_SPLINAR)
			{
				debugSb.Append( "\nGenerating splinar " + gameObject.name + " with " ).Append( numPerSection ).Append( " per section" );
			}

			GameObject spineGO = new GameObject( "Sp");
			spine_ = spineGO.AddComponent<Spine_Linear>( );
			spine_.Init( this );
			yield return null;

			bool abort = false;

			int numHoopPoints = int.MaxValue;
			for (int i = 0; !abort && i < srcTs.spine_.NumSpinePoints; i++)
			{
				SpinePoint_Linear spt = srcTs.spine_.GetSpinePoint( i );
				if (numHoopPoints != int.MaxValue && numHoopPoints != spt.hoop.numPoints( ))
				{
					Debug.LogError( "NumPoints mismatch" );
					numHoopPoints = int.MaxValue;
					abort = true;
					break;
				}
				else
				{
					numHoopPoints = spt.hoop.numPoints( );
				}
			}
			yield return null;

			if (numHoopPoints != int.MaxValue)
			{
				if (DEBUG_SPLINAR)
				{
					debugSb.Append( "\n Found " ).Append( numHoopPoints ).Append( " hoop points per spine point" );
				}

				// interpolate spine points

				List<Vector3> oldSpinePtPosns = new List<Vector3>( );
				for (int i = 0; i < srcTs.spine_.NumSpinePoints; i++)
				{
					oldSpinePtPosns.Add( srcTs.spine_.GetSpinePoint( i ).transform.position );
				}

				// Prepare lists to contains positions : spinePoints & N * hoopPoints

				List<RJWard.Core.CatMullRom3D> spinePointInterpolators = new List<Core.CatMullRom3D>( );
				List<Vector3> spinePointPositions = RJWard.Core.CatMullRom3D.InterpolateFixedNumCentripetal( oldSpinePtPosns, numPerSection, spinePointInterpolators );
				// yield return null;

				if (spinePointPositions.Count != (spinePointInterpolators.Count * numPerSection +1))
				{
					Debug.LogError( spinePointPositions.Count + " posns " + spinePointInterpolators.Count + " interpolators" );
				}
				int numSpinePoints = spinePointPositions.Count;
				if (DEBUG_SPLINAR)
				{
					debugSb.Append( "\n Interpolated spine points from " ).Append( oldSpinePtPosns.Count ).Append( " to " ).Append( numSpinePoints );
				}

				// interpolate hoop points
				List<Vector3>[] hoopPointPositions = new List<Vector3>[numHoopPoints];

				for (int hoopIndex = 0; !abort && hoopIndex < numHoopPoints; hoopIndex++)
				{
					List<Vector3> oldHoopPtPosns = new List<Vector3>( );
					for (int i = 0; i < srcTs.spine_.NumSpinePoints; i++)
					{
						oldHoopPtPosns.Add( srcTs.spine_.GetSpinePoint( i ).hoop.GetHoopPoint( hoopIndex ).transform.position );
					}

					if (oldHoopPtPosns.Count != oldSpinePtPosns.Count)
					{
						Debug.LogError( "spine/hoop src num mismatch" );
						abort = true;
					}
					else
					{
						hoopPointPositions[hoopIndex] = RJWard.Core.CatMullRom3D.InterpolateFixedNumCentripetal( oldHoopPtPosns, numPerSection, null );
						//yield return null;
						if (DEBUG_SPLINAR)
						{
							debugSb.Append( "\n Interpolated hoop points " ).Append( hoopIndex ).Append( " from " ).Append( oldSpinePtPosns.Count ).Append( " to " ).Append( numSpinePoints );
						}
						if (hoopPointPositions[hoopIndex].Count != spinePointPositions.Count)
						{
							Debug.LogError( "spine/hoop src num mistmatch" );
							abort = true;
						}
					}
				}

				if (!abort)
				{
					int numNewPoints = spinePointPositions.Count;
					for (int ptNum = 0; ptNum < numNewPoints; ptNum++ )
					{
						spine_.AddHoopLess( spinePointPositions[ptNum], ptNum==0  );						
					}
					if (DEBUG_SPLINAR)
					{
						debugSb.Append( "\n Made " ).Append( spine_.NumSpinePoints ).Append( " spine points, waiting for rotations" );
					}
					yield return null;

					for (int ptNum = 0; ptNum < numNewPoints; ptNum++)
					{
						SpinePoint_Linear spinePoint = spine_.GetSpinePoint( ptNum );
						if (spinePoint == null)
						{
							Debug.LogError( "No spine point " + ptNum );
						}
						else
						{
							int nAdded = 0;
							for (int hoopPtNum = 0; hoopPtNum < numHoopPoints; hoopPtNum++)
							{
								try
								{
									HoopPoint h = spinePoint.hoop.AddHoopPoint( hoopPointPositions[hoopPtNum][ptNum] );
									if (h == null)
									{
										Debug.LogError( "null hooppoint" );
									}
									else
									{
										nAdded++;
									}
								}
								catch (System.NullReferenceException /* nre */)
								{
									Debug.LogError( "NRE when hoopPtNum = " + hoopPtNum + " and ptNum = " + ptNum );
								}
								//yield return null;
							}
							for (int hoopPtNum = 0; hoopPtNum < numHoopPoints; hoopPtNum++)
							{
								HoopPoint hp = spinePoint.hoop.GetHoopPoint( hoopPtNum );
								RJWard.Core.Test.DebugBlob.AddToObject( hp.gameObject, 0.1f, spinePoint.hoop.GetColourForPoint( hp) );
								//yield return null;
							}

							if (DEBUG_SPLINAR)
							{
								debugSb.Append( "\n Added " ).Append( nAdded ).Append( " hps of " ).Append( numHoopPoints )
									.Append( " to hoop " ).Append( ptNum );
							}
						}
						spinePoint.hoop.CheckCentreing( );
					}
				}
			}

			if (DEBUG_SPLINAR && debugSb.Length > 0)
			{
				Debug.Log( debugSb.ToString( ) );
			}
			yield return null;
			SetMeshDirty(false );
		}

		private void LateUpdate()
		{
			if (remakeMeshWhenDirty && isMeshDirty_)
			{
				MakeMesh( );
			}
			else if (remakeMeshWhenDirty)
			{
//				Debug.LogWarning( "Not remaking as not dirty" );
			}
			else if (isMeshDirty_)
			{
				if (DEBUG_LOCAL)
				{
					Debug.LogWarning( "Not remaking as not remaking when dirty" );
				}
			}
		}

		private void MakeMesh()
		{
			if (DEBUG_MESH)
			{
				Debug.Log( "Make Mesh for " + this.gameObject );
			}

			RJWard.Core.ReverseNormals reverseNormals = GetComponent<RJWard.Core.ReverseNormals>( );
			if (reverseNormals != null)
			{
				Component.Destroy( reverseNormals );
				reverseNormals = null;
			}

			MeshRenderer meshRenderer = GetComponent<MeshRenderer>( );
			if (meshRenderer == null)
			{
				meshRenderer = gameObject.AddComponent<MeshRenderer>( );
			}
			meshRenderer.sharedMaterial = tubeWallMaterial_;

			MeshFilter meshFilter = GetComponent<MeshFilter>( );
			if (meshFilter == null)
			{
				meshFilter = gameObject.AddComponent<MeshFilter>( );
			}
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh == null)
			{
				meshFilter.sharedMesh = new Mesh( );
				mesh = meshFilter.sharedMesh;
			}
			else
			{
				Debug.LogError( "Remaking mesh" );
			}
			mesh.Clear( );
			
			if (spine_ != null)
			{
				List<Vector3> verts = new List<Vector3>( );
				List<Vector2> uvs = new List<Vector2>( );
				List<Vector3> normals = new List<Vector3>( );

				spine_.AddAllVertexInfoToLists( verts, normals, uvs );
				if (DEBUG_MESH)
				{
					Debug.Log( "Verts count = " + verts.Count );
				}

				List<int> triVerts = new List<int>( );
				spine_.AddAllTriInfoToList( triVerts );

				mesh.vertices = verts.ToArray( );
				mesh.triangles = triVerts.ToArray( );
				mesh.uv = uvs.ToArray();
				mesh.normals = normals.ToArray();

				mesh.RecalculateBounds( );
				mesh.Optimize( );

				reverseNormals = gameObject.AddComponent<RJWard.Core.ReverseNormals>( );
				reverseNormals.Init( Core.ReverseNormals.EState.Inside );
			}
			isMeshDirty_ = false;
		}
	}

}
