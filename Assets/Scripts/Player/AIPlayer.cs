using RolePlayCharacter;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WellFormedNames;

public abstract class AIPlayer : UIPlayer
{
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

    public AIPlayer(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref,id, name){

        InitDelays();

        GameObject erp = new GameObject("EmotionalRoboticPlayer");
        emotionalModule = erp.AddComponent<EmotionalModule>();
        emotionalModule.Speaks = isSpeechAllowed;
        emotionalModule.ReceiveInvoker(this); //only pass the invoker after it is initialized
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
        informDiceRollDelay = 0.1f;
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


    
    #region Inform methods
    public override void InformChoosePreferredInstrument(Player nextPlayer) {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformChoosePreferredInstrumentActions(nextPlayer, informChoosePreferredInstrumentDelay, true));
        }
        else
        {
            InformChoosePreferredInstrumentActions(nextPlayer);
        }
    }
    public override void InformLevelUp(GameProperties.Instrument leveledUpInstrument) {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformLevelUpActions(leveledUpInstrument,informLevelUpDelay,true));
        }
        else
        {
            InformLevelUpActions(leveledUpInstrument);
        }
    }
    public override void InformPlayForInstrument(Player nextPlayer) {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformPlayForInstrumentActions(nextPlayer, informPlayForInstrumentDelay, true));
        }
        else
        {
            InformPlayForInstrumentActions(nextPlayer);
        }
    }
    public override void InformLastDecision(Player nextPlayer) {
        if (!GameProperties.configurableProperties.isSimulation)
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
        if (!GameProperties.configurableProperties.isSimulation)
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
        if (!GameProperties.configurableProperties.isSimulation)
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
        if (!GameProperties.configurableProperties.isSimulation)
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
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformNewAlbumActions(informNewAlbumDelay, true));
        }
        else
        {
            InformNewAlbumActions();
        }
    }

    protected virtual void InformChoosePreferredInstrumentActions(Player nextPlayer){
         emotionalModule.GazeAt(nextPlayer.GetName());
    }
    protected virtual void InformLevelUpActions(GameProperties.Instrument leveledUpInstrument)
    {
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
        emotionalModule.GazeAt(nextPlayer.GetName());
    }
    protected virtual void InformLastDecisionActions(Player nextPlayer)
    {
        emotionalModule.GazeAt(nextPlayer.GetName());
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
    #endregion

    #region Delayed methods
    private IEnumerator DelayedInformChoosePreferredInstrumentActions(Player nextPlayer, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformChoosePreferredInstrumentActions(nextPlayer);
    }
    private IEnumerator DelayedInformLevelUpActions(GameProperties.Instrument leveledUpInstrument, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformLevelUpActions(leveledUpInstrument);
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
    #endregion

    #region Response methods
    //All AI players pick one of the available instruments similarly
    //All AI players play for the available instruments similarly
    protected virtual GameProperties.Instrument ChoosePreferredInstrumentActions(Album currAlbum) {

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
                return instrument;
            }
        }
        return GameProperties.Instrument.NONE;

    }
    protected virtual GameProperties.Instrument LevelUpActions(Album currAlbum) { return GameProperties.Instrument.NONE; }
    protected virtual GameProperties.Instrument PlayforInstrumentActions(Album currAlbum) {
        if (skillSet[preferredInstrument] > 0)
        {
            return preferredInstrument;
        }
        else
        {
            return GameProperties.Instrument.NONE;
        }
    }
    protected virtual int LastDecisionsActions(Album currAlbum) { return 0; }

    public override void ChoosePreferredInstrument(Album currAlbum)
    {
        base.ChoosePreferredInstrument(currAlbum);
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeChoosingPreferredInstrument(currAlbum,choosePreferredInstrumentDelay, GameProperties.Instrument.NONE, false));
        }
        else
        {
            ChangePreferredInstrument(ChoosePreferredInstrumentActions(currAlbum));
            SendChoosePreferredInstrumentResponse();
        }
    }
    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(currAlbum, levelUpThinkingDelay, GameProperties.Instrument.NONE, false));
        }
        else
        {
            BuyTokens(1);
            SpendToken(LevelUpActions(currAlbum));
            SendLevelUpResponse();
        }
    }
    public override void PlayForInstrument(Album currAlbum)
    {
        base.PlayForInstrument(currAlbum);
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(currAlbum, playForInstrumentThinkingDelay, GameProperties.Instrument.NONE, false));
        }
        else
        {
            ChangeDiceRollInstrument(PlayforInstrumentActions(currAlbum));
            SendPlayForInstrumentResponse();
        }
    }
    public override void LastDecisionsPhase(Album currAlbum)
    {
        base.LastDecisionsPhase(currAlbum);
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(currAlbum, lastDecisionThinkingDelay, false, 0));
        }
        else
        {
            int condition = LastDecisionsActions(currAlbum);
            SendLastDecisionsPhaseResponse(condition);
        }
    }

    //predicting hri-s? calculated Xb

    //simulate button clicks
    private void SimulateMouseDown(Button button)
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
    }
    private void SimulateMouseUp(Button button)
    {
        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);
    }
    private IEnumerator SimulateMouseClick(Button button, float pressingTime)
    {
        SimulateMouseDown(button);
        yield return new WaitForSeconds(pressingTime);
        SimulateMouseUp(button);
    }

    private IEnumerator ThinkBeforeChoosingPreferredInstrument(Album currAlbum, float delay, GameProperties.Instrument preferredInstrument, bool isSendingResponse)
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

            preferredInstrument = ChoosePreferredInstrumentActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeChoosingPreferredInstrument(currAlbum, sendResponsesDelay, preferredInstrument, true));
        }
        else
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UISkillIconsButtons[(int)preferredInstrument],0.3f));
            yield return new WaitForSeconds(1.0f); //wait before pushing the other button
            playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIplayerActionButton,0.3f));
        }
    }
    private IEnumerator ThinkBeforeLevelingUp(Album currAlbum, float delay, GameProperties.Instrument chosenLevelUpInstrument, bool isSendingResponse)
    {
        yield return new WaitForSeconds(delay);
        if (!isSendingResponse)
        {
            //Fatima call
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
            emotionalModule.Decide();

            chosenLevelUpInstrument = LevelUpActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(currAlbum, sendResponsesDelay, chosenLevelUpInstrument, true));
        }
        else
        {
            if (chosenLevelUpInstrument == GameProperties.Instrument.MARKETING)
            {
                playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIspendTokenInMarketingButton, 0.3f));
            }
            else
            {
                playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIspendTokenInInstrumentButton, 0.3f));
            }
        }
    }
    private IEnumerator ThinkBeforePlayForInstrument(Album currAlbum, float delay, GameProperties.Instrument diceRollInstrument, bool isSendingResponse) 
    //diceRollInstrument not used in current version, maintained to improve flexibility
    {
        yield return new WaitForSeconds(delay);
        if (!isSendingResponse)
        {
            //Fatima call
            emotionalModule.NumDices = skillSet[preferredInstrument];
            emotionalModule.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "PlayForInstrument", name) });
            emotionalModule.Decide();

            diceRollInstrument = PlayforInstrumentActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(currAlbum, sendResponsesDelay, diceRollInstrument, true));
        }
        else
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIrollForPreferredInstrumentButton,0.3f));
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

            if (GameGlobals.albums.Count == GameProperties.configurableProperties.numberOfAlbumsPerGame)
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
            switch (receivedCondition)
            {
                case 0:
                    playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIReceiveMegaHitButton, 0.3f));
                    break;
                case 1:
                    playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIStickWithMarketingMegaHitButton, 0.3f));
                    break;
                case 2:
                    playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIReceiveFailButton, 0.3f));
                    break;

            }
        }
    }
    #endregion

    public void FlushAIUtterance(string text)
    {
        emotionalModule.FlushUtterance(text);
    }

}

public class AIPlayerSimpleStrategy : AIPlayer
{
    public AIPlayerSimpleStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.PlayerType.SIMPLE;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        return GameProperties.Instrument.GUITAR;

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

public class AIPlayerRandomStrategy : AIPlayer
{
    public AIPlayerRandomStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.PlayerType.RANDOM;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        GameProperties.Instrument instrumentToLevelUp = GameProperties.Instrument.NONE;
        instrumentToLevelUp = (Random.Range(0, 2) > 0) ? preferredInstrument : GameProperties.Instrument.MARKETING;
        return instrumentToLevelUp;
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
    public AIPlayerCoopStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.PlayerType.COOPERATIVE;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        //if there is money left spend it
        //if (skillSet[preferredInstrument] < GameProperties.maximumSkillLevelPerInstrument)
        //{
            return preferredInstrument;
        //}
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
    public AIPlayerGreedyStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.PlayerType.GREEDY;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        return GameProperties.Instrument.MARKETING;
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
    public AIPlayerBalancedStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.PlayerType.BALANCED;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        //on first round put one token on instrument
        if (GameGlobals.currGameRoundId == 0)
        {
            return preferredInstrument;
        }
        else //on the other rounds see if album was a fail and play cooperatively, otherwise play for marketing
        {
            Album lastAlbum = GameGlobals.albums[GameGlobals.albums.Count - 2];
            if (lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
            {
                //coop strategy
                return preferredInstrument;
            }
            else if(lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                //greedy strategy
                return GameProperties.Instrument.MARKETING;
            }

            return GameProperties.Instrument.NONE; //hey code, try reaching this if u can :)
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

public class AIPlayerUnbalancedStrategy : AIPlayer
{
    public AIPlayerUnbalancedStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.PlayerType.UNBALANCED;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        //on first round put one token on instrument
        if (GameGlobals.currGameRoundId == 0)
        {
            return preferredInstrument;
        }
        else //on the other rounds see if album was a fail and play cooperatively, otherwise play for marketing
        {
            Album lastAlbum = GameGlobals.albums[GameGlobals.albums.Count - 2];
            if (lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
            {
                //greedy strategy
                return GameProperties.Instrument.MARKETING;
            }
            else if (lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                //coop strategy
                return preferredInstrument;
            }

            return GameProperties.Instrument.NONE; //hey code, try reaching this if u can :)
        }
    }
    protected override int LastDecisionsActions(Album currAlbum)
    {
        int condition = 0;
        //if I have marketing skill, I use it always
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


public class AIPlayerTitForTatStrategy : AIPlayer
{
    private bool didSomeoneDefectedLastRound;
    private bool didSomeoneDefectedThisRound;

    public AIPlayerTitForTatStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed)
    {
        this.type = GameProperties.PlayerType.TITFORTAT;
        didSomeoneDefectedThisRound = false;
    }

    protected override void InformLevelUpActions(GameProperties.Instrument leveledUpInstrument)
    {
        if (didSomeoneDefectedThisRound == false)
        {
            didSomeoneDefectedThisRound = (leveledUpInstrument == GameProperties.Instrument.MARKETING);
        }
    }


    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        GameProperties.Instrument levelUpInstrument = GameProperties.Instrument.NONE;
        //if someone defected than defect
        if (didSomeoneDefectedLastRound)
        {
            levelUpInstrument = GameProperties.Instrument.MARKETING;
        }
        else //on the other rounds see if album was a fail and play cooperatively, otherwise play for marketing
        {

            levelUpInstrument = preferredInstrument;
        }
        return levelUpInstrument;
    }
    protected override int LastDecisionsActions(Album currAlbum)
    {
        //abused this method to update the perception of the AI sry :(
        didSomeoneDefectedLastRound = didSomeoneDefectedThisRound;
        didSomeoneDefectedThisRound = false;
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaa: " + didSomeoneDefectedLastRound);

        int condition = 0;
        //if I have marketing skill, I use it always
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