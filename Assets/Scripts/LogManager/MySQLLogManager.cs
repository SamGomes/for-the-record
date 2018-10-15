using System;
using System.Collections;
using UnityEngine;

//MySQL log manager
public class MySQLLogManager : ILogManager
{
    private string phpLogServerConnectionPath;
    private string databaseName;
    public WWW phpConnection;

    private IEnumerator YieldedActions(WWW connection, Func<int> yieldedReaction)
    {
        yield return connection;
        phpConnection = connection;
        yieldedReaction();
    }

    public void InitLogs()
    {
        phpLogServerConnectionPath = "http://fortherecordlogs.duckdns.org:4000/dbActions.php";
        databaseName = "ist176415";
    }
    public void WritePlayerToLog(string sessionId, string gameId, string playerId, string playerName, string type)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "players_log");

        string[] keys = { "sessionId", "gameId", "playerId", "playerName", "type" };
        string[] values = { sessionId, gameId, playerId, playerName, type };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        phpConnection = new WWW(phpLogServerConnectionPath, form);

        //while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WriteGameToLog(string sessionId, string gameId, string gameCondition, string result)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "sessionId", "gameId", "gameCondition", "result" };
        string[] values = { sessionId, gameId, gameCondition, result };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        phpConnection = new WWW(phpLogServerConnectionPath, form);

        //while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marketingState)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "album_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "albumId", "albumName", "marketingState" };
        string[] values = { sessionId, currGameId, currGameRoundId, currAlbumId, currAlbumName, marketingState };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        phpConnection = new WWW(phpLogServerConnectionPath, form);

        //while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "player_stats_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "money" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, money };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        phpConnection = new WWW(phpLogServerConnectionPath, form);

        //while (!phpConnection.isDone) { } //wait until php is done
    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string skill, string coins)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "INSERT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "player_actions_log");

        string[] keys = { "sessionId", "gameId", "roundId", "playerId", "playerName", "eventType", "skill", "coins" };
        string[] values = { sessionId, currGameId, currGameRoundId, playerId, playerName, eventType, skill, coins };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, "");
        phpConnection = new WWW(phpLogServerConnectionPath, form);

        //while (!phpConnection.isDone) { } //wait until php is done
    }

    public string GetLastSessionConditionFromLog(Func<int> yieldedReactionToGet)
    {
       
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "SELECT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "gameCondition" };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, new string[] { "0" }, "WHERE timestampedId >= ALL( SELECT timestampedId FROM game_stats_log )");
        phpConnection = new WWW(phpLogServerConnectionPath, form);

        GameGlobals.monoBehaviourFunctionalities.StartCoroutine(YieldedActions(phpConnection, yieldedReactionToGet));
        

        return "<error>";
        //return phpConnection.text;
    }

    public void EndLogs() { }
    private void PassTableArguments(WWWForm form, string keysTableName, string valuesTableName, string extraParamsName, string[] keys, string[] values, string extraParams)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            form.AddField(keysTableName + "[]", keys[i]);
        }
        for (int i = 0; i < values.Length; i++)
        {
            form.AddField(valuesTableName + "[]", values[i]);
        }
        form.AddField(extraParamsName, extraParams);
    }
}
