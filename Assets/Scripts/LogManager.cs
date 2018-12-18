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
    void WriteGameToLog(string sessionId, string gameId, string result);
    void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState);
    void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money);
    void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins);
    void WriteChangeDecisionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string initialDecision, string state);
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
    public void WriteGameToLog(string sessionId, string currGameId, string result)
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
    public void WriteChangeDecisionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string initialDecision, string state)
    {
        Debug.Log("ChangeDecisionLog: " + sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + initialDecision + ";" + state);
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
    private StreamWriter changeDecisionLogFileWritter;

    private bool isInitialized = false;
    
    public void InitLogs()
    {
        if (isInitialized)
        {
            return;
        }

        string dateTime = DateTime.Now.ToString("yyyy/MM/dd/HH-mm-ss");

        string directoryPath = Application.dataPath + "/Logs/" + dateTime;
		Directory.CreateDirectory (directoryPath);

        Debug.Log(directoryPath);

		albumStatsFileWritter = File.CreateText(directoryPath + "/albumGameStatsLog.txt");
		playersLogFileWritter = File.CreateText(directoryPath + "/playerStatsLog.txt");
		playerStatsFileWritter = File.CreateText(directoryPath + "/playerGameStatsLog.txt");
		gameStatsFileWritter = File.CreateText(directoryPath + "/gameStatsLog.txt");
		eventsLogFileWritter = File.CreateText(directoryPath + "/eventsLog.txt");
        changeDecisionLogFileWritter = File.CreateText(directoryPath + "/changeDecisionLog.txt");

        albumStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"AlbumId\";\"AlbumName\";\"MState\"");
        playersLogFileWritter.WriteLine("\"SessionId\";\"GameId\";\"PlayerId\";\"PlayerName\";\"AIType\"");
        playerStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Money\"");
        gameStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"Result\"");
        eventsLogFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Event Type\";\"Instrument\";\"Value\"");
        changeDecisionLogFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Initial Decision\";\"State\"");

        FlushLogs();
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
    public void WriteGameToLog(string sessionId, string currGameId, string result)
    {
        //prevent access after disposal
        if (gameStatsFileWritter != null)
        {
            gameStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + result);
        }
        gameStatsFileWritter.Flush();
    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState)
    {
        //prevent access after disposal
        if (albumStatsFileWritter != null)
        {
            albumStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
        }
        albumStatsFileWritter.Flush();
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        //prevent access after disposal
        if (playerStatsFileWritter != null)
        {
            playerStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
        }
        playerStatsFileWritter.Flush();
    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        //prevent access after disposal
        if (eventsLogFileWritter != null)
        {
            eventsLogFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + skill + ";" + coins);
        }
        eventsLogFileWritter.Flush();
    }
    public void WriteChangeDecisionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string initialDecision, string state)
    {
        //prevent access after disposal
        if (changeDecisionLogFileWritter != null)
        {
            changeDecisionLogFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + initialDecision + ";" + state);
        }
        changeDecisionLogFileWritter.Flush();
    }

    private void FlushLogs()
    {
        gameStatsFileWritter.Flush();
        albumStatsFileWritter.Flush();
        playersLogFileWritter.Flush();
        playerStatsFileWritter.Flush();
        eventsLogFileWritter.Flush();
        changeDecisionLogFileWritter.Flush();
    }

    public void EndLogs()
    {
        FlushLogs();
    }

}

//google forms log manager
public class GoogleFormsLogManager : ILogManager
{
    public void InitLogs() { }

    public void WritePlayerToLog(string sessionId, string gameId, string playerId, string playerName, string type)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSfyUPtS4_dN6iJaQgKSfKhqNZH0tCyfMto_jyvApmumYkFuBg/viewform?usp=pp_url&entry.1243275873="+ sessionId + "&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+type+"&submit=Submit\", \"_blank\")).close()");
    }
    public void WriteGameToLog(string sessionId, string gameId, string result)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSeL5rFO12-RniFZU3HtKHhJMg-jh6pqMqjc5rWAbgSFc3qKsg/formResponse?usp=pp_url&entry.1243275873="+ sessionId + "&entry.67037947=" + gameId+"&entry.1708403356="+result+"&submit=Submit\", \"_blank\")).close()");
    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSffdgyATkT9In487OPjpQ-sXPLDtXcKy6jSdIsWbOLk1dAbyQ/formResponse?ifq&entry.1243275873="+ sessionId + "&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+currAlbumId+"&entry.416810127="+currAlbumName+"&entry.724890801="+marketingState+ "&submit=Submit\", \"_blank\")).close()");
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSddPLzrQO1J_9vBGmGpjs1FOIGn3Z92fw23X-otjNQLG7cpSg/formResponse?usp=pp_url&entry.1243275873="+ sessionId + "&entry.67037947=" + currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+money+"&submit=Submit\", \"_blank\")).close()");

    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLScYaTNzROIoL4P6D40B_mcpM1xZuXdJBJz_neHCvCxf0qWpLA/formResponse?usp=pp_url&entry.1243275873="+ sessionId + "&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+eventType+"&entry.1712145275="+skill+"&entry.877028457="+coins+"&submit=Submit\", \"_blank\")).close()");

    }
    public void WriteChangeDecisionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string initialDecision, string state)
    {

    }

    public void EndLogs()
    {
    }
}

//mySQL log manager
public class MySQLLogManager : ILogManager
{
    private string phpLogServerConnectionPath;
    private string databaseName;

    public void InitLogs()
    {
        phpLogServerConnectionPath = "http://localhost:3000/dbActions.php";
        databaseName = "for_the_record_logs";
    }
    public void WritePlayerToLog(string sessionId, string gameId, string playerId, string playerName, string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "players_log");

        string[] keys = { "sessionId", "gameId", "playerId", "playerName", "type" };
        string[] values = { sessionId, gameId, playerId, playerName, type };

        PassTableArguments(form, "arrFields", "arrValues", keys,values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: "+phpConnection.error);
        
    }
    public void WriteGameToLog(string sessionId, string gameId, string result)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "sessionId", "gameId", "result" };
        string[] values = { sessionId , gameId, result };

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "album_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "albumId", "albumName", "marketingState" };
        string[] values = { sessionId, currGameId, currGameRoundId, currAlbumId, currAlbumName, marketingState};

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "player_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "money" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, money };

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "events_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "eventType", "skill", "coins" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, eventType, skill, coins };

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        Debug.Log("php error: " + phpConnection.error);

    }

    public void WriteChangeDecisionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string initialDecision, string state)
    {

    }

    public void EndLogs() { }

    private void PassTableArguments(WWWForm form, string keysTableName, string valuesTableName, string[] keys, string[] values)
    {
        for(int i=0; i < keys.Length; i++)
        {
            form.AddField(keysTableName+"[]", keys[i]);
        }
        for(int i=0; i< values.Length; i++)
        {
            form.AddField(valuesTableName+"[]", values[i]);
        }
    }
}
