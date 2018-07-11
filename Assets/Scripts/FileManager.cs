using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileManager{
    
    static StreamWriter albumStatsFileWritter = File.CreateText("Assets/Logs/albumGameStatsLog.txt");
    static StreamWriter playersLogFileWritter = File.CreateText("Assets/Logs/playerStatsLog.txt");
    static StreamWriter playerStatsFileWritter = File.CreateText("Assets/Logs/playerGameStatsLog.txt");
    static StreamWriter gameStatsFileWritter = File.CreateText("Assets/Logs/gameStatsLog.txt");
    static StreamWriter playerActionsLogFileWritter = File.CreateText("Assets/Logs/playerActionsLog.txt");

    static public void InitWriter()
    {
        albumStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"AlbumId\";\"AlbumName\";\"MState\"");
        playersLogFileWritter.WriteLine("\"PlayerId\";\"PlayerName\";\"AIType\"");
        playerStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Money\"");
        gameStatsFileWritter.WriteLine("\"GameId\";\"Result\"");
        playerActionsLogFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Event Type\";\"Instrument\";\"Value\"");
    }
    static public void WritePlayerToLog(string playerId, string playername, string type)
    {
        //prevent access after disposal
        if (playersLogFileWritter != null)
        {
            playersLogFileWritter.WriteLine(playerId + ";" + playername + ";" + type);
        }
    }
    static public void WriteGameToLog(string gameId, string result)
    {
        //prevent access after disposal
        if (gameStatsFileWritter != null)
        {
            gameStatsFileWritter.WriteLine(gameId + ";" + result);
        }
    }
    static public void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState) {
        //prevent access after disposal
        if (albumStatsFileWritter != null)
        {
            albumStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
        }
    }
    static public void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        //prevent access after disposal
        if (playerStatsFileWritter != null)
        {
            playerStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
        }
    }
    static public void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins)
    {
        //prevent access after disposal
        if (playerActionsLogFileWritter != null)
        {
            playerActionsLogFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + instrument + ";" + coins);
        }
    }

    static public void CloseWriter()
    {
        gameStatsFileWritter.Close();
        albumStatsFileWritter.Close();
        playersLogFileWritter.Close();
        playerStatsFileWritter.Close();
        playerActionsLogFileWritter.Close();
    }

}
