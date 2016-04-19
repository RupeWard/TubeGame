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

		public void AddToEnd(TubeSection_Linear ts)
		{
			Debug.Log( "Appending to end" );
			ts.transform.parent = transform;
			ts.transform.localScale = Vector3.one;
			ts.transform.localPosition = Vector3.zero;
			ts.transform.localRotation = Quaternion.identity;

			if (tubeSections_.Count > 0)
			{
				Hoop lastHoopOfPrevious = tubeSections_[tubeSections_.Count-1].LastHoop( );
				if (lastHoopOfPrevious == null)
				{
					Debug.LogWarning( "No last hoop of previous" );
				}
				Hoop firstHoop = ts.FirstHoop( );
				if (firstHoop == null)
				{
					Debug.LogWarning( "No firstHoop" );
				}

				ts.gameObject.transform.Translate( lastHoopOfPrevious.transform.position - firstHoop.transform.position );
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

