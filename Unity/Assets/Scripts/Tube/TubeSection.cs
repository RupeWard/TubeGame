using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeSection : MonoBehaviour
	{
		public bool remakeMeshWhenDirty = true;

		private Spine spine_ = null;
		private Material tubeWallMaterial_;

		private static System.Text.StringBuilder debugSb = new System.Text.StringBuilder( );

		private bool isMeshDirty_ = false;
		public void SetMeshDirty ()
		{
			isMeshDirty_ = true;
		}

		public static TubeSection CreateCircular( string n, TubeSectionDefinition tsd, Material mat )
		{
			GameObject tsGo = new GameObject( n );
			TubeSection result = tsGo.AddComponent<TubeSection>( );
			result.InitCircular( n, tsd, mat );

			return result;
		}

		public static TubeSection CreateSplinar( string n, TubeSection ts, int numPerSection, Material mat )
		{
			GameObject tsGo = new GameObject( n );
			TubeSection result = tsGo.AddComponent<TubeSection>( );
			result.InitSplinar( n, ts, numPerSection, mat );

			return result;
		}

		private void Init(string n, Material mat)
		{
			gameObject.name = n;

			tubeWallMaterial_ = mat;

		}

		private void InitCircular( string n, TubeSectionDefinition tsd, Material mat )
		{
			Init( n, mat );

			debugSb.Length = 0;
			debugSb.Append( "Creating TubeSection" );

			GameObject spineGO = new GameObject( "SP"+n );
			spine_ = spineGO.AddComponent<Spine>( );
			spine_.Init( this );

			debugSb.Append( "\n Created spine, adding " ).Append( tsd.NumSpinePoints ).Append( " spinepoints" );

			for (int i = 0; i < tsd.NumSpinePoints; i++)
			{
				SpinePointDefinition spd = tsd.GetSpinePointDefn( i );
				if (spd != null)
				{
					spine_.AddCircularSpinePoint( spd );
					debugSb.Append( "\n  " ).Append( i ).Append( ": " ).DebugDescribe( spd );
				}
				else
				{
					Debug.LogError( "NULL SPD" );
					debugSb.Append( "\n  " ).Append( i ).Append( ": NULL" );
				}
			}

			Debug.Log( debugSb.ToString( ) );
		}

		public void InitSplinar( string n, TubeSection srcTs, int numPerSection, Material mat )
		{
			remakeMeshWhenDirty = false;
			StartCoroutine( InitSplinarCR(n, srcTs, numPerSection, mat ) );
        }

		private IEnumerator InitSplinarCR(string n, TubeSection srcTs, int numPerSection, Material mat)
		{
			Init( n + "_SPL", mat );

			debugSb.Length = 0;
			debugSb.Append( "Generating splinar with " ).Append( numPerSection ).Append( " per section" );
		
			GameObject spineGO = new GameObject( "Sp");
			spine_ = spineGO.AddComponent<Spine>( );
			spine_.Init( this );
			
			bool abort = false;

			int numHoopPoints = int.MaxValue;
			for (int i = 0; !abort && i < srcTs.spine_.NumSpinePoints; i++)
			{
				SpinePoint spt = srcTs.spine_.GetSpinePoint( i );
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

			if (numHoopPoints != int.MaxValue)
			{
				debugSb.Append( "\n Found " ).Append( numHoopPoints ).Append( " hoop points per spine point" );

				// interpolate spine points

				List<Vector3> oldSpinePtPosns = new List<Vector3>( );
				for (int i = 0; i < srcTs.spine_.NumSpinePoints; i++)
				{
					oldSpinePtPosns.Add( srcTs.spine_.GetSpinePoint( i ).transform.position );
				}

				// Prepare lists to contains positions : spinePoints & N * hoopPoints

				List<RJWard.Core.CatMullRom3D> spinePointInterpolators = new List<Core.CatMullRom3D>( );
				List<Vector3> spinePointPositions = RJWard.Core.CatMullRom3D.InterpolateFixedNumCentripetal( oldSpinePtPosns, numPerSection, spinePointInterpolators );

				if (spinePointPositions.Count != (spinePointInterpolators.Count+1))
				{
					Debug.LogError( spinePointPositions.Count + " posns " + spinePointInterpolators.Count + " interpolators" );
				}
				int numSpinePoints = spinePointPositions.Count;
				debugSb.Append( "\n Interpolated spine points from ").Append(oldSpinePtPosns.Count).Append( " to ").Append(numSpinePoints );

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
						debugSb.Append( "\n Interpolated hoop points ").Append(hoopIndex).Append(" from " ).Append( oldSpinePtPosns.Count ).Append( " to " ).Append( numSpinePoints );
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
						spine_.AddHoopLess( spinePointPositions[ptNum] );						
					}
					debugSb.Append( "\n Made ").Append(spine_.NumSpinePoints).Append(" spine points, waiting for rotations" );
					yield return null;

					for (int ptNum = 0; ptNum < numNewPoints; ptNum++)
					{
						SpinePoint spinePoint = spine_.GetSpinePoint( ptNum );
						if (spinePoint == null)
						{
							Debug.LogError( "No spine point " + ptNum );
						}
						else
						{
							if (ptNum > 0)
							{
								spinePoint.backInterpolator = spinePointInterpolators[ptNum - 1];
							}
							if (ptNum < (numNewPoints-1))
							{
								spinePoint.forwardInterpolator = spinePointInterpolators[ptNum];
							}
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
							}
							for (int hoopPtNum = 0; hoopPtNum < numHoopPoints; hoopPtNum++)
							{
								HoopPoint hp = spinePoint.hoop.GetHoopPoint( hoopPtNum );
								RJWard.Core.Test.DebugBlob.AddToObject( hp.gameObject, 0.2f, spinePoint.hoop.GetColourForPoint( hp) );
							}

							debugSb.Append( "\n Added " ).Append( nAdded ).Append( " hps of " ).Append( numHoopPoints )
								.Append( " to hoop " ).Append( ptNum );
						}
					}


				}

			}

			if (debugSb.Length > 0)
			{
				Debug.Log( debugSb.ToString( ) );
			}
			yield return null;
			remakeMeshWhenDirty = true;
			SetMeshDirty( );
		}

		private void Update()
		{
			if (remakeMeshWhenDirty && isMeshDirty_)
			{
				MakeMesh( );
			}
		}

		
		public static TubeSection CreateLinear( Vector3 start, Vector3? startRotation, Vector3 end, Vector3? endRotation, int num, float startRadius, float endRadius, int numHoopPoints, Material mat )
		{			
			GameObject tsGo = new GameObject( "TubeSection" );
			TubeSection result = tsGo.AddComponent<TubeSection>( );
			result.tubeWallMaterial_ = mat;

			GameObject spineGO = new GameObject( "Spine" );
			spineGO.transform.parent = tsGo.transform;

			result.spine_ = spineGO.AddComponent<Spine>( );

			for (int i = 0; i < num; i++)
			{
				Vector3? rot = null;
				if (i == 0)
				{
					rot = startRotation;
				}
				else if (i == num-1)
				{
					rot = endRotation;
				}
				float interpolator = (float)i / (num - 1);
                result.spine_.AddCircularSpinePoint( Vector3.Lerp(start, end,  interpolator) , rot, numHoopPoints, Mathf.Lerp(startRadius, endRadius,  interpolator));
			}

			return result;
		}

		private void MakeMesh()
		{
			Debug.Log( "Make Mesh for " + this.gameObject );

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
			mesh.Clear( );

			
			if (spine_ != null)
			{
				List<Vector3> verts = new List<Vector3>( );
				List<Vector2> uvs = new List<Vector2>( );
				List<Vector3> normals = new List<Vector3>( );

				spine_.AddAllVertexInfoToLists( verts, normals, uvs );
				Debug.Log( "Verts count = " + verts.Count );

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
