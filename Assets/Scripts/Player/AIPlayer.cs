using RolePlayCharacter;
using System.Collections;
using UnityEngine;
using WellFormedNames;

public abstract class AIPlayer : UIPlayer
{
    protected bool isSpeechAllowed;

    protected GameProperties.AIPlayerType type;

    protected float choosePreferredInstrumentDelay;
    protected float levelUpThinkingDelay;
    protected float playForInstrumentThinkingDelay;
    protected float lastDecisionThinkingDelay;

    //more specific ones can be used
    protected float informChoosePreferredInstrumentDelay;
    protected float informLevelUpDelay;
    protected float informPlayForInstrumentDelay;
    protected float informLastDecisionDelay;

    protected float informNewAlbumDelay;
    protected float informDiceRollDelay;
    protected float informAlbumResultDelay;
    protected float informGameResultDelay;


    protected float sendResponsesDelay;


    protected EmotionalModule emotionalModule;

    public AIPlayer(GameObject playerUIPrefab, GameObject canvas, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAlloweed) : base(playerUIPrefab, canvas, warningScreenref,id, name){
        this.isSpeechAllowed = isSpeechAllowed;

        InitDelays();

        GameObject erp = new GameObject("EmotionalRoboticPlayer");
        emotionalModule = erp.AddComponent<EmotionalModule>();
        emotionalModule.Speaks = false;
        emotionalModule.ReceiveInvoker(this); //only pass the invoker after it is initialized
    }

    public AIPlayer(int id, string name) : base(id, name)
    {
        InitDelays();
    }

    public void InitDelays()
    {
        choosePreferredInstrumentDelay = 2.0f;
        levelUpThinkingDelay = 2.0f;
        playForInstrumentThinkingDelay = 0.5f;
        lastDecisionThinkingDelay = 1.0f;

        informChoosePreferredInstrumentDelay = 1.0f;
        informLevelUpDelay = 2.0f;
        informPlayForInstrumentDelay = 1.0f;
        informLastDecisionDelay = 1.0f;

        informNewAlbumDelay = 1.0f;
        informDiceRollDelay = 2.0f;
        informAlbumResultDelay = 1.0f;
        informGameResultDelay = 1.0f;

        sendResponsesDelay = 2.0f;
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

    public override void InformChoosePreferredInstrument(Player nextPlayer) {
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformChoosePreferredInstrumentActions(nextPlayer, informChoosePreferredInstrumentDelay, true));
        }
        else
        {
            InformChoosePreferredInstrumentActions(nextPlayer);
        }
    }
    public override void InformLevelUp() {
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformLevelUpActions(informLevelUpDelay,true));
        }
        else
        {
            InformLevelUpActions();
        }
    }
    public override void InformPlayForInstrument(Player nextPlayer) {
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformPlayForInstrumentActions(nextPlayer, informPlayForInstrumentDelay, true));
        }
        else
        {
            InformPlayForInstrumentActions(nextPlayer);
        }
    }
    public override void InformLastDecision(Player nextPlayer) {
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformLastDecisionActions(nextPlayer, informLastDecisionDelay, true));
        }
        else
        {
            InformLastDecisionActions(nextPlayer);
        }
    }

    public override void InformRollDicesValue(Player invoker, int maxValue, int obtainedValue) {
        //base.InformRollDicesValue(invoker, maxValue, obtainedValue, speakingRobotId);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformRollDicesValueActions(invoker, maxValue, obtainedValue, informDiceRollDelay, true));
        }
        else
        {
            InformRollDicesValueActions(invoker, maxValue, obtainedValue);
        }
    }
    public override void InformAlbumResult(int albumValue, int marketValue) {
        //base.InformRollDicesValue(invoker, maxValue, obtainedValue, speakingRobotId);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformAlbumResultActions(albumValue, marketValue, informAlbumResultDelay, true));
        }
        else
        {
            InformAlbumResultActions(albumValue, marketValue);
        }
    }
    public override void InformGameResult(GameProperties.GameState state) {
        //base.InformRollDicesValue(invoker, maxValue, obtainedValue, speakingRobotId);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformGameResultActions(state, informGameResultDelay, true));
        }
        else
        {
            InformGameResultActions(state);
        }
    }
    public override void InformNewAlbum()
    {
        //base.InformRollDicesValue(invoker, maxValue, obtainedValue, speakingRobotId);
        if (!GameProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformNewAlbumActions(informNewAlbumDelay, true));
        }
        else
        {
            InformNewAlbumActions();
        }
    }


    protected virtual void InformChoosePreferredInstrumentActions(Player nextPlayer){
        if (nextPlayer.GetName() != name)
        {
            emotionalModule.GazeAt(nextPlayer.GetName());
        }
    }
    protected virtual void InformLevelUpActions(){
        Player currentPlayer = gameManagerRef.GetCurrentPlayer();
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (currSpeakingPlayerId == id && currentPlayer.GetName() == "Player")
        {
            Debug.Log(name + ": É a vez do " + currentPlayer.GetName());
            emotionalModule.Perceive(new Name[] {
                EventHelper.PropertyChange("CurrentPlayer(Name)", currentPlayer.GetName(), name),
                EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
            emotionalModule.Decide();
        }
        else if (currentPlayer != this)
        {
            emotionalModule.GazeAt(currentPlayer.GetName());
        }
    }
    protected virtual void InformPlayForInstrumentActions(Player nextPlayer)
    {
        if (nextPlayer.GetName() != name)
        {
            emotionalModule.GazeAt(nextPlayer.GetName());
        }
    }
    protected virtual void InformLastDecisionActions(Player nextPlayer)
    {
        if (nextPlayer.GetName() != name)
        {
            emotionalModule.GazeAt(nextPlayer.GetName());
        }
    }


    protected virtual void InformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue)
    {
        //Fatima calls
        emotionalModule.GazeAt("screen");
        // rolling d6 dice(s)
        if (maxValue % 20 != 0)
        {
            int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();
            if (invoker == this)
            {
                emotionalModule.DicesValue = obtainedValue;
                float luckFactor = (float)obtainedValue / (float)maxValue;

                if (luckFactor >= 0.5)
                {
                    emotionalModule.Perceive(new Name[] {
                        EventHelper.PropertyChange("State(Game)", "SelfRollInstrumentDice", name),
                        EventHelper.PropertyChange("Roll(InstrumentDice)", "Luck", invoker.GetName()) });
                }
                else
                {
                    emotionalModule.Perceive(new Name[] {
                        EventHelper.PropertyChange("State(Game)", "SelfRollInstrumentDice", name),
                        EventHelper.PropertyChange("Roll(InstrumentDice)", "BadLuck", invoker.GetName()) });
                }
                emotionalModule.Decide();
            }
            else if (currSpeakingPlayerId == id && invoker.GetName() == "Player")
            {

                float luckFactor = (float)obtainedValue / (float)maxValue;

                if (luckFactor > 0.7)
                {
                    emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "OtherRollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Luck", invoker.GetName()) });
                }
                else if (luckFactor < 0.2)
                {
                    emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "OtherRollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "BadLuck", invoker.GetName()) });
                }
                else
                {
                    emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "OtherRollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Neutral", invoker.GetName()) });
                }
                emotionalModule.Decide();
            }
            else if (invoker != this)
            {
                emotionalModule.GazeAt(invoker.GetName());
            }
        }


    }
    protected virtual void InformAlbumResultActions(int albumValue, int marketValue)
    {

        //Fatima calls
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (albumValue >= marketValue)
        {
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Success", name) });
        }
        else
        {
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Fail", name) });
        }

        if (currSpeakingPlayerId == id)
        {
            emotionalModule.Decide();
        }
        else
        {
            emotionalModule.GazeAt("Player");
        }
    }
    protected virtual void InformGameResultActions(GameProperties.GameState state)
    {
        //Fatima calls
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();
        if (state == GameProperties.GameState.VICTORY)
        {
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Victory", name) });
        }
        else if (state == GameProperties.GameState.LOSS)
        {
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Loss", name) });
        }

        if (currSpeakingPlayerId == id)
        {
            emotionalModule.Decide();
        }
        else
        {
            emotionalModule.GazeAt("Player");
        }
    }
    protected virtual void InformNewAlbumActions()
    {
        Player currentPlayer = gameManagerRef.GetCurrentPlayer();
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (currSpeakingPlayerId == id && currentPlayer.GetName() == "Player")
        {
            //Fatima call
            Debug.Log(name + ": É a vez do " + currentPlayer.GetName());
            emotionalModule.Perceive(new Name[] {
                EventHelper.PropertyChange("CurrentPlayer(Name)", currentPlayer.GetName(), name),
                EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
            emotionalModule.Decide();
        }
    }




    private IEnumerator DelayedInformChoosePreferredInstrumentActions(Player nextPlayer, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformChoosePreferredInstrumentActions(nextPlayer);
    }
    private IEnumerator DelayedInformLevelUpActions(float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformLevelUpActions();
    }
    private IEnumerator DelayedInformPlayForInstrumentActions(Player nextPlayer, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformPlayForInstrumentActions(nextPlayer);
    }
    private IEnumerator DelayedInformLastDecisionActions(Player nextPlayer, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformLastDecisionActions(nextPlayer);
    }

    private IEnumerator DelayedInformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformRollDicesValueActions(invoker, maxValue, obtainedValue);
        
    }
    private IEnumerator DelayedInformAlbumResultActions(int albumValue, int marketValue, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformAlbumResultActions(albumValue,  marketValue);

    }
    private IEnumerator DelayedInformGameResultActions(GameProperties.GameState state, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformGameResultActions(state);
    }
    private IEnumerator DelayedInformNewAlbumActions(float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformNewAlbumActions();
    }


    public void FlushRobotUtterance(string text)
    {
        emotionalModule.FlushUtterance(text);
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
            //Fatima call
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("Character(Name)", name, name),
                EventHelper.PropertyChange("Album(Last)", "False", name),
            EventHelper.PropertyChange("State(Game)", "ChoosePreferredInstrument", name) });
            emotionalModule.Decide();

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
            //Fatima call
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
            emotionalModule.Decide();

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
            //Fatima call
            emotionalModule.NumDices = skillSet[preferredInstrument];
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "PlayForInstrument", name) });
            emotionalModule.Decide();

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

            //Fatima call
            if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                emotionalModule.Perceive(new Name[] {
                EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name),
                EventHelper.PropertyChange("Album(Result)", "Success", name) });
            }
            else
            {
                emotionalModule.Perceive(new Name[] {
                EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name),
                EventHelper.PropertyChange("Album(Result)", "Fail", name) });
            }

            if (GameGlobals.albums.Count == GameProperties.numberOfAlbumsPerGame)
            {
                emotionalModule.Perceive(new Name[] {
                EventHelper.PropertyChange("Album(Last)", "True", name) });
            }
            emotionalModule.Decide();


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
    public AIPlayerSimple(GameObject playerUIPrefab, GameObject canvas, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.AIPlayerType.SIMPLE;
    }
    //for simulations
    public AIPlayerSimple(int id, string name) : base(id, name)
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
    public AIPlayerCoopStrategy(GameObject playerUIPrefab, GameObject canvas, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.AIPlayerType.COOPERATIVE;
    }
    //for simulations
    public AIPlayerCoopStrategy(int id, string name) : base(id, name)
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
    public AIPlayerGreedyStrategy(GameObject playerUIPrefab, GameObject canvas, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.AIPlayerType.GREEDY;
    }
    //for simulations
    public AIPlayerGreedyStrategy(int id, string name) : base(id, name)
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
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT && skillSet[GameProperties.Instrument.MARKETING] > 0)
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
    public AIPlayerBalancedStrategy(GameObject playerUIPrefab, GameObject canvas, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.AIPlayerType.BALANCED;
    }
    //for simulations
    public AIPlayerBalancedStrategy(int id, string name) : base(id, name)
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
        //if I have marketing skill, I use it always
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT && skillSet[GameProperties.Instrument.MARKETING]>0) 
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