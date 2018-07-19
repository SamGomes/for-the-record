using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//file log manager
public abstract class LogManager
{
    abstract public void InitLogs();
    abstract public void WritePlayerToLog(string playerId, string playername, string type);
    abstract public void WriteGameToLog(string gameId, string result);
    abstract public void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState);
    abstract public void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money);
    abstract public void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins);
    abstract public void CloseLogs();
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

        albumStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"AlbumId\";\"AlbumName\";\"MState\"");
        playersLogFileWritter.WriteLine("\"PlayerId\";\"PlayerName\";\"AIType\"");
        playerStatsFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Money\"");
        gameStatsFileWritter.WriteLine("\"GameId\";\"Result\"");
        playerActionsLogFileWritter.WriteLine("\"GameId\";\"RoundId\";\"PlayerId\";\"PlayerName\";\"Event Type\";\"Instrument\";\"Value\"");
    }
    public override void WritePlayerToLog(string playerId, string playername, string type)
    {
        //prevent access after disposal
        if (playersLogFileWritter != null)
        {
            playersLogFileWritter.WriteLine(playerId + ";" + playername + ";" + type);
        }
    }
    public override void WriteGameToLog(string gameId, string result)
    {
        //prevent access after disposal
        if (gameStatsFileWritter != null)
        {
            gameStatsFileWritter.WriteLine(gameId + ";" + result);
        }
    }
    public override void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState)
    {
        //prevent access after disposal
        if (albumStatsFileWritter != null)
        {
            albumStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + currAlbumId + ";" + currAlbumName + ";" + marktingState);
        }
    }
    public override void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        //prevent access after disposal
        if (playerStatsFileWritter != null)
        {
            playerStatsFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + money);
        }
    }
    public override void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins)
    {
        //prevent access after disposal
        if (playerActionsLogFileWritter != null)
        {
            playerActionsLogFileWritter.WriteLine(currGameId + ";" + currGameRoundId + ";" + playerId + ";" + playerName + ";" + eventType + ";" + instrument + ";" + coins);
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

    public override void WritePlayerToLog(string playerId, string playerName, string type)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSfyUPtS4_dN6iJaQgKSfKhqNZH0tCyfMto_jyvApmumYkFuBg/viewform?usp=pp_url&entry.1243275873=SessionId&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+type+"&submit=Submit\", \"_blank\")).close()");
    }
    public override void WriteGameToLog(string gameId, string result)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSeL5rFO12-RniFZU3HtKHhJMg-jh6pqMqjc5rWAbgSFc3qKsg/formResponse?usp=pp_url&entry.1243275873=SessionId&entry.67037947=" + gameId+"&entry.1708403356="+result+"&submit=Submit\", \"_blank\")).close()");
    }
    public override void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSffdgyATkT9In487OPjpQ-sXPLDtXcKy6jSdIsWbOLk1dAbyQ/formResponse?ifq&entry.1243275873=SessionId&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+currAlbumId+"&entry.416810127="+currAlbumName+"&entry.724890801="+marketingState+ "&submit=Submit\", \"_blank\")).close()");
    }
    public override void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLSddPLzrQO1J_9vBGmGpjs1FOIGn3Z92fw23X-otjNQLG7cpSg/formResponse?usp=pp_url&entry.1243275873=SessionId&entry.67037947=" + currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+money+"&submit=Submit\", \"_blank\")).close()");

    }
    public override void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        Application.ExternalEval("(window.open(\"https://docs.google.com/forms/d/e/1FAIpQLScYaTNzROIoL4P6D40B_mcpM1xZuXdJBJz_neHCvCxf0qWpLA/formResponse?usp=pp_url&entry.1243275873=SessionId&entry.67037947="+currGameId+"&entry.1708403356="+currGameRoundId+"&entry.1264259345="+playerId+"&entry.416810127="+playerName+"&entry.724890801="+eventType+"&entry.1712145275="+skill+"&entry.877028457="+coins+"&submit=Submit\", \"_blank\")).close()");

    }

    public override void CloseLogs()
    {
    }
}

//mySQL log manager
//public class MySQLLogManager: LogManager
//{
//    private WWW albumStats;
//    private WWW playersLog;
//    private WWW playerStats;
//    private WWW gameStats;
//    private WWW playerActionsLog;

//    public void InitLogs()
//    {
//        albumStats = new WWW("http://localhost:3000/sqlconnect/albumStats.php");
//        playersLog;
//        playerStats;
//        gameStats;
//        playerActionsLog;
//    }
//    public void WritePlayerToLog(string playerId, string playerName, string type)
//    {
//        WWWForm form = new WWWForm();
//        form.AddField("playerId", playerId);
//        form.AddField("playerName", playerName);
//        form.AddField("type", playerId);
//    }
//    public void WriteGameToLog(string gameId, string result);
//    public void WriteAlbumResultsToLog(string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState);
//    public void WritePlayerResultsToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string money);
//    public void WritePlayerActionToLog(string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins);
//    public void CloseLogs();
//}