using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileManager{
    
    static StreamWriter albumStatsFileWritter = File.CreateText("Assets/Logs/albumGameStatsLog.txt");
    static StreamWriter playerStatsFileWritter = File.CreateText("Assets/Logs/playerGameStatsLog.txt");
    static StreamWriter playerActionsLogFileWritter = File.CreateText("Assets/Logs/playerActionsLog.txt");

    static public void InitWriter()
    {
        albumStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"AlbumId\";\"AlbumName\";\"MState\"");
        playerStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Money\"");
        playerActionsLogFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Event Type\";\"Instrument\";\"Value\"");
    }

    static public void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState) {
        albumStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
    }
    static public void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        playerStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
    }
    static public void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins)
    {
        playerActionsLogFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";"+ instrument + ";"+ coins);
    }

    static public void CloseWriter()
    {
        albumStatsFileWritter.Close();
        playerStatsFileWritter.Close();
        playerActionsLogFileWritter.Close();
    }

}
