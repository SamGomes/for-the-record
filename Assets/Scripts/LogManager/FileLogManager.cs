using System;
using System.IO;
using UnityEngine;

//File log manager
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
        Directory.CreateDirectory(directoryPath);

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

    public string GetLastSessionConditionFromLog(Func<int> yieldedReactionToGet)
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
