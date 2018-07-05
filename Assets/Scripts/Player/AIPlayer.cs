using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIPlayerSimple : Player
{
    public AIPlayerSimple(string name) : base(name)
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

public class AIPlayerCoopStrategy : UIPlayer
{
    GameProperties.Instrument preferredInstrument;


    public AIPlayerCoopStrategy(string name) : base(name)
    {
    }


    public override void LevelUp()
    {
        //this strategy always plays for instruments, not for markting
        GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.BASS;
        int smallestValuForInstrumenteOverall = -1;
        
        List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(this.GetSkillSet().Keys);
        for (int j = 0 ; j < (playerSkillSetKeys.Count - 1) ; j++) //exclude markting, thats why the (playerSkillSetKeys.Count - 1)
        {
            GameProperties.Instrument currInstrument = playerSkillSetKeys[j];
            int biggestValueForInstrument = -1;
            for (int i=0; i < GameGlobals.players.Count; i++)
            {
                Player currPlayer = GameGlobals.players[i];
                if (this == currPlayer)
                {
                    continue;
                }
            
                int currValue = currPlayer.GetSkillSet()[playerSkillSetKeys[j]];
                if(biggestValueForInstrument == -1 || currValue > biggestValueForInstrument)
                {
                    biggestValueForInstrument = currValue;
                }
            }
            if(smallestValuForInstrumenteOverall == -1 || biggestValueForInstrument < smallestValuForInstrumenteOverall)
            {
                smallestValuForInstrumenteOverall = biggestValueForInstrument;
                smallestInstrumentOverall = currInstrument;
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

public class AIPlayerGreedyStrategy : UIPlayer
{
    GameProperties.Instrument preferredInstrument;

    public AIPlayerGreedyStrategy(string name) : base(name)
    {
    }

    public override void LevelUp()
    {
        //this strategy always plays for markting except in the first play where it plays cooperatively
        if (gameManagerRef.GetCurrGameRound() == 0)
        {
            GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.BASS;
            int smallestValuForInstrumenteOverall = -1;

            List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(this.GetSkillSet().Keys);
            for (int j = 0; j < (playerSkillSetKeys.Count - 1); j++) //exclude markting, thats why the (playerSkillSetKeys.Count - 1)
            {
                GameProperties.Instrument currInstrument = playerSkillSetKeys[j];
                int biggestValueForInstrument = -1;
                for (int i = 0; i < GameGlobals.players.Count; i++)
                {
                    Player currPlayer = GameGlobals.players[i];
                    if (this == currPlayer)
                    {
                        continue;
                    }

                    int currValue = currPlayer.GetSkillSet()[playerSkillSetKeys[j]];
                    if (biggestValueForInstrument == -1 || currValue > biggestValueForInstrument)
                    {
                        biggestValueForInstrument = currValue;
                    }
                }
                if (smallestValuForInstrumenteOverall == -1 || biggestValueForInstrument < smallestValuForInstrumenteOverall)
                {
                    smallestValuForInstrumenteOverall = biggestValueForInstrument;
                    smallestInstrumentOverall = currInstrument;
                }
            }
            preferredInstrument = smallestInstrumentOverall;
        }
        else
        {
            preferredInstrument = GameProperties.Instrument.MARKETING;
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
            SendLastDecisionsPhaseResponse(1);
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            SendLastDecisionsPhaseResponse(2);
        }
    }

}
