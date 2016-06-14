using UnityEngine;
using UnityEditor;

public class DatabaseEditor
{
	[MenuItem( "Tube/DB/Delete Progress" )]
	public static void DeleteProgress( )
	{
		DeleteDB( "Progress" );
	}

	[MenuItem( "Tube/DB/Delete CoreData" )]
	public static void DeleteCoreData( )
	{
		DeleteDB( "CoreData" );
	}

	public static void DeleteDB( string n)
	{
		string databaseFolder = Application.persistentDataPath + "/Data/";
		System.IO.FileInfo file = new System.IO.FileInfo( databaseFolder + n + ".db" );
		if (file.Exists)
		{
			Debug.LogWarning( n+".db deleted" );
			file.Delete( );
		}
		else
		{
			Debug.LogWarning( n+".db does not exist" );
		}
	}


}
