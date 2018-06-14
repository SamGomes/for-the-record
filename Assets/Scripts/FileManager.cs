using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileManager{
    
    static StreamWriter gameLogFileWritter = File.CreateText("Assets/Logs/gameLog.txt");

    static public void InitWriter()
    {
        for (int i = 0; i < GameProperties.numberOfAlbumsPerGame; i++)
        {
            if (i > 0)
            {
                gameLogFileWritter.Write(";");
            }
            gameLogFileWritter.Write("\"album_\""+i+"\"_MState\"");
        }
        gameLogFileWritter.WriteLine();
    }

    // Use this for initialization
    static public void WriteGameResultsToLog() {

        for (int i = 0; i < GameProperties.numberOfAlbumsPerGame; i++)
        {
            Album currAlbum = GameManager.albums[i];
            if (i > 0)
            {
                gameLogFileWritter.Write(";");
            }
            gameLogFileWritter.Write("\""+System.Enum.GetNames(typeof(GameProperties.AlbumMarketingState))[(int)currAlbum.GetMarketingState()]+"\"");
        }
        gameLogFileWritter.WriteLine();
    }

    static public void CloseWriter()
    {
        gameLogFileWritter.Close();
    }

}
