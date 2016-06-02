using UnityEngine;
using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class SqliteUtils : RJWard.Core.Singleton.SingletonApplicationLifetimeLazy< SqliteUtils >
{
	public static readonly bool DEBUG_SQL = true;

	private Dictionary<string, SqliteConnection> storedConnections_ = new Dictionary<string, SqliteConnection>( );
	private string databaseFolder_ = "ERR_DBFOLDER_NOT_SET";

	public Action databaseLoadComplete;

	private List<string> databaseList_;
	private int numLoadedDatabases_;
//	private static string language;

	private RJWard.Core.Version.VersionNumber previousVersionNumber_ = null;

	protected override void PostAwake( )
	{
		databaseFolder_ = Application.persistentDataPath + "/Data";
	}

	private string getDatabaseFilename( string databaseName )
	{
		return databaseName + ".db";
	}

	private string getDatabasePath( string databaseName)
	{
        return databaseFolder_ +"/" + getDatabaseFilename( databaseName );
	}

	public static string streamingAssetsPath
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return "jar:file://" + Application.dataPath + "!/assets";
			}
			else
			{
				return "file://" + Application.streamingAssetsPath;
			}
		}
	}

	protected override void PostOnDestroy( )
	{
		foreach (KeyValuePair<string, SqliteConnection> pair in storedConnections_)
		{
			pair.Value.Close();
			pair.Value.Dispose();
		}
	}


	public static SqliteParameter newSqlParameter( object value )
	{
		SqliteParameter sqlParameter = new SqliteParameter( );
		sqlParameter.Value = value;
		return sqlParameter;
	}

	public SqliteConnection getConnection( string databaseName )
	{
		if (storedConnections_.ContainsKey( databaseName ))
		{
			if (DEBUG_SQL)
			{
				Debug.Log( "SqliteUtils found connection to '" + databaseName + "'" );
			}
			return storedConnections_[databaseName];
		}

		SqliteConnection connection;
		connection = new SqliteConnection( "URI=file:" + getDatabasePath(databaseName) );
		connection.Open( );
		storedConnections_[databaseName] = connection;
		if (DEBUG_SQL)
		{
			Debug.Log( "SqliteUtils created connection to '" + databaseName + "'" );
		}
		return connection;
	}

	public void initialiseDatabases( string language )
	{
		if (DEBUG_SQL)
		{
			Debug.Log( "SQL: initialiseDatabases( " + language + " ): Language not implemented" );
		}

		if (!Directory.Exists( databaseFolder_ ))
		{
			if (DEBUG_SQL)
			{
				Debug.Log( "Creating database folder '" + databaseFolder_ + "'" );
			}
			Directory.CreateDirectory( databaseFolder_ );
		}

//		SqliteUtils.language = language;
		getOriginalSettings( );

		if (previousVersionNumber_ == null)
		{
			Debug.LogError( "null previous version number" );
		}
		if (RJWard.Core.Version.DEBUG_VERSION)
		{
			Debug.Log( "THIS VERSION = " + RJWard.Core.Version.versionNumber.DebugDescribe( )
					  + "\nPREVIOUS = " + previousVersionNumber_.DebugDescribe( )
					  + " (BEFORE = " + previousVersionNumber_.Before( RJWard.Core.Version.versionNumber ) + " )" );
		}

		databaseList_ = new List<string>( );

		if (previousVersionNumber_.Before( RJWard.Core.Version.versionNumber ))
		{
//			databaseList.Add( language );
			if (DEBUG_SQL)
			{
				Debug.Log( "Previous version is older so copying DBs from StreamingAssets");
			}
			databaseList_.Add( "CoreData" );
		}
		else
		{
			if (!File.Exists( getDatabasePath("CoreData" )))
			{
				if (DEBUG_SQL)
				{
					Debug.Log( "CoreData does not exist so copying DBs from StreamingAssets" );
				}
				databaseList_.Add( "CoreData" );
			}
			else
			{
				if (DEBUG_SQL)
				{
					Debug.Log( "DB file '" + getDatabasePath( "CoreData" ) + "' exists and version is not new" );
				}
			}
			/*
			if (!File.Exists( Application.persistentDataPath + "/Data/" + language + ".db" ))
			{
				databaseList.Add( language );
			}
			*/
		}

		if (databaseList_.Count > 0)
		{
			// replace databases with those in bundle
			numLoadedDatabases_ = 0;
			if (DEBUG_SQL)
			{
				Debug.Log( "Copying " + databaseList_.Count+ " DBs from StreamingAssets" );
			}

			for (int i = 0; i < databaseList_.Count; i++)
			{
				StartCoroutine( copyDatabaseFromBundle( databaseList_[i] ) );
			}
		}
		else
		{
			databaseFilesCopied( );
		}
	}

	private void getOriginalSettings( )
	{
		if (DEBUG_SQL)
		{
			Debug.Log( "SQL: getOriginalSettings()" );
		}

		// check if settings exists

		string progressPath = getDatabasePath("Progress");
		if (File.Exists( progressPath ))
		{
			SqliteConnection connection = new SqliteConnection( "URI=file:" + progressPath );
			connection.Open( );
			SqliteCommand query = connection.CreateCommand( );

			query.CommandText = "SELECT value FROM settings WHERE id=?";
			SqliteParameter valueSql = new SqliteParameter( );
			query.Parameters.Add( valueSql );

			SqliteDataReader reader = null;

			valueSql.Value = SettingsIds.versionNumber;
			reader = query.ExecuteReader( );

			string previousVersionString = string.Empty;

			if (reader.Read( ))
			{
				previousVersionString = reader.GetString( 0 );
				previousVersionNumber_ = new RJWard.Core.Version.VersionNumber( previousVersionString );
			}
			reader.Close( );

			query.Dispose( );
		}
		else
		{
			Debug.LogWarning( "No settings table in getOriginalSettings" );
		}
		if (previousVersionNumber_ == null)
		{
			previousVersionNumber_ = new RJWard.Core.Version.VersionNumber( 0, 0, 0, 0 );
			Debug.Log( "No previousVersionNumber, defaulting to " + previousVersionNumber_.ToString( ) );
		}
	}

	private IEnumerator copyDatabaseFromBundle( string databaseName )
	{
		if (DEBUG_SQL)
		{
			Debug.Log( "SQL: copyDatabaseFromBundle( " + databaseName + " )" );
		}

		string dbPath = getDatabasePath( databaseName );

		if (File.Exists( dbPath ))
		{
			File.Delete( dbPath );
		}

		WWW wwwFile = new WWW( streamingAssetsPath + "/Data/" + getDatabaseFilename( databaseName ));
		yield return wwwFile;

		//Save to persistent data path
		File.WriteAllBytes( dbPath, wwwFile.bytes );

#if UNITY_IPHONE
		iPhone.SetNoBackupFlag( dataBasePath );
#endif

		//Check if we have loaded all databases
		numLoadedDatabases_++;
		if (numLoadedDatabases_ == databaseList_.Count)
		{
			databaseFilesCopied( );
		}
	}

	private void databaseFilesCopied( )
	{
		numLoadedDatabases_ = 0;
		onOpenConnections( );
	}

	private void onOpenConnections( )
	{
		// Debug.Log ("onOpenConnections");

//		SqliteUtils.Instance.getConnection( language );
		SqliteUtils.Instance.getConnection( "CoreData" );
		SqliteUtils.Instance.getConnection( "Progress" );

		prepareProgressTable( );

		if (databaseLoadComplete != null)
		{
			databaseLoadComplete( );
		}
	}

	private void prepareProgressTable( )
	{
		//		Debug.Log ("prepareProgressTable");
		createSettingsTable( );
	}

	private void createSettingsTable( )
	{
		SqliteConnection connection = getConnection( "Progress" );
		SqliteCommand create = connection.CreateCommand( );
		create.CommandText = "CREATE TABLE IF NOT EXISTS settings (" +
			"id TEXT PRIMARY KEY, " +
				"value TEXT " +
				")";

		create.ExecuteNonQuery( );
		create.Dispose( );

		SqliteCommand insert = connection.CreateCommand( );
		insert.CommandText = "INSERT OR IGNORE INTO settings ( id, value ) VALUES ( ?, ? )";
		SqliteParameter idSql = new SqliteParameter( );
		SqliteParameter valueSql = new SqliteParameter( );
		insert.Parameters.Add( idSql );
		insert.Parameters.Add( valueSql );

		foreach (KeyValuePair<string, string> entry in SettingsIds.defaults)
		{
			idSql.Value = entry.Key;
			if (SettingsIds.encrypted.Contains( entry.Key ))
			{
				Debug.LogError( "Encryption not implemented" ); // See SS Code below
//				valueSql.Value = EncryptionHelper.EncryptString( entry.Key + entry.Value );
			}
			else
			{
				valueSql.Value = entry.Value;
			}
			insert.ExecuteNonQuery( );
		}
		insert.Dispose( );

		SqliteCommand replace = connection.CreateCommand( );
		replace.CommandText = "REPLACE INTO settings ( id, value ) VALUES ( ?, ? )";
		SqliteParameter id2Sql = new SqliteParameter( );
		SqliteParameter value2Sql = new SqliteParameter( );
		replace.Parameters.Add( id2Sql );
		replace.Parameters.Add( value2Sql );

		id2Sql.Value = SettingsIds.versionNumber;
		value2Sql.Value = RJWard.Core.Version.versionNumber.ToString();
		replace.ExecuteNonQuery( );

		//		Debug.Log("Set DB version number to "+AppConfig.version.ToString());

		replace.Dispose( );
	}



}
