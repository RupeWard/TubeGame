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

