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

    public static int tokenValue = 1000;
}

public interface IUtilities
{
    int RollTheDice(int diceNumbers);
}

public class RandomUtilities: IUtilities{
    public int RollTheDice(int diceNumbers)
    {
        return UnityEngine.Random.Range(1, diceNumbers + 1);
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