using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileManager{
    
    static StreamWriter gameLogFileWritter = File.CreateText("Assets/Logs/gameLog.txt");
    static StreamWriter playersLogFileWritter = File.CreateText("Assets/Logs/playersLog.txt");

    static public void InitWriter()
    {
        for (int i = 0; i < GameProperties.numberOfAlbumsPerGame; i++)
        {
            if (i > 0)
            {
                gameLogFileWritter.Write(";");
            }
            gameLogFileWritter.Write("\"album_" + i + "_MState\"");

            gameLogFileWritter.Write(";");
            for (int j = 0; j < GameProperties.numberOfPlayersPerGame; j++)
            {
                if (j > 0)
                {
                    gameLogFileWritter.Write(";");
                }
                gameLogFileWritter.Write("\"player_" + j + "_money\"");
            }
        }
        gameLogFileWritter.WriteLine();

        playersLogFileWritter.WriteLine("\"GameId\";\"Name\";\"Event Type\";\"Instrument\";\"Value\"");
    }

    static public void WritePlayerActionToLog(string currGameId, string playerName, string eventType, string instrument, string coins)
    {
        playersLogFileWritter.WriteLine(currGameId + ";" + playerName + ";" + eventType + ";"+ instrument + ";"+ coins);

    }

    static public void WriteAlbumResultsToLog() {

        for (int i = 0; i < GameProperties.numberOfAlbumsPerGame; i++)
        {
            Album currAlbum = GameGlobals.albums[i];
            if (i > 0)
            {
                gameLogFileWritter.Write(";");
            }
            gameLogFileWritter.Write(System.Enum.GetNames(typeof(GameProperties.AlbumMarketingState))[(int)currAlbum.GetMarketingState()]);
        }
        gameLogFileWritter.WriteLine();
    }

    static public void CloseWriter()
    {
        gameLogFileWritter.Close();
        playersLogFileWritter.Close();
    }

}
