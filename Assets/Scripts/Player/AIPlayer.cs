using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : UIPlayer
{
    protected GameProperties.AIPlayerType type;

    protected float levelUpThinkingDelay;
    protected float playForInstrumentThinkingDelay;
    protected float lastDecisionThinkingDelay;

    public AIPlayer(string name) : base(name) {
        levelUpThinkingDelay = 4.0f;
        playForInstrumentThinkingDelay = 8.0f;
        lastDecisionThinkingDelay = 2.0f;
    }

    protected void RegisterMeOnPlayersLog()
    {
        GameProperties.gameLogManager.WritePlayerToLog(this.id.ToString(), this.name, this.type.ToString());
    }

    //predicting hri-s
    public IEnumerator ThinkBeforeLevelingUp(float delay)
    {
        yield return new WaitForSeconds(delay);
        //yield return null;
        SendLevelUpResponse();
    }
    public IEnumerator ThinkBeforePlayForInstrument(float delay)
    {
        yield return new WaitForSeconds(delay);
        //yield return null;
        SendPlayForInstrumentResponse();
    }
    public IEnumerator ThinkBeforeLastDecisionPhase(int condition, float delay)
    {
        yield return new WaitForSeconds(delay);
        //yield return null;
        SendLastDecisionsPhaseResponse(condition);
    }
}

public class AIPlayerSimple : AIPlayer
{
    public AIPlayerSimple(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.SIMPLE;
        RegisterMeOnPlayersLog();
    }

    public override void LevelUp(Album currAlbum)
    {
        SpendToken(GameProperties.Instrument.GUITAR);
        SpendToken(GameProperties.Instrument.GUITAR);

        if (!GameProperties.isSimulation)
        {
            base.LevelUp(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(levelUpThinkingDelay));
        }
        else
        {
            SendLevelUpResponse();
        }

    }

    public override void PlayForInstrument(Album currAlbum)
    {
        ChangeDiceRollInstrument(GameProperties.Instrument.GUITAR);

        if (!GameProperties.isSimulation)
        {
            base.LevelUp(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(playForInstrumentThinkingDelay));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 0;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }

        if (!GameProperties.isSimulation)
        {
            base.LastDecisionsPhase(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(condition, lastDecisionThinkingDelay));
        }
        else
        {
            SendLastDecisionsPhaseResponse(condition);
        }
    }

}

public class AIPlayerCoopStrategy : AIPlayer
{
    GameProperties.Instrument preferredInstrument;

    public AIPlayerCoopStrategy(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.COOPERATIVE;
        RegisterMeOnPlayersLog();
    }

    public override void LevelUp(Album currAlbum)
    {
        //if there is money left spend it
        if (money > 0)
        {
            BuyTokens(1);
        }

        //this strategy always plays for instruments, not for markting
        for (int numTokensSpent = 0; numTokensSpent < (this.numTokens + 1); numTokensSpent++)
        {
            GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.NONE;
            int smallestValueForInstrumenteOverall = -1;

            List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(this.GetSkillSet().Keys);
            for (int j = 0; j < (playerSkillSetKeys.Count - 1); j++) //exclude markting, thats why the (playerSkillSetKeys.Count - 1)
            {
                GameProperties.Instrument currInstrument = playerSkillSetKeys[j];

                //cannot evolve fully evolved skill
                if (skillSet[currInstrument] >= GameProperties.maximumSkillLevelPerInstrument)
                {
                    continue;
                }

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
                if (smallestValueForInstrumenteOverall == -1 || biggestValueForInstrument < smallestValueForInstrumenteOverall)
                {
                    smallestValueForInstrumenteOverall = biggestValueForInstrument;
                    smallestInstrumentOverall = currInstrument;
                }
            }
            //if my preferred skill is less evolved than other of my skills, I change my mind! XD
            if (skillSet[preferredInstrument] < skillSet[smallestInstrumentOverall])
            {
                preferredInstrument = smallestInstrumentOverall;
            }
            SpendToken(smallestInstrumentOverall);
        }

        if (!GameProperties.isSimulation)
        {
            base.LevelUp(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(levelUpThinkingDelay));
        }
        else
        {
            SendLevelUpResponse();
        }
    }

    public override void PlayForInstrument(Album currAlbum)
    {
        ChangeDiceRollInstrument(preferredInstrument);
        if (!GameProperties.isSimulation)
        {
            base.PlayForInstrument(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(playForInstrumentThinkingDelay));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 0;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }

        if (!GameProperties.isSimulation)
        {
            base.LastDecisionsPhase(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(condition, lastDecisionThinkingDelay));
        }
        else
        {
            SendLastDecisionsPhaseResponse(condition);
        }
    }

}

//this strategy always plays for markting except in the first play where it is cooperative. In the last decision it always trusts his markting.
public class AIPlayerGreedyStrategy : AIPlayer
{
    GameProperties.Instrument preferredInstrument;

    public AIPlayerGreedyStrategy(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.GREEDY;
        RegisterMeOnPlayersLog();
    }

    public override void LevelUp(Album currAlbum)
    {

        if (gameManagerRef.GetCurrGameRound() == 0)
        {
            //this strategy always plays for instruments, not for markting
            for (int numTokensSpent = 0; numTokensSpent < (this.numTokens - 1); numTokensSpent++)
            {
                GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.NONE;
                int smallestValueForInstrumenteOverall = -1;

                List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(this.GetSkillSet().Keys);
                for (int j = 0; j < (playerSkillSetKeys.Count - 1); j++) //exclude markting, thats why the (playerSkillSetKeys.Count - 1)
                {
                    GameProperties.Instrument currInstrument = playerSkillSetKeys[j];

                    //cannot evolve fully evolved skill
                    if (skillSet[currInstrument] >= GameProperties.maximumSkillLevelPerInstrument)
                    {
                        continue;
                    }

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
                    if (smallestValueForInstrumenteOverall == -1 || biggestValueForInstrument < smallestValueForInstrumenteOverall)
                    {
                        smallestValueForInstrumenteOverall = biggestValueForInstrument;
                        smallestInstrumentOverall = currInstrument;
                    }
                }
                preferredInstrument = smallestInstrumentOverall;
                SpendToken(smallestInstrumentOverall);
            }
            SpendToken(GameProperties.Instrument.MARKETING);
        }
        else
        {
            //if there is money left spend it
            if (money > 0 && skillSet[GameProperties.Instrument.MARKETING] < GameProperties.maximumSkillLevelPerInstrument)
            {
                BuyTokens(1);
            }

            //this strategy always plays for instruments, not for markting
            for (int numTokensSpent = 0; numTokensSpent < this.numTokens; numTokensSpent++)
            {
                SpendToken(GameProperties.Instrument.MARKETING);
            }
            
        }
        if (!GameProperties.isSimulation)
        {
            base.LevelUp(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(levelUpThinkingDelay));
        }
        else
        {
            SendLevelUpResponse();
        }
    }

    public override void PlayForInstrument(Album currAlbum)
    {
        ChangeDiceRollInstrument(preferredInstrument);
        if (!GameProperties.isSimulation)
        {
            base.PlayForInstrument(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(playForInstrumentThinkingDelay));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 1;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }

        if (!GameProperties.isSimulation)
        {
            base.LastDecisionsPhase(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(condition, lastDecisionThinkingDelay));
        }
        else
        {
            SendLastDecisionsPhaseResponse(condition);
        }
    }

}

public class AIPlayerBalancedStrategy : AIPlayer
{
    GameProperties.Instrument preferredInstrument;

    public AIPlayerBalancedStrategy(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.BALANCED;
        RegisterMeOnPlayersLog();
    }

    private void LevelUpForInstrument(Album currAlbum)
    {
        
        GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.NONE;
        int smallestValueForInstrumenteOverall = -1;

        List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(this.GetSkillSet().Keys);
        for (int j = 0; j < (playerSkillSetKeys.Count - 1); j++) //exclude markting, thats why the (playerSkillSetKeys.Count - 1)
        {
            GameProperties.Instrument currInstrument = playerSkillSetKeys[j];

            //cannot evolve fully evolved skill
            if (skillSet[currInstrument] >= GameProperties.maximumSkillLevelPerInstrument)
            {
                continue;
            }

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
            if (smallestValueForInstrumenteOverall == -1 || biggestValueForInstrument < smallestValueForInstrumenteOverall)
            {
                smallestValueForInstrumenteOverall = biggestValueForInstrument;
                smallestInstrumentOverall = currInstrument;
            }
        }
        preferredInstrument = smallestInstrumentOverall;
        SpendToken(smallestInstrumentOverall);
    }

    public override void LevelUp(Album currAlbum)
    {
        //on first round put one token on instrument and one token on marketing
        if (gameManagerRef.GetCurrGameRound() == 0)
        {
            //this strategy always plays for instruments, not for markting
            for (int numTokensSpent = 0; numTokensSpent < (this.numTokens - 1); numTokensSpent++)
            {
                LevelUpForInstrument(currAlbum);
            }
            SpendToken(GameProperties.Instrument.MARKETING);
        }
        else //on the other rounds see if album was a fail and play cooperatively, otherwise play for marketing
        {
            Album lastAlbum = GameGlobals.albums[GameGlobals.albums.Count - 2];
            if (lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
            {
                //if there is money left spend it
                if (money > 0)
                {
                    BuyTokens(1);
                }

                for (int numTokensSpent = 0; numTokensSpent < this.numTokens; numTokensSpent++)
                {
                    LevelUpForInstrument(currAlbum);
                }
            }
            else if(lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                //if there is money left spend it
                if (money > 0 && skillSet[GameProperties.Instrument.MARKETING] < GameProperties.maximumSkillLevelPerInstrument)
                {
                    BuyTokens(1);
                }
                
                for (int numTokensSpent = 0; numTokensSpent < this.numTokens; numTokensSpent++)
                {
                    SpendToken(GameProperties.Instrument.MARKETING);
                }
            }
        }
        if (!GameProperties.isSimulation)
        {
            base.LevelUp(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(levelUpThinkingDelay));
        }
        else
        {
            SendLevelUpResponse();
        }
    }

    public override void PlayForInstrument(Album currAlbum)
    {
        ChangeDiceRollInstrument(preferredInstrument);
        if (!GameProperties.isSimulation)
        {
            base.PlayForInstrument(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(playForInstrumentThinkingDelay));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 1;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }

        if (!GameProperties.isSimulation)
        {
            base.LastDecisionsPhase(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(condition, lastDecisionThinkingDelay));
        }
        else
        {
            SendLastDecisionsPhaseResponse(condition);
        }
    }
}