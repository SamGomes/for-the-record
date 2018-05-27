using System.Collections;
using System.Collections.Generic;

public static class Utilities{
    public enum Instrument
    {
        GUITAR,
        DRUMS,
        VOCALS,
        KEYBOARD,
        BASS
    }
    public enum AlbumMarketingState
    {
        FAIL,
        MEGA_HIT
    }

    //methods to roll the dice
    public static int RandomRollTheDice(int diceNumbers, int callNumber)
    {
        return UnityEngine.Random.Range(1, diceNumbers + 1);
    }
    public static int FixedRollTheDice(int diceNumbers, int callNumber)
    {
        return callNumber;
    }
}
