using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//file log manager
using System;


public interface ILogManager
{
    void InitLogs();
    void WritePlayerToLog(string sessionId, string gameId, string playerId, string playername, string type);
    void WriteGameToLog(string sessionId, string gameId, string condition, string result);
    void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState);
    void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money);
    void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins);

    string GetLastSessionConditionFromLog();
    void EndLogs();
}

//debug log manager
public class DebugLogManager : ILogManager
{
    public void InitLogs()
    {
        Debug.Log("Log Initialzed.");
    }
    public void WritePlayerToLog(string sessionId, string currGameId, string playerId, string playername, string type)
    {
        Debug.Log("WritePlayerToLog: " + sessionId + ";" + currGameId + ";" + playerId + ";" + playername + ";" + type);
    }
    public void WriteGameToLog(string sessionId, string currGameId, string condition, string result)
    {
        Debug.Log("WriteGameToLog: " + sessionId + ";" + currGameId + ";" + result);
    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState)
    {
        Debug.Log("WriteAlbumResultsToLog: " + sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        Debug.Log("WritePlayerResultsToLog: " + sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        Debug.Log("WriteEventToLog: " + sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + skill + ";" + coins);
    }


    public string GetLastSessionConditionFromLog()
    {
        Debug.Log("GotLastSessionConditionFromLog");
        return "A";
    }

    public void EndLogs()
    {
        Debug.Log("Log Closed.");
    }

}


//file log manager
public class FileLogManager : ILogManager
{
    private StreamWriter albumStatsFileWritter;
    private StreamWriter playersLogFileWritter;
    private StreamWriter playerStatsFileWritter;
    private StreamWriter gameStatsFileWritter;
    private StreamWriter eventsLogFileWritter;

    private bool isInitialized = false;
    
    public void InitLogs()
    {
        if (isInitialized)
        {
            return;
        }

        string dateTime = DateTime.Now.ToString("yyyy/MM/dd/HH-mm-ss");

        string directoryPath = Application.dataPath + "./Logs/" + dateTime;
		Directory.CreateDirectory (directoryPath);

		albumStatsFileWritter = File.CreateText(directoryPath + "/albumGameStatsLog.txt");
		playersLogFileWritter = File.CreateText(directoryPath + "/playerStatsLog.txt");
		playerStatsFileWritter = File.CreateText(directoryPath + "/playerGameStatsLog.txt");
		gameStatsFileWritter = File.CreateText(directoryPath + "/gameStatsLog.txt");
		eventsLogFileWritter = File.CreateText(directoryPath + "/eventsLog.txt");

        albumStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"AlbumId\";\"AlbumName\";\"MState\"");
        playersLogFileWritter.WriteLine("\"SessionId\";\"GameId\";\"PlayerId\";\"PlayerName\";\"AIType\"");
        playerStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Money\"");
        gameStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"Result\"");
        eventsLogFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Event Type\";\"Instrument\";\"Value\"");

        isInitialized = true;
    }
    public void WritePlayerToLog(string sessionId, string currGameId, string playerId, string playername, string type)
    {
        //prevent access after disposal
        if (playersLogFileWritter != null)
        {
            playersLogFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + playerId + ";" + playername + ";" + type);
        }
        FlushLogs();
    }
    public void WriteGameToLog(string sessionId, string currGameId, string condition, string result)
    {
        //prevent access after disposal
        if (gameStatsFileWritter != null)
        {
            gameStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + condition + ";" + result);
        }
        FlushLogs();
    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState)
    {
        //prevent access after disposal
        if (albumStatsFileWritter != null)
        {
            albumStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
        }
        FlushLogs();
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        //prevent access after disposal
        if (playerStatsFileWritter != null)
        {
            playerStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
        }
        FlushLogs();
    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        //prevent access after disposal
        if (eventsLogFileWritter != null)
        {
            eventsLogFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + skill + ";" + coins);
        }
        FlushLogs();
    }


    public string GetLastSessionConditionFromLog()
    {
        Debug.Log("GotLastSessionConditionFromLog");
        return "A";
    }


    private bool FlushLogs()
    {
        gameStatsFileWritter.Flush();
        albumStatsFileWritter.Flush();
        playersLogFileWritter.Flush();
        playerStatsFileWritter.Flush();
        eventsLogFileWritter.Flush();
        return true;
    }

    public void EndLogs()
    {
        FlushLogs();
    }

}

//mySQL log manager
public class MySQLLogManager : ILogManager
{
    private string phpLogServerConnectionPath;
    private string databaseName;

    private string currentGetResponse;

    public void InitLogs()
    {
        phpLogServerConnectionPath = "http://localhost:4000/dbActions.php";
        databaseName = "for_the_record_logs";
        currentGetResponse = "";
    }
    public void WritePlayerToLog(string sessionId, string gameId, string playerId, string playerName, string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "players_log");

        string[] keys = { "sessionId", "gameId", "playerId", "playerName", "type" };
        string[] values = { sessionId, gameId, playerId, playerName, type };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: "+phpConnection.error);

        while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WriteGameToLog(string sessionId, string gameId, string gameCondition, string result)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "sessionId", "gameId", "gameCondition" , "result" };
        string[] values = { sessionId , gameId, gameCondition, result };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

        while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "album_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "albumId", "albumName", "marketingState" };
        string[] values = { sessionId, currGameId, currGameRoundId, currAlbumId, currAlbumName, marketingState};

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

        while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "player_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "money" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, money };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

        while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "player_actions_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "eventType", "skill", "coins" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, eventType, skill, coins };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

        while (!phpConnection.isDone) { } //wait until php is done
    }

    public string GetLastSessionConditionFromLog()
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "SELECT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "gameCondition" };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, new string[]{ "0" }, "WHERE timestampedId >= ALL( SELECT timestampedId FROM game_stats_log )");
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

        //return phpConnection.text;

        while (!phpConnection.isDone) { } //wait until php is done
        return phpConnection.text;
    }
    

    public void EndLogs() { }

    private void PassTableArguments(WWWForm form, string keysTableName, string valuesTableName, string extraParamsName, string[] keys, string[] values, string extraParams)
    {
        for(int i=0; i < keys.Length; i++)
        {
            form.AddField(keysTableName+"[]", keys[i]);
        }
        for(int i=0; i< values.Length; i++)
        {
            form.AddField(valuesTableName+"[]", values[i]);
        }
        form.AddField(extraParamsName, extraParams);
    }
}