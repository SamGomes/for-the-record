using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileManager{
    
    static StreamWriter gameLogFileWritter = File.CreateText("Assets/Logs/gameLog.txt");

    static public void InitWriter()
    {
        gameLogFileWritter.WriteLine("albumId;albumMState");
    }

    // Use this for initialization
    static public void WriteGameResultsToLog() {

        for (int i =0; i < GameManager.albums.Count; i++)
        {
            Album currAlbum = GameManager.albums[i];
            gameLogFileWritter.WriteLine(i+";"+System.Enum.GetNames(typeof(GameProperties.AlbumMarketingState))[(int)currAlbum.GetMarketingState()]);
        }
    }
    static public void CloseWriter()
    {
        gameLogFileWritter.Close();
    }

}
