using System.Collections;
using System.Collections.Generic;


public static class GameProperties
{
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