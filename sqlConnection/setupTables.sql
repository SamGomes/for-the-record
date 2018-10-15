DROP TABLE IF EXISTS ist176415.players_log;
DROP TABLE IF EXISTS ist176415.game_stats_log;
DROP TABLE IF EXISTS ist176415.album_stats_log;
DROP TABLE IF EXISTS ist176415.player_stats_log;
DROP TABLE IF EXISTS ist176415.player_actions_log;

USE ist176415;

CREATE TABLE players_log (
    sessionId varchar(255),
    gameId varchar(255),
    playerId varchar(255),
    playerName varchar(255),
    type varchar(255)
);

CREATE TABLE game_stats_log (
    timestampedId INT AUTO_INCREMENT PRIMARY KEY,
    sessionId varchar(255),
    gameId varchar(255),
    gameCondition varchar(255),
    result varchar(255)
);

CREATE TABLE album_stats_log (
    sessionId varchar(255),
    gameId varchar(255),
    roundId varchar(255),
    albumId varchar(255),
    albumName varchar(255),
    marketingState varchar(255)
);

CREATE TABLE player_stats_log (
    sessionId varchar(255),
    gameId varchar(255),
    roundId varchar(255),
    playerId varchar(255),
    playerName varchar(255),
    money varchar(255)
);

CREATE TABLE player_actions_log (
    sessionId varchar(255),
    gameId varchar(255),
    roundId varchar(255),
    playerId varchar(255),
    playerName varchar(255),
    eventType varchar(255),
    skill varchar(255),
    coins varchar(255)
);