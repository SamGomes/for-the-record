using IntegratedAuthoringTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameGlobals
{
    public static MonoBehaviourFunctionalities monoBehaviourFunctionalities;

    public static List<Album> albums;
    public static List<Player> players;

    public static string currSessionId;
    public static int currGameId;
    public static int currGameRoundId;

    public static int albumIdCount;

    public static GameProperties.GameState currGameState;

    public static int numberOfSpeakingPlayers;

    public static IDiceNG gameDiceNG;

    public static ILogManager gameLogManager;
    public static AudioManager audioManager;
    public static GameManager gameManager;

    //fatima stuff
    public static string FAtiMAScenarioPath;
    public static IntegratedAuthoringToolAsset FAtiMAIat;
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

    public enum MusicianRole
    {
        GUITARIST,
        DRUMMER,
        VOCALIST,
        PIANIST,
        BASSIST,
        MANAGER,
        NONE
    }
    public enum AlbumMarketingState
    {
        NON_PUBLISHED,
        FAIL,
        MEGA_HIT
    }

    public enum PlayerType
    {
        AIPLAYER_NOT_ASSIGNED,
        HUMAN,
        SIMPLE,
        RANDOM,
        COOPERATIVE,
        GREEDY,
        BALANCED,
        UNBALANCED,
        TITFORTAT
    }

    public enum NGType
    {
        RANDOM,
        VICTORY,
        LOSS
    }

    public static GameParameterization currGameParameterization; //assigned automatically when isAutomaticDebriefing or isSimulation is assigned
    public static SessionParameterization currSessionParameterization; //assigned automatically when isAutomaticDebriefing or isSimulation is assigned
    
    public static DynamicallyConfigurableGameProperties configurableProperties;

    public static bool displayFetchExternalConfigFileOption = false;
}




//configurations classes

[Serializable]
public class DynamicallyConfigurableGameProperties
{ 
    //(default configurations already assigned)

    public int tokenValue = 0; //1000;
    public int marketingPointValue = 1000; //config the initial number of dices to roll

    public int allowedPlayerActionsPerAlbum = 2;
    public int allowedPlayerTokenBuysPerRound = 1;

    public int maximumSkillLevelPerInstrument = 9000; //infinite

    public int numberOfAlbumsPerGame = 5;
    public int numberOfPlayersPerGame = 3;

    public int initNumberMarketDices = 2; //config the initial number of dices to roll


    //----------- AutomaticBriefing -------------------
    public bool isAutomaticalBriefing = false;
    public int numSessionGames = 0; //no tutorials

    //------------ Simulation --------------------
    public bool isSimulation = false;
    public int numGamesToSimulate = 1;

    public List<SessionParameterization> possibleParameterizations = new List<SessionParameterization>(); //only used when generating the AI types automatically (for example when "isSimulation=true or isAutomaticBriefing==true")
}

[Serializable]
public struct SessionParameterization
{
    public string id;
    public List<GameParameterization> gameParameterizations;

    public SessionParameterization(string id)
    {
        this.id = id;
        this.gameParameterizations = new List<GameParameterization>();
    }

    public SessionParameterization(string id, List<GameParameterization> gameParameterizations)
    {
        this.id = id;
        this.gameParameterizations = gameParameterizations;
    }

}


[Serializable]
public struct GameParameterization
{
    public List<PlayerParameterization> playerParameterizations;
    public string ngType;

    public GameParameterization(string id, List<PlayerParameterization> playerParameterizations, string ngType)
    {
        this.playerParameterizations = playerParameterizations;
        this.ngType = ngType;
    }

}

[Serializable]
public struct PlayerParameterization
{

    public string name;
    public string playerType;
    public bool isSpeechAllowed;

    //used when the the game ngType is not RANDOM
    public int[] fixedInstrumentDiceResults; 
    public int[] fixedMarketingDiceResults;

    public string likedInstrument;

    public PlayerParameterization(string name, string playerType, bool isSpeechAllowed, int[] fixedInstrumentDiceResults, int[] fixedMarketingDiceResults, string likedInstrument)
    {
        this.name = name;
        this.isSpeechAllowed = isSpeechAllowed;
        this.playerType = playerType;

        this.fixedInstrumentDiceResults = fixedInstrumentDiceResults;
        this.fixedMarketingDiceResults = fixedMarketingDiceResults;

        this.likedInstrument = likedInstrument;
    }
    public PlayerParameterization(string name, string playerType, bool isSpeechAllowed, int[] fixedInstrumentDiceResults, int[] fixedMarketingDiceResults) : this(name, playerType, isSpeechAllowed, fixedInstrumentDiceResults, fixedMarketingDiceResults, null) { }
    public PlayerParameterization(string name, string playerType, bool isSpeechAllowed) : this(name, playerType, isSpeechAllowed, new int[] { }, new int[] { }) { }
    public PlayerParameterization(string name, string playerType) : this(name, playerType, false) { }
}