using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameGlobals
{
    public static List<Album> albums;
    public static List<Player> players;

    public static double currSessionId = System.DateTime.Now.ToOADate();
    public static int currGameId = 0;
    public static int currGameRoundId = 0;

    public static int albumIdCount = 0;
    public static int playerIdCount = 0;

    public static GameProperties.GameState currGameState;

    public static string FAtiMAScenarioPath = "/Scenarios/ForTheRecord.iat";

    public static IDiceNG gameDiceNG = new VictoryDiceNG();

    public static ILogManager gameLogManager = new FileLogManager();
    public static AudioManager audioManager = new AudioManager();
    
}

public static class GameProperties
{
    public enum GameState
    {
        NOT_FINISHED,
        VICTORY,
        LOSS
    }

    public enum Instrument
    {
        GUITAR,
        DRUMS,
        VOCALS,
        KEYBOARD,
        BASS,
        MARKETING,
        NONE
    }
    public enum AlbumMarketingState
    {
        NON_PUBLISHED,
        FAIL,
        MEGA_HIT
    }

    public enum AIPlayerType
    {
        SIMPLE,
        COOPERATIVE,
        GREEDY,
        BALANCED
    }

    public static int tokenValue = 1000;
    public static int allowedPlayerActionsPerAlbum = 2;
    public static int allowedPlayerTokenBuysPerRound = 1;

    public static int maximumSkillLevelPerInstrument = 3;

    public static int numberOfAlbumsPerGame = 5;
    public static int numberOfPlayersPerGame = 3;

    //------------ Simulation --------------------
    public static bool isSimulation = false;
    public static int numGamesToSimulate = 20;
}

