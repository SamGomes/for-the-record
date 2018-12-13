using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Debug log manager
public class MongoDBLogManager : ILogManager
{
    private bool isGameRunning;
    
    private string databaseName;
    private string myApiKey;

    Hashtable postHeader;
    private List<PendingCall> pendingCalls;
    private WWW phpConnection;
    
    [Serializable]
    public class DataEntryPlayerLog
    {
        public string sessionId;
        public string currGameId;
        public string playerId;
        public string playerName;
        public string type;
    }
    [Serializable]
    public class DataEntryGameLog
    {
        public string sessionId;
        public string currGameId;
        public string playerId;
        public string playerName;
        public string type;
    }
    [Serializable]
    public class DataEntryGameResultLog
    {
        public string sessionId;
        public string currGameId;
        public string condition;
        public string result;
    }
    [Serializable]
    public class DataEntryAlbumResultLog
    {
        public string sessionId;
        public string currGameId;
        public string currGameRoundId;
        public string currAlbumId;
        public string currAlbumName;
        public string marktingState;
    }
    [Serializable]
    public class DataEntryPlayerResultLog
    {
        public string sessionId;
        public string currGameId;
        public string currGameRoundId;
        public string playerId;
        public string playerName;
        public string money;
    }
    [Serializable]
    public class DataEntryEventLog
    {
        public string sessionId;
        public string currGameId;
        public string currGameRoundId;
        public string playerId;
        public string playerName;
        public string eventType;
        public string skill;
        public string coins;
    }

    [Serializable]
    public class DataEntryGameResultLogQueryResult
    {
        public List<DataEntryGameResultLog> results;
    }


    private struct PendingCall
    {
        public WWW www;
        public Func<string, int> yieldedReaction;
        public PendingCall(WWW www, Func<string, int> yieldedReaction)
        {
            this.yieldedReaction = yieldedReaction;
            this.www = www;
        }
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
                currConnection = call.www;
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
        databaseName = "fortherecordlogs";
        myApiKey = "skgyQ8WGQIP6tfmjytmcjzlgZDU2jWBD";

        postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");

        pendingCalls = new List<PendingCall>();
        isGameRunning = true;
        GameGlobals.monoBehaviourFunctionalities.StartCoroutine(ConsumePendingCalls(phpConnection));
    }

    private WWW ConvertEntityToWriteWWW(System.Object entity, string database, string collection)
    {
        string url = "https://api.mlab.com/api/1/databases/" + databaseName + "/collections/" + collection + "?apiKey=" + myApiKey;

        string entityJson = JsonUtility.ToJson(entity);
        byte[] formData = System.Text.Encoding.UTF8.GetBytes(entityJson);
        WWW www = new WWW(url, formData, postHeader);
        return www;
    }


    public void WritePlayerToLog(string sessionId, string currGameId, string playerId, string playerName, string type)
    {
        var entity = new DataEntryPlayerLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            playerId = playerId,
            playerName = playerName,
            type = type
        };
        pendingCalls.Add(new PendingCall(ConvertEntityToWriteWWW(entity, databaseName, "playerslog"), null));
    }

    public void WriteGameToLog(string sessionId, string currGameId, string condition, string result)
    {
        var entity = new DataEntryGameResultLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            condition = condition,
            result = result
        };
        pendingCalls.Add(new PendingCall(ConvertEntityToWriteWWW(entity, databaseName, "gameresultslog"), null));
    }
    public void UpdateGameResultInLog(string sessionId, string gameId, string result)
    {

        //var collection = database.GetCollection<DataEntryPlayerLog>("playerstatslog");
        //var entity = new DataEntryGameResultLog
        //{
        //    sessionId = sessionId,
        //    currGameId = currGameId,
        //    condition = condition,
        //    result = result
        //};
        //collection.Update(entity);
    }
    public void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState)
    {
        var entity = new DataEntryAlbumResultLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            currGameRoundId = currGameRoundId,
            currAlbumId = currAlbumId,
            currAlbumName = currAlbumName,
            marktingState = marktingState
        };
        pendingCalls.Add(new PendingCall(ConvertEntityToWriteWWW(entity, databaseName, "albumresultslog"), null));
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        var entity = new DataEntryPlayerResultLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            currGameRoundId = currGameRoundId,
            playerId = playerId,
            playerName = playerName,
            money = money
        };
        pendingCalls.Add(new PendingCall(ConvertEntityToWriteWWW(entity, databaseName, "playerresultslog"), null));
    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName,
        string eventType, string skill, string coins)
    {
        var entity = new DataEntryEventLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            currGameRoundId = currGameRoundId,
            playerId = playerId,
            playerName = playerName,
            eventType = eventType,
            skill = skill,
            coins = coins
        };
        pendingCalls.Add(new PendingCall(ConvertEntityToWriteWWW(entity, databaseName, "eventslog"), null));
    }

    public void GetLastSessionConditionFromLog(Func<string, int> yieldedReactionToGet)
    {
        string tableName = "gameresultslog";
        string url = "https://api.mlab.com/api/1/databases/" + databaseName + "/collections/" +
            tableName + "?apiKey=" + myApiKey + "&s={\"_id\": -1}&l=1"; //query which returns the last game result
        WWW www = new WWW(url);
        
        pendingCalls.Add(new PendingCall(www, delegate (string lastGameEntry)
        {
            string lastConditionString = "";
            
            lastGameEntry = "{ \"results\": " + lastGameEntry + "}";
            DataEntryGameResultLogQueryResult lastGameEntriesObject = JsonUtility.FromJson<DataEntryGameResultLogQueryResult>(lastGameEntry);
            if (lastGameEntriesObject.results.Count > 0)
            {
                lastConditionString = ((DataEntryGameResultLog)(lastGameEntriesObject.results[lastGameEntriesObject.results.Count-1])).condition;
            }
            return yieldedReactionToGet(lastConditionString);
        }));
    }

    public void EndLogs()
    {
        Debug.Log("Log Closed.");
    }
}

