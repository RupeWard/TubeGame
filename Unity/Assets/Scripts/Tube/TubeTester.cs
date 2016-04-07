﻿using UnityEngine;
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

		private TubeSection tubeSection_ = null;

		// Use this for initialization
		void Start( )
		{
			StartCoroutine( CreateFromSourcesInContainerCR( testTubeContainer, HandleInitialTubesectionMade ) );

		}

		private int tsNumber = 0;

		private IEnumerator CreateFromSourcesInContainerCR( Transform container, System.Action<TubeSection> onTubeSectionMadeAction)
		{
			yield return null;

			if (testTubeContainer != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder( );

				TubeSectionDefinition tsd = new TubeSectionDefinition( );

				sb.Append( "Building Tube Section" );

                SpinePointSource[] spinePointSources = container.GetComponentsInChildren<SpinePointSource>( );
				sb.Append( "\nFound " ).Append( spinePointSources.Length ).Append( " sources" );

				for (int i = 0; i < spinePointSources.Length; i++)
				{
					SpinePointSource sps = spinePointSources[i];
					sb.Append( "\n " ).Append( i ).Append(": ").DebugDescribe(sps );

					Vector3 pos = sps.transform.position;
					Vector3? rot = null;
					if (sps.fixRotation)
					{
						rot = sps.transform.rotation.eulerAngles;
					}
					float rad = sps.radius;

					SpinePointDefinition spd = new SpinePointDefinition( pos, rot, numHoopPoints, rad );
					tsd.AddSpinePointDefn( spd );

					sb.Append( "\n  Added as " ).DebugDescribe( spd );
				}

				Debug.Log( sb.ToString( ) );

				TubeSection newTs0 = TubeSection.CreateCircular("TS"+tsNumber.ToString(), tsd, tubeWallMaterial );
				testTubeContainer.gameObject.SetActive( false );
				tsNumber++;

				for (int i = delaySecs; i > -1; i--)
				{
					Debug.Log( "Waiting for " + i );
					yield return new WaitForSeconds( 1f );
				}
				TubeSection newTs1 = TubeSection.CreateSplinar( "SPLINAR", newTs0, 5, tubeWallMaterial );
				GameObject.Destroy( newTs0.gameObject );
				yield return null;
				yield return new WaitForSeconds(5f);
				if (onTubeSectionMadeAction != null)
				{
					onTubeSectionMadeAction( newTs1 );
				}
			}

		}

		private void HandleInitialTubesectionMade(TubeSection ts)
		{
			tubeSection_ = ts;
		}

		public void AppendSectionToEnd(TubeSection ts)
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

		public void DuplicateSection()
		{
			Debug.Log( "DUPLICATING" );
			StartCoroutine( CreateFromSourcesInContainerCR( testTubeContainer, AppendSectionToEnd ) );

		}

		// Update is called once per frame
		void Update( )
		{

		}
	}

}
