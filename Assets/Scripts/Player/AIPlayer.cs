using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AIPlayer : UIPlayer
{
    protected GameProperties.AIPlayerType type;

    protected float choosePreferredInstrumentDelay;
    protected float levelUpThinkingDelay;
    protected float playForInstrumentThinkingDelay;
    protected float lastDecisionThinkingDelay;

    protected float sendResponsesDelay;

    public AIPlayer(string name) : base(name) {
        choosePreferredInstrumentDelay = 2.0f;
        levelUpThinkingDelay = 2.0f;
        playForInstrumentThinkingDelay = 2.0f;
        lastDecisionThinkingDelay = 2.0f;

        sendResponsesDelay = 0.5f;
    }

    public override void RegisterMeOnPlayersLog()
    {
        GameGlobals.gameLogManager.WritePlayerToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), this.id.ToString(), this.name, this.type.ToString());
    }

    public override void InitUI(GameObject playerUIPrefab, GameObject canvas, PoppupScreenFunctionalities warningScreenRef)
    {
        base.InitUI(playerUIPrefab, canvas, warningScreenRef);
        this.DisableAllInputs();
    }

    //All AI players pick one of the available instruments similarly
    public virtual void ChoosePreferredInstrumentActions(Album currAlbum) {

        foreach (GameProperties.Instrument instrument in skillSet.Keys)
        {
            bool instrumentIsAvailable = true;
            //check if other players have the same preferred instrument
            foreach (Player player in GameGlobals.players)
            {
                if (player == this)
                {
                    continue;
                }
                if (player.GetPreferredInstrument() == instrument)
                {
                    instrumentIsAvailable = false;
                    break;
                }
            }
            if (instrumentIsAvailable)
            {
                ChangePreferredInstrument(instrument);
                break;
            }
        }

    }

    public virtual void LevelUpActions(Album currAlbum) { }

    //All AI players play for the available instruments similarly
    public virtual void PlayforInstrumentActions(Album currAlbum) {
        if (skillSet[preferredInstrument] > 0)
        {
            ChangeDiceRollInstrument(preferredInstrument);
        }
        else
        {
            ChangeDiceRollInstrument(GameProperties.Instrument.NONE);
        }
    }

    public virtual int LastDecisionsActions(Album currAlbum) { return 0; }


    public override void ChoosePreferredInstrument(Album currAlbum)
    {
        base.ChoosePreferredInstrument(currAlbum);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeChoosingPreferredInstrument(currAlbum,choosePreferredInstrumentDelay, false));
        }
        else
        {
            ChoosePreferredInstrumentActions(currAlbum);
            SendChoosePreferredInstrumentResponse();
        }
    }
    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(currAlbum, levelUpThinkingDelay, false));
        }
        else
        {
            LevelUpActions(currAlbum);
            SendLevelUpResponse();
        }
    }
    public override void PlayForInstrument(Album currAlbum)
    {
        base.PlayForInstrument(currAlbum);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(currAlbum, playForInstrumentThinkingDelay, false));
        }
        else
        {
            PlayforInstrumentActions(currAlbum);
            SendPlayForInstrumentResponse();
        }
    }
    public override void LastDecisionsPhase(Album currAlbum)
    {
        base.LastDecisionsPhase(currAlbum);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(currAlbum, lastDecisionThinkingDelay, false, 0));
        }
        else
        {
            int condition = LastDecisionsActions(currAlbum);
            SendLastDecisionsPhaseResponse(condition);
        }
    }

    //predicting hri-s
    public IEnumerator ThinkBeforeChoosingPreferredInstrument(Album currAlbum, float delay, bool isSendingResponse)
    {
        yield return new WaitForSeconds(delay);
        if (!isSendingResponse)
        {
            ChoosePreferredInstrumentActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeChoosingPreferredInstrument(currAlbum, sendResponsesDelay, true));
        }
        else
        {
            SendChoosePreferredInstrumentResponse();
        }
    }
    public IEnumerator ThinkBeforeLevelingUp(Album currAlbum, float delay, bool isSendingResponse)
    {
        yield return new WaitForSeconds(delay);
        if (!isSendingResponse)
        {
            LevelUpActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(currAlbum, sendResponsesDelay, true));
        }
        else
        {
            SendLevelUpResponse();
        }
    }
    public IEnumerator ThinkBeforePlayForInstrument(Album currAlbum, float delay, bool isSendingResponse)
    {
        yield return new WaitForSeconds(delay);
        if (!isSendingResponse)
        {
            PlayforInstrumentActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(currAlbum, sendResponsesDelay, true));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }
    public IEnumerator ThinkBeforeLastDecisionPhase(Album currAlbum, float delay, bool isSendingResponse, int receivedCondition)
    {
        yield return new WaitForSeconds(delay);
        if (!isSendingResponse)
        {
            int condition = LastDecisionsActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(currAlbum, sendResponsesDelay, true, condition));
        }
        else
        {
            SendLastDecisionsPhaseResponse(receivedCondition);
        }
    }
}

public class AIPlayerSimple : AIPlayer
{
    public AIPlayerSimple(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.SIMPLE;
    }

    public override void LevelUpActions(Album currAlbum)
    {
        SpendToken(GameProperties.Instrument.GUITAR);

    }
    public override int LastDecisionsActions(Album currAlbum)
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
        return condition;
    }
}

public class AIPlayerCoopStrategy : AIPlayer
{
    public AIPlayerCoopStrategy(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.COOPERATIVE;
    }

    public override void LevelUpActions(Album currAlbum)
    {
        //if there is money left spend it
        if (skillSet[preferredInstrument] < GameProperties.maximumSkillLevelPerInstrument)
        {
            if (money > 0)
            {
                BuyTokens(1);
            }
            SpendToken(preferredInstrument);
        }
    }
    public override int LastDecisionsActions(Album currAlbum)
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
        return condition;
    }
}

//this strategy always plays for markting except in the first play where it is cooperative. In the last decision it always trusts his markting.
public class AIPlayerGreedyStrategy : AIPlayer
{
    public AIPlayerGreedyStrategy(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.GREEDY;
    }

    public override void LevelUpActions(Album currAlbum)
    {
        if (GameGlobals.currGameRoundId == 0)
        {
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
    }
    public override int LastDecisionsActions(Album currAlbum)
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
        return condition;
    }
}

public class AIPlayerBalancedStrategy : AIPlayer
{
    public AIPlayerBalancedStrategy(string name) : base(name)
    {
        this.type = GameProperties.AIPlayerType.BALANCED;
    }
   
    public override void LevelUpActions(Album currAlbum)
    {
        //on first round put one token on instrument and one token on marketing
        if (GameGlobals.currGameRoundId == 0)
        {
            SpendToken(preferredInstrument);
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
                    SpendToken(preferredInstrument);
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
    }
    public override int LastDecisionsActions(Album currAlbum)
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
        return condition;
    }
}