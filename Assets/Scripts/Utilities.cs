using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameGlobals
{
    public static List<Album> albums;
    public static List<Player> players;
    

    public static int currGameId = 0;

    public static int albumIdCount = 0;
    public static int playerIdCount = 0;

    public static GameProperties.GameState currGameState;
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
        MARKTING
    }
    public enum AlbumMarketingState
    {
        NON_PUBLISHED,
        FAIL,
        MEGA_HIT
    }

    public enum PlayerAction
    {
        SPEND_TOKEN,
        CHANGE_PREFERRED_INSTRUMENT
    }

    public static int tokenValue = 1000;
    public static int allowedPlayerActionsPerAlbum = 2;
    public static int allowedPlayerTokenBuysPerRound = 1;

    public static int numberOfAlbumsPerGame = 1;
    public static int numberOfPlayersPerGame = 3;


    //------------ Simulation --------------------
    public static bool isSimulation = false;
    public static int numGamesToSimulate = 10000;
}

public interface IUtilities
{
    int RollTheDice(int diceNumbers);
}

public class RandomUtilities: IUtilities{
    private System.Random random = new System.Random();

    public int RollTheDice(int diceNumbers)
    {
        return random.Next(diceNumbers-1) + 1;
    }
}

public class FixedUtilities : IUtilities
{
    int numberOfDiceRolls = 0;
    public int RollTheDice(int diceNumbers)
    {
        numberOfDiceRolls++;
        return numberOfDiceRolls;
    }
}