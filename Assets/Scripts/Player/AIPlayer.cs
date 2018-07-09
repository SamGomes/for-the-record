using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIPlayer: UIPlayer
{
    public AIPlayer(string name) : base(name)
    {
    }
}

public class AIPlayerSimple : AIPlayer
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

public class AIPlayerCoopStrategy : AIPlayer
{
    GameProperties.Instrument preferredInstrument;

    public AIPlayerCoopStrategy(string name) : base(name)
    {
    }

    public override void LevelUp()
    {
        //this strategy always plays for instruments, not for markting
        while (this.numTokens > 0)
        {
            GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.BASS;
            int smallestValueForInstrumenteOverall = -1;

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
                if (smallestValueForInstrumenteOverall == -1 || (this.skillSet[currInstrument] < GameProperties.maximumSkillLevelPerInstrument && biggestValueForInstrument < smallestValueForInstrumenteOverall))
                {
                    smallestValueForInstrumenteOverall = biggestValueForInstrument;
                    smallestInstrumentOverall = currInstrument;
                }
            }
            preferredInstrument = smallestInstrumentOverall;
            if (!GameProperties.isSimulation)
            {
                //StartCoroutine(ThinkBeforeAction(SpendToken,preferredInstrument, 2.0f));
                UpdateCommonUIElements();
            }
            else
            {
                SpendToken(preferredInstrument);
            }
        }
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

    //predicting hri-s
    public IEnumerator ThinkBeforeAction(Func<GameProperties.Instrument, bool> methodName, GameProperties.Instrument instrument, float delay)
    {
        yield return new WaitForSeconds(delay);
        methodName(instrument);
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
            //this strategy always plays for instruments, not for markting
            GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.BASS;
            int smallestValueForInstrumenteOverall = -1;

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
                if (smallestValueForInstrumenteOverall == -1 || (this.skillSet[currInstrument] < GameProperties.maximumSkillLevelPerInstrument && biggestValueForInstrument < smallestValueForInstrumenteOverall))
                {
                    smallestValueForInstrumenteOverall = biggestValueForInstrument;
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
