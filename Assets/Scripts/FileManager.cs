using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//file log manager
//public static class FileManager{

//    static StreamWriter albumStatsFileWritter = File.CreateText(Application.dataPath+ "/Assets/Logs/albumGameStatsLog.txt");
//    static StreamWriter playersLogFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/playerStatsLog.txt");
//    static StreamWriter playerStatsFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/playerGameStatsLog.txt");
//    static StreamWriter gameStatsFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/gameStatsLog.txt");
//    static StreamWriter playerActionsLogFileWritter = File.CreateText(Application.dataPath + "/Assets/Logs/playerActionsLog.txt");

//    static public void InitWriter()
//    {
//        albumStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"AlbumId\";\"AlbumName\";\"MState\"");
//        playersLogFileWritter.WriteLine("\"PlayerId\";\"PlayerName\";\"AIType\"");
//        playerStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Money\"");
//        gameStatsFileWritter.WriteLine("\"GameId\";\"Result\"");
//        playerActionsLogFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Event Type\";\"Instrument\";\"Value\"");
//    }
//    static public void WritePlayerToLog(string playerId, string playername, string type)
//    {
//        //prevent access after disposal
//        if (playersLogFileWritter != null)
//        {
//            playersLogFileWritter.WriteLine(playerId + ";" + playername + ";" + type);
//        }
//    }
//    static public void WriteGameToLog(string gameId, string result)
//    {
//        //prevent access after disposal
//        if (gameStatsFileWritter != null)
//        {
//            gameStatsFileWritter.WriteLine(gameId + ";" + result);
//        }
//    }
//    static public void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState) {
//        //prevent access after disposal
//        if (albumStatsFileWritter != null)
//        {
//            albumStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
//        }
//    }
//    static public void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money)
//    {
//        //prevent access after disposal
//        if (playerStatsFileWritter != null)
//        {
//            playerStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
//        }
//    }
//    static public void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins)
//    {
//        //prevent access after disposal
//        if (playerActionsLogFileWritter != null)
//        {
//            playerActionsLogFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + instrument + ";" + coins);
//        }
//    }

//    static public void CloseWriter()
//    {
//        gameStatsFileWritter.Close();
//        albumStatsFileWritter.Close();
//        playersLogFileWritter.Close();
//        playerStatsFileWritter.Close();
//        playerActionsLogFileWritter.Close();
//    }

//}




//google forms log manager
public static class FileManager
{
    static public void InitWriter() { }

    static public void WritePlayerToLog(string playerId, string playerName, string type)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSfyUPtS4_dN6iJaQgKSfKhqNZH0tCyfMto_jyvApmumYkFuBg/viewform?usp=pp_url&entry.1243275873=SessionId&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+type+"&submit=Submit\", \"_blank\")).close()");
    }
    static public void WriteGameToLog(string gameId, string result)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSeL5rFO12-RniFZU3HtKHhJMg-jh6pqMqjc5rWAbgSFc3qKsg/formResponse?usp=pp_url&entry.1243275873=SessionId&entry.67037947=" + gameId+"&entry.1708403356="+result+"&submit=Submit\", \"_blank\")).close()");
    }
    static public void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSffdgyATkT9In487OPjpQ-sXPLDtXcKy6jSdIsWbOLk1dAbyQ/formResponse?ifq&entry.1243275873=SessionId&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+currAlbumId+"&entry.416810127="+currAlbumName+"&entry.724890801="+marketingState+ "&submit=Submit\", \"_blank\")).close()");
    }
    static public void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSddPLzrQO1J_9vBGmGpjs1FOIGn3Z92fw23X-otjNQLG7cpSg/formResponse?usp=pp_url&entry.1243275873=SessionId&entry.67037947=" + currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+money+"&submit=Submit\", \"_blank\")).close()");

    }
    static public void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLScYaTNzROIoL4P6D40B_mcpM1xZuXdJBJz_neHCvCxf0qWpLA/formResponse?usp=pp_url&entry.1243275873=SessionId&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+eventType+"&entry.1712145275="+skill+"&entry.877028457="+coins+"&submit=Submit\", \"_blank\")).close()");

    }

    static public void CloseWriter()
    {
        
    }

}