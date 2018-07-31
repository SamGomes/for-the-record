using System.Collections;
using UnityEngine;

public abstract class AIPlayer : UIPlayer
{
    protected GameProperties.AIPlayerType type;

    protected float choosePreferredInstrumentDelay;
    protected float levelUpThinkingDelay;
    protected float playForInstrumentThinkingDelay;
    protected float lastDecisionThinkingDelay;

    //more specific ones can be used
    protected float informDiceRollDelay;
    protected float informAlbumResultDelay;
    protected float informGameResultDelay;


    protected float sendResponsesDelay;

    public AIPlayer(string name) : base(name) {
        choosePreferredInstrumentDelay = 2.0f;
        levelUpThinkingDelay = 2.0f;
        playForInstrumentThinkingDelay = 0.5f;
        lastDecisionThinkingDelay = 1.0f;


        informDiceRollDelay = 3.0f;
        informAlbumResultDelay = 3.0f;
        informGameResultDelay = 3.0f;

        sendResponsesDelay = 1.0f;
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



    //----------------------------------[INFORM METHODS]-----------------------------------

    public override void InformRollDicesValue(Player invoker, int maxValue, int obtainedValue, int speakingRobotId) {
        //base.InformRollDicesValue(invoker, maxValue, obtainedValue, speakingRobotId);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformRollDicesValueActions(invoker, maxValue, obtainedValue, speakingRobotId, informDiceRollDelay, true));
        }
        else
        {
            InformRollDicesValueActions(invoker, maxValue, obtainedValue, speakingRobotId);
        }
    }
    public override void InformAlbumResult(int albumValue, int marketValue, int speakingRobotId) {
        //base.InformRollDicesValue(invoker, maxValue, obtainedValue, speakingRobotId);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformAlbumResultActions(albumValue, marketValue, speakingRobotId, informAlbumResultDelay, true));
        }
        else
        {
            InformAlbumResultActions(albumValue, marketValue, speakingRobotId);
        }
    }
    public override void InformGameResult(GameProperties.GameState state, int speakingRobotId) {
        //base.InformRollDicesValue(invoker, maxValue, obtainedValue, speakingRobotId);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedGameResultActions(state, speakingRobotId, informGameResultDelay, true));
        }
        else
        {
            InformGameResultActions(state, speakingRobotId);
        }
    }

    protected virtual void InformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue, int speakingRobotId)
    {

    }
    protected virtual void InformAlbumResultActions(int albumValue, int marketValue, int speakingRobotId)
    {

    }
    protected virtual void InformGameResultActions(GameProperties.GameState state, int speakingRobotId)
    {

    }

    private IEnumerator DelayedInformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue, int speakingRobotId, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformRollDicesValueActions(invoker, maxValue, obtainedValue, speakingRobotId);
        
    }
    private IEnumerator DelayedInformAlbumResultActions(int albumValue, int marketValue, int speakingRobotId, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformAlbumResultActions(albumValue,  marketValue,  speakingRobotId);

    }
    private IEnumerator DelayedGameResultActions(GameProperties.GameState state, int speakingRobotId, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformGameResultActions(state, speakingRobotId);
    }


    //------------------------------------[RESPONSE METHODS]---------------------------------------------------

    //All AI players pick one of the available instruments similarly
    //All AI players play for the available instruments similarly
    protected virtual void ChoosePreferredInstrumentActions(Album currAlbum) {

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
    protected virtual void LevelUpActions(Album currAlbum) { }
    protected virtual void PlayforInstrumentActions(Album currAlbum) {
        if (skillSet[preferredInstrument] > 0)
        {
            ChangeDiceRollInstrument(preferredInstrument);
        }
        else
        {
            ChangeDiceRollInstrument(GameProperties.Instrument.NONE);
        }
    }
    protected virtual int LastDecisionsActions(Album currAlbum) { return 0; }

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
    private IEnumerator ThinkBeforeChoosingPreferredInstrument(Album currAlbum, float delay, bool isSendingResponse)
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
    private IEnumerator ThinkBeforeLevelingUp(Album currAlbum, float delay, bool isSendingResponse)
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
    private IEnumerator ThinkBeforePlayForInstrument(Album currAlbum, float delay, bool isSendingResponse)
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
    private IEnumerator ThinkBeforeLastDecisionPhase(Album currAlbum, float delay, bool isSendingResponse, int receivedCondition)
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

    protected override void LevelUpActions(Album currAlbum)
    {
        SpendToken(GameProperties.Instrument.GUITAR);

    }
    protected override int LastDecisionsActions(Album currAlbum)
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

    protected override void LevelUpActions(Album currAlbum)
    {
        //if there is money left spend it
        if (skillSet[preferredInstrument] < GameProperties.maximumSkillLevelPerInstrument)
        {
            if (money >= 0)
            {
                BuyTokens(1);
            }
            SpendToken(preferredInstrument);
        }
    }
    protected override int LastDecisionsActions(Album currAlbum)
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

    protected override void LevelUpActions(Album currAlbum)
    {
        if (money >= 0)
        {
            BuyTokens(1);
        }
        SpendToken(GameProperties.Instrument.MARKETING);
    }
    protected override int LastDecisionsActions(Album currAlbum)
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

    protected override void LevelUpActions(Album currAlbum)
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
                //coop strategy
                if (skillSet[preferredInstrument] < GameProperties.maximumSkillLevelPerInstrument)
                {
                    if (money >= 0)
                    {
                        BuyTokens(1);
                    }
                    SpendToken(preferredInstrument);
                }
            }
            else if(lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                //greedy strategy
                if (money >= 0)
                {
                    BuyTokens(1);
                }
                SpendToken(GameProperties.Instrument.MARKETING);
            }
        }
    }
    protected override int LastDecisionsActions(Album currAlbum)
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