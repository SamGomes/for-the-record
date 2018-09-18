using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameGlobals
{

    public static List<Album> albums;
    public static List<Player> players;

    public static string currSessionId;
    public static int currGameId;
    public static int currGameRoundId;

    public static int albumIdCount;

    public static GameProperties.GameState currGameState;

    public static string FAtiMAScenarioPath;
    public static int numberOfSpeakingPlayers;

    public static IDiceNG gameDiceNG;

    public static ILogManager gameLogManager;
    public static AudioManager audioManager;
    public static GameManager gameManager;
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

    public static int tokenValue = 0; //1000;
    public static int marketingPointValue = 1000; //config the initial number of dices to roll

    public static int allowedPlayerActionsPerAlbum = 2;
    public static int allowedPlayerTokenBuysPerRound = 1;

    public static int maximumSkillLevelPerInstrument = 9000; //infinite

    public static int numberOfAlbumsPerGame = 5;
    public static int numberOfPlayersPerGame = 3;

    public static int initNumberMarketDices = 2; //config the initial number of dices to roll


    public static bool isSpeechAllowed = false;

    //----------- AutomaticBriefing -------------------
    public static bool isAutomaticalBriefing = false;
    public static int numGamesToPlay = 2;



    //------------ Simulation --------------------
    public static bool isSimulation = false;
    public static int numGamesToSimulate = 10000;

}

