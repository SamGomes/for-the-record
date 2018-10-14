public interface ILogManager
{
    void InitLogs();
    void WritePlayerToLog(string sessionId, string gameId, string playerId, string playername, string type);
    void WriteGameToLog(string sessionId, string gameId, string condition, string result);
    void WriteAlbumResultsToLog(string sessionId, string currGameId, string currGameRoundId, string currAlbumId, string currAlbumName, string marktingState);
    void WritePlayerResultsToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string money);
    void WriteEventToLog(string sessionId, string currGameId, string currGameRoundId, string playerId, string playerName, string eventType, string instrument, string coins);

    string GetLastSessionConditionFromLog();
    void EndLogs();
}
