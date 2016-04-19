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

//		private TubeSection_Linear tubeSection_ = null;

		public TubeFactory.RandLinearSectionDefn randLinearSectionDefn;

		public Tube tube_ = null;

		// Use this for initialization
		void Start( )
		{
			TubeFactory.Instance.tubeWallMaterial = tubeWallMaterial;
			GameObject tubeGO = new GameObject( "Tube" );
			tubeGO.transform.position = Vector3.zero;
			tubeGO.transform.localScale = Vector3.one;
			tube_ = tubeGO.AddComponent<Tube>( );
		}

		public void NewFromSources()
		{
//			ClearTubeSection( );
			TubeFactory.Instance.CreateFromSourcesInContainer( testTubeContainer, numHoopPoints, tubeWallMaterial, HandleTubeSectionMade );
		}

		public void ClearTube()
		{
			tube_.DeleteAllSections( );
		}

		private void HandleTubeSectionMade( TubeSection_Linear ts)
		{
			tube_.AddToEnd( ts );
		}

		/*
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

		}*/

		public void CreateRandomSection()
		{
			Debug.Log( "Randomising" );
			TubeFactory.Instance.CreateRandomLinearSection(randLinearSectionDefn,  HandleTubeSectionMade );
		}
	}

}
