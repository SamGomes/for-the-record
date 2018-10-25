using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//MySQL log manager
public class MySQLLogManager : ILogManager
{
    private bool isGameRunning;
    private struct PendingCall
    {
        public WWWForm form;
        public Func<string, int> yieldedReaction;
        public PendingCall(WWWForm form, Func<string, int> yieldedReaction)
        {
            this.form = form;
            this.yieldedReaction = yieldedReaction;
        }
    }

    private string phpLogServerConnectionPath;
    private string databaseName;
    private WWW phpConnection;

    private List<PendingCall> pendingCalls; 

    private IEnumerator YieldedActions(WWW connection, Func<string,int> yieldedReaction)
    {
        yield return connection;
        yieldedReaction(connection.text);
    }

    private IEnumerator ConsumePendingCalls(WWW currConnection)
    {
        //Debug.Log("number of pending calls: " + pendingCalls.Count);
        List<PendingCall> currList = new List<PendingCall>(pendingCalls); //in order to not access main list while being updated
        if (!isGameRunning && currList.Count == 0)
        {
            //finish monitorizing calls
            yield return null;
        }
        else
        {
            foreach (PendingCall call in currList)
            {
                yield return currConnection;
                //if (currConnection != null)
                //{
                //    currConnection.Dispose(); //remove not needed connections
                //}
                currConnection = new WWW(phpLogServerConnectionPath, call.form);
                yield return currConnection;
                if (call.yieldedReaction != null)
                {
                    call.yieldedReaction(currConnection.text);
                }
                pendingCalls.Remove(call);
            }
            yield return currConnection;
            yield return new WaitForSeconds(0.05f);
            GameGlobals.monoBehaviourFunctionalities.StartCoroutine(ConsumePendingCalls(phpConnection));
        }
    }

    public void InitLogs()
    {
        Debug.Log("InitLogs ");

        phpLogServerConnectionPath = "http://fortherecordlogs.duckdns.org:4000/dbActions.php";
        databaseName = "fortherecordlogs";

        isGameRunning = true;

        pendingCalls = new List<PendingCall>();
        GameGlobals.monoBehaviourFunctionalities.StartCoroutine(ConsumePendingCalls(phpConnection));

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

        pendingCalls.Add(new PendingCall(form, null));
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

        pendingCalls.Add(new PendingCall(form, null));
    }
    public void UpdateGameResultInLog(string sessionId, string gameId, string result)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "UPDATE");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "result" };
        string[] values = { result };
        string extraParams = "WHERE sessionId = \""+sessionId+"\" AND gameId =\""+gameId+"\"";

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, values, extraParams);

        pendingCalls.Add(new PendingCall(form, null));
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

        pendingCalls.Add(new PendingCall(form, null));
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

        pendingCalls.Add(new PendingCall(form, null));
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

        pendingCalls.Add(new PendingCall(form, null));
    }

    public void GetLastSessionConditionFromLog(Func<string,int> yieldedReactionToGet)
    {
       
        WWWForm form = new WWWForm();
        form.AddField("dbAction", "SELECT");
        form.AddField("databaseName", databaseName);
        form.AddField("tableName", "game_stats_log");

        string[] keys = { "gameCondition" };

        PassTableArguments(form, "arrFields", "arrValues", "extraParams", keys, new string[] { "0" }, "WHERE timestampedId >= ALL( SELECT timestampedId FROM game_stats_log )");
        pendingCalls.Add(new PendingCall(form, yieldedReactionToGet));

    }

    public void EndLogs() {
        isGameRunning = false;
    }
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
