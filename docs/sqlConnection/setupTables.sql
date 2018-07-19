DROP TABLE IF EXISTS for_the_record_logs.player_stats_log;
DROP DATABASE IF EXISTS for_the_record_logs; 

CREATE DATABASE for_the_record_logs;

USE for_the_record_logs;

CREATE TABLE player_stats_log (
    playerId varchar(255),
    playerName varchar(255),
    type varchar(255)
);