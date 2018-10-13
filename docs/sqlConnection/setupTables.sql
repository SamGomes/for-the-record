DROP TABLE IF EXISTS for_the_record_logs.players_log;
DROP TABLE IF EXISTS for_the_record_logs.game_stats_log;
DROP TABLE IF EXISTS for_the_record_logs.album_stats_log;
DROP TABLE IF EXISTS for_the_record_logs.player_stats_log;
DROP TABLE IF EXISTS for_the_record_logs.player_actions_log;
DROP DATABASE IF EXISTS for_the_record_logs; 

CREATE DATABASE for_the_record_logs;

USE for_the_record_logs;

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