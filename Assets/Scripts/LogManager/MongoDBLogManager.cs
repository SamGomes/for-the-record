using System;
using UnityEngine;

using MongoDB.Bson;
using MongoDB.Driver;

//Debug log manager
public class MongoDBLogManager : ILogManager
{
    private MongoClient client;
    private MongoServer server;
    private MongoDatabase database;

    public class DataEntryPlayerLog
    {
        public ObjectId id { get; set; }
        public string sessionId { get; set; }
        public string currGameId { get; set; }
        public string playerId { get; set; }
        public string playerName { get; set; }
        public string type { get; set; }
    }

    public class DataEntryGameLog
    {
        public ObjectId id { get; set; }
        public string sessionId { get; set; }
        public string currGameId { get; set; }
        public string playerId { get; set; }
        public string playerName { get; set; }
        public string type { get; set; }
    }

    public class DataEntryGameResultLog
    {
        public string sessionId { get; set; }
        public string currGameId { get; set; }
        public string condition { get; set; }
        public string result { get; set; }
    }

    public class DataEntryAlbumResultLog
    {
        public ObjectId id { get; set; }
        public string sessionId { get; set; }
        public string currGameId { get; set; }
        public string currGameRoundId { get; set; }
        public string currAlbumId { get; set; }
        public string currAlbumName { get; set; }
        public string marktingState { get; set; }
    }


    public class DataEntryPlayerResultLog
    {
        public ObjectId id { get; set; }
        public string sessionId { get; set; }
        public string currGameId { get; set; }
        public string currGameRoundId { get; set; }
        public string playerId { get; set; }
        public string playerName { get; set; }
        public string money { get; set; }
    }

    public class DataEntryEventLog
    {
        public ObjectId id { get; set; }
        public string sessionId { get; set; }
        public string currGameId { get; set; }
        public string currGameRoundId { get; set; }
        public string playerId { get; set; }
        public string playerName { get; set; }
        public string eventType { get; set; }
        public string skill { get; set; }
        public string coins { get; set; }
    }


    public void InitLogs()
    {
        client = new MongoClient("off because of security");
        server = client.GetServer();
        server.Connect();
        database = server.GetDatabase("fortherecordlogs");
    }
    public void WritePlayerToLog(string sessionId, string currGameId, string playerId, string playerName, string type)
    {
        var collection = database.GetCollection<DataEntryPlayerLog>("playerslog");
        var entity = new DataEntryPlayerLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            playerId = playerId,
            playerName = playerName,
            type = type
        };
        collection.Insert(entity);
        //var id = entity.Id;
    }
    public void WriteGameToLog(string sessionId, string currGameId, string condition, string result)
    {
        var collection = database.GetCollection<DataEntryPlayerLog>("gameresultslog");
        var entity = new DataEntryGameResultLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            condition = condition,
            result = result
        };
        collection.Insert(entity);
    }
    public void UpdateGameResultInLog(string sessionId, string gameId, string result)
    {

        var collection = database.GetCollection<DataEntryPlayerLog>("playerstatslog");
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
        var collection = database.GetCollection<DataEntryPlayerLog>("albumresultslog");
        var entity = new DataEntryAlbumResultLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            currGameRoundId = currGameRoundId,
            currAlbumId = currAlbumId,
            currAlbumName = currAlbumName,
            marktingState = marktingState
        };
        collection.Insert(entity);
    }
    public void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money)
    {
        var collection = database.GetCollection<DataEntryPlayerLog>("playerresultslog");
        var entity = new DataEntryPlayerResultLog
        {
            sessionId = sessionId,
            currGameId = currGameId,
            currGameRoundId = currGameRoundId,
            playerId = playerId,
            playerName = playerName,
            money = money
        };
        collection.Insert(entity);
    }
    public void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, 
        string eventType, string skill, string coins)
    {
        var collection = database.GetCollection<DataEntryPlayerLog>("eventslog");
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
        collection.Insert(entity);
    }


    public void GetLastSessionConditionFromLog(Func<string,int> yieldedReactionToGet)
    {
        var collection = database.GetCollection<DataEntryPlayerLog>("playerstatslog");
        //collection.();

        yieldedReactionToGet("B");

    }

    public void EndLogs()
    {
        Debug.Log("Log Closed.");
    }
}

