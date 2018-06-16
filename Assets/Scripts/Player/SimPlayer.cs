using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIPlayerSimple : Player
{
    public SimPlayerSimple(string name) : base(name)
    {
    }


    public override void LevelUp()
    {
        SpendToken(GameProperties.Instrument.GUITAR);
        SpendToken(GameProperties.Instrument.GUITAR);
        SendLevelUpResponse();
    }

    public override void PlayForInstrument()
    {
        ChangeDiceRollInstrument(GameProperties.Instrument.GUITAR);
        SendPlayForInstrumentResponse();
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            SendLastDecisionsPhaseResponse(0);
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            SendLastDecisionsPhaseResponse(2);
        }
    }

}

public class AIPlayerCoopStrategy : Player
{
    GameProperties.Instrument preferredInstrument;


    public SimPlayerCoopStrategy(string name) : base(name)
    {
    }


    public override void LevelUp()
    {
        //this strategy always plays for instruments
        GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.BASS;
        int smallestValue = -1;
        for (int i=0; i < GameGlobals.players.Count; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            Dictionary<GameProperties.Instrument, int> playerSkillSet = currPlayer.GetSkillSet();
            List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(playerSkillSet.Keys);
            for (int j = 0 ; j < playerSkillSetKeys.Count ; j++)
            {
                GameProperties.Instrument currInstrument = playerSkillSetKeys[j];
                int currValue = playerSkillSet[playerSkillSetKeys[j]];
                if(smallestValue==-1 || currValue < smallestValue)
                {
                    smallestInstrumentOverall = currInstrument;
                    smallestValue = currValue;
                }
            }
        }
        preferredInstrument = smallestInstrumentOverall;
        SpendToken(preferredInstrument);
        SendLevelUpResponse();
    }

    public override void PlayForInstrument()
    {
        ChangeDiceRollInstrument(preferredInstrument);
        SendPlayForInstrumentResponse();
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            SendLastDecisionsPhaseResponse(0);
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            SendLastDecisionsPhaseResponse(2);
        }
    }

}

public class AIPlayerGreedyStrategy : Player
{
    GameProperties.Instrument preferredInstrument;


    public AIPlayerGreedyStrategy(string name) : base(name)
    {
    }

    public override void LevelUp()
    {
        //this strategy always plays for markting except in the first play
        if (gameManagerRef.GetCurrGameRound() == 0)
        {
            GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.BASS;
            int smallestValue = -1;
            for (int i = 0; i < GameGlobals.players.Count; i++)
            {
                Player currPlayer = GameGlobals.players[i];
                Dictionary<GameProperties.Instrument, int> playerSkillSet = currPlayer.GetSkillSet();
                List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(playerSkillSet.Keys);
                for (int j = 0; j < playerSkillSetKeys.Count; j++)
                {
                    GameProperties.Instrument currInstrument = playerSkillSetKeys[j];
                    int currValue = playerSkillSet[playerSkillSetKeys[j]];
                    if (smallestValue == -1 || currValue < smallestValue)
                    {
                        smallestInstrumentOverall = currInstrument;
                        smallestValue = currValue;
                    }
                }
            }
            preferredInstrument = smallestInstrumentOverall;
        }
        else
        {
            preferredInstrument = GameProperties.Instrument.MARKTING;
        }
        SpendToken(preferredInstrument);
        SendLevelUpResponse();
    }

    public override void PlayForInstrument()
    {
        ChangeDiceRollInstrument(preferredInstrument);
        SendPlayForInstrumentResponse();
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            SendLastDecisionsPhaseResponse(0);
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            SendLastDecisionsPhaseResponse(2);
        }
    }

}
