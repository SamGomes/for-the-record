using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//file log manager
public abstract class LogManager
{
    abstract public void InitLogs();
    abstract public void WritePlayerToLog(string sessionId, string gameId, string playerId, string playername, string type);
    abstract public void WriteGameToLog(string sessionId, string gameId, string result);
    abstract public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState);
    abstract public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money);
    abstract public void WritePlayerActionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins);
    abstract public void CloseLogs();
}

//debug log manager
public class DebugLogManager : LogManager
{
    public override void InitLogs()
    {
        Debug.Log("Log Initialzed.");
    }
    public override void WritePlayerToLog(string sessionId, string currGameId, string playerId, string playername, string type)
    {
        Debug.Log("WritePlayerToLog: " + sessionId + ";" + currGameId + ";" + playerId + ";" + playername + ";" + type);
    }
    public override void WriteGameToLog(string sessionId, string currGameId, string result)
    {
        Debug.Log("WriteGameToLog: " + sessionId + ";" + currGameId + ";" + result);
    }
    public override void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState)
    {
        Debug.Log("WriteAlbumResultsToLog: " + sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
    }
    public override void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        Debug.Log("WritePlayerResultsToLog: " + sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
    }
    public override void WritePlayerActionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        Debug.Log("WritePlayerActionToLog: " + sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + skill + ";" + coins);
    }

    public override void CloseLogs()
    {
        Debug.Log("Log Closed.");
    }

}


//file log manager
public class FileLogManager : LogManager
{
    private StreamWriter albumStatsFileWritter;
    private StreamWriter playersLogFileWritter;
    private StreamWriter playerStatsFileWritter;
    private StreamWriter gameStatsFileWritter;
    private StreamWriter playerActionsLogFileWritter;

    
    public override void InitLogs()
    {
        StreamWriter albumStatsFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/albumGameStatsLog.txt");
        StreamWriter playersLogFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/playerStatsLog.txt");
        StreamWriter playerStatsFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/playerGameStatsLog.txt");
        StreamWriter gameStatsFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/gameStatsLog.txt");
        StreamWriter playerActionsLogFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/playerActionsLog.txt");

        albumStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"AlbumId\";\"AlbumName\";\"MState\"");
        playersLogFileWritter.WriteLine("\"SessionId\";\"GameId\";\"PlayerId\";\"PlayerName\";\"AIType\"");
        playerStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Money\"");
        gameStatsFileWritter.WriteLine("\"SessionId\";\"GameId\";\"Result\"");
        playerActionsLogFileWritter.WriteLine("\"SessionId\";\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Event Type\";\"Instrument\";\"Value\"");
    }
    public override void WritePlayerToLog(string sessionId, string currGameId, string playerId, string playername, string type)
    {
        //prevent access after disposal
        if (playersLogFileWritter != null)
        {
            playersLogFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + playerId + ";" + playername + ";" + type);
        }
    }
    public override void WriteGameToLog(string sessionId, string currGameId, string result)
    {
        //prevent access after disposal
        if (gameStatsFileWritter != null)
        {
            gameStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + result);
        }
    }
    public override void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState)
    {
        //prevent access after disposal
        if (albumStatsFileWritter != null)
        {
            albumStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
        }
    }
    public override void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        //prevent access after disposal
        if (playerStatsFileWritter != null)
        {
            playerStatsFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
        }
    }
    public override void WritePlayerActionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        //prevent access after disposal
        if (playerActionsLogFileWritter != null)
        {
            playerActionsLogFileWritter.WriteLine(sessionId + ";" + currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + skill + ";" + coins);
        }
    }

    public override void CloseLogs()
    {
        gameStatsFileWritter.Close();
        albumStatsFileWritter.Close();
        playersLogFileWritter.Close();
        playerStatsFileWritter.Close();
        playerActionsLogFileWritter.Close();
    }

}

//google forms log manager
public class GoogleFormsLogManager : LogManager
{
    public override void InitLogs() { }

    public override void WritePlayerToLog(string sessionId, string gameId, string playerId, string playerName, string type)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSfyUPtS4_dN6iJaQgKSfKhqNZH0tCyfMto_jyvApmumYkFuBg/viewform?usp=pp_url&entry.1243275873="+ sessionId + "&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+type+"&submit=Submit\", \"_blank\")).close()");
    }
    public override void WriteGameToLog(string sessionId, string gameId, string result)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSeL5rFO12-RniFZU3HtKHhJMg-jh6pqMqjc5rWAbgSFc3qKsg/formResponse?usp=pp_url&entry.1243275873="+ sessionId + "&entry.67037947=" + gameId+"&entry.1708403356="+result+"&submit=Submit\", \"_blank\")).close()");
    }
    public override void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSffdgyATkT9In487OPjpQ-sXPLDtXcKy6jSdIsWbOLk1dAbyQ/formResponse?ifq&entry.1243275873="+ sessionId + "&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+currAlbumId+"&entry.416810127="+currAlbumName+"&entry.724890801="+marketingState+ "&submit=Submit\", \"_blank\")).close()");
    }
    public override void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSddPLzrQO1J_9vBGmGpjs1FOIGn3Z92fw23X-otjNQLG7cpSg/formResponse?usp=pp_url&entry.1243275873="+ sessionId + "&entry.67037947=" + currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+money+"&submit=Submit\", \"_blank\")).close()");

    }
    public override void WritePlayerActionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLScYaTNzROIoL4P6D40B_mcpM1xZuXdJBJz_neHCvCxf0qWpLA/formResponse?usp=pp_url&entry.1243275873="+ sessionId + "&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+eventType+"&entry.1712145275="+skill+"&entry.877028457="+coins+"&submit=Submit\", \"_blank\")).close()");

    }

    public override void CloseLogs()
    {
    }
}

//mySQL log manager
public class MySQLLogManager : LogManager
{
    private string phpLogServerConnectionPath;
    private string databaseName;

    public override void InitLogs()
    {
        phpLogServerConnectionPath = "http://localhost:3000/dbActions.php";
        databaseName = "for_the_record_logs";
    }
    public override void WritePlayerToLog(string sessionId, string gameId, string playerId, string playerName, string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "players_log");

        string[] keys = { "sessionId", "gameId", "playerId", "playerName", "type" };
        string[] values = { sessionId, gameId, playerId, playerName, type };

        PassTableArguments(form, "arrFields", "arrValues", keys,values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
        
    }
    public override void WriteGameToLog(string sessionId, string gameId, string result)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "sessionId", "gameId", "result" };
        string[] values = { sessionId , gameId, result };

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
    }
    public override void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "album_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "albumId", "albumName", "marketingState" };
        string[] values = { sessionId, currGameId, currGameRoundId, currAlbumId, currAlbumName, marketingState};

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
    }
    public override void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "player_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "money" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, money };

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
    }
    public override void WritePlayerActionToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins) {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "player_actions_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "eventType", "skill", "coins" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, eventType, skill, coins };

        PassTableArguments(form, "arrFields", "arrValues", keys, values);
        WWW phpConnection = new WWW(phpLogServerConnectionPath, form);
    }
    public override void CloseLogs() { }

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