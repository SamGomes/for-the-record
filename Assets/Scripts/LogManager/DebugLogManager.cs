using System;
using UnityEngine;

//Debug log manager
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
    public void UpdateGameResultInLog(string sessionId, string gameId, string condition, string result)
    {
        Debug.Log("UpdateGameInLog: " + sessionId + ";" + gameId + ";" + result);
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

    public void GetLastSessionConditionFromLog(Func<string,int> yieldedReactionToGet)
    {
        Debug.Log("GotLastSessionConditionFromLog");
        yieldedReactionToGet("B");
    }

    public void EndLogs()
    {
        Debug.Log("Log Closed.");
    }
}

