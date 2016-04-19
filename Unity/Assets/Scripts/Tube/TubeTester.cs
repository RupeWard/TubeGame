using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeTester: MonoBehaviour
	{
		public Transform pos1 = null;
		public Transform pos2 = null;
		public int num = 5;
		public float startRadius = 5f;
		public float endRadius = 8f;

		public int delaySecs = 10;

		public int numHoopPoints = 10;

		public Material tubeWallMaterial;

		public Transform testTubeContainer;

		private TubeSection_Linear tubeSection_ = null;

		public TubeFactory.RandLinearSectionDefn randLinearSectionDefn;

		// Use this for initialization
		void Start( )
		{
			TubeFactory.Instance.tubeWallMaterial = tubeWallMaterial;
		}

		public void NewFromSources()
		{
//			ClearTubeSection( );
			TubeFactory.Instance.CreateFromSourcesInContainer( testTubeContainer, numHoopPoints, tubeWallMaterial, HandleTubeSectionMade );
		}

		public void ClearTubeSection()
		{
			if (tubeSection_ != null)
			{
				GameObject.Destroy( tubeSection_.gameObject );
			}
			tubeSection_ = null;
		}

		private void HandleInitialTubesectionMade(TubeSection_Linear ts)
		{
			Debug.Log( "HandleInitialTubesectionMade" );
			tubeSection_ = ts;
		}

		private void HandleTubeSectionMade( TubeSection_Linear ts)
		{
			if (tubeSection_ == null)
			{
				tubeSection_ = ts;
			}
			else
			{
				AppendSectionToEnd( ts );
			}
		}

		public void AppendSectionToEnd(TubeSection_Linear ts)
		{
			Debug.Log( "Appending to end" );
			ts.gameObject.name = "New_" + ts.gameObject.name;

			Hoop lastHoopOfPrevious = tubeSection_.LastHoop( );
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

		public void AppendSection()
		{
			Debug.Log( "DEFUNCT" );

			//			TubeFactory.Instance.CreateFromSourcesInContainer( testTubeContainer, numHoopPoints, tubeWallMaterial, AppendSectionToEnd );
//			TubeFactory.Instance.CreateRandomLinearSection( randLinearSectionDefn, AppendSectionToEnd );

		}

		// Update is called once per frame
		void Update( )
		{

		}

		public void CreateRandomSection()
		{
			Debug.Log( "Randomising" );
			TubeFactory.Instance.CreateRandomLinearSection(randLinearSectionDefn,  HandleTubeSectionMade );
		}
	}

}
