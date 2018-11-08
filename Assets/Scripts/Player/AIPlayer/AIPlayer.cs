using RolePlayCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WellFormedNames;

public abstract class AIPlayer : UIPlayer
{
    protected GameProperties.Instrument likedInstrument; //Instrument I would like to get (not guaranteed to get)

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

    public AIPlayer(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, 
        PoppupScreenFunctionalities warningScreenref,
        int id, string name, bool isSpeechAllowed, GameProperties.Instrument likedInstrument) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name)
    { 
        InitDelays();
        this.likedInstrument = likedInstrument;

        if (!GameProperties.configurableProperties.isSimulation && isSpeechAllowed)
        {
            GameObject erp = new GameObject("EmotionalRoboticPlayer");
            emotionalModule = erp.AddComponent<EmotionalModule>();
            emotionalModule.Speaks = isSpeechAllowed;
            emotionalModule.ReceiveInvoker(this); //only pass the invoker after it is initialized
        }

        this.type = GameProperties.PlayerType.AIPLAYER_NOT_ASSIGNED;
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
    public override void InformLevelUp(Player invoker, GameProperties.Instrument leveledUpInstrument) {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(DelayedInformLevelUpActions(invoker, leveledUpInstrument,informLevelUpDelay,true));
        }
        else
        {
            InformLevelUpActions(invoker, leveledUpInstrument);
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
        if (emotionalModule != null)
        {
            emotionalModule.GazeAt(nextPlayer.GetName());
        }
    }
    protected virtual void InformLevelUpActions(Player invoker, GameProperties.Instrument leveledUpInstrument)
    {
        if (emotionalModule != null)
        {
            int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId(); if (currSpeakingPlayerId == id && invoker.GetName() == "Player")
            {
                Debug.Log(name + ": É a vez do " + invoker.GetName()); emotionalModule.Perceive(new Name[] {
                       EventHelper.PropertyChange("CurrentPlayer(Name)", invoker.GetName(), name),
                       EventHelper.PropertyChange("State(Game)", "LevelUp", name) }); emotionalModule.Decide();
            }
            else if (invoker != this)
            {
                emotionalModule.GazeAt(invoker.GetName());
            }
        }
    }
    protected virtual void InformPlayForInstrumentActions(Player nextPlayer)
    {
        if (emotionalModule != null)
        {
            emotionalModule.GazeAt(nextPlayer.GetName());
        }
    }
    protected virtual void InformLastDecisionActions(Player nextPlayer)
    {
        if (emotionalModule != null)
        {
            emotionalModule.GazeAt(nextPlayer.GetName());
        }
    }

    protected virtual void InformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue)
    {
        if (emotionalModule != null)
        {
            //Fatima calls
            emotionalModule.GazeAt("screen");
            // rolling d6 dice
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
    }
    protected virtual void InformAlbumResultActions(int albumValue, int marketValue)
    {
        if (emotionalModule != null)
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
    }
    protected virtual void InformGameResultActions(GameProperties.GameState state)
    {
        if (emotionalModule != null)
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
    }
    protected virtual void InformNewAlbumActions()
    {
        Player currentPlayer = gameManagerRef.GetCurrentPlayer();
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (currSpeakingPlayerId == id && currentPlayer.GetName() == "Player")
        {
            if (emotionalModule != null)
            {
                //Fatima call
                Debug.Log(name + ": É a vez do " + currentPlayer.GetName());
                emotionalModule.Perceive(new Name[] {
                EventHelper.PropertyChange("CurrentPlayer(Name)", currentPlayer.GetName(), name),
                EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
                emotionalModule.Decide();
            }
        }
    }
    #endregion

    #region Delayed methods
    private IEnumerator DelayedInformChoosePreferredInstrumentActions(Player nextPlayer, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformChoosePreferredInstrumentActions(nextPlayer);
    }
    private IEnumerator DelayedInformLevelUpActions(Player invoker, GameProperties.Instrument leveledUpInstrument, float delay, bool isInformDelayed)
    {
        yield return new WaitForSeconds(delay);
        InformLevelUpActions(invoker, leveledUpInstrument);
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
        List<GameProperties.Instrument> skillSetKeys = new List<GameProperties.Instrument>(skillSet.Keys);
        for (int i=(int)likedInstrument; i<skillSetKeys.Count; i= ((i+1)%skillSetKeys.Count))
        {
            GameProperties.Instrument currInstrument = skillSetKeys[i];
            bool instrumentIsAvailable = true;
            //check if other players have the same preferred instrument
            foreach (Player player in GameGlobals.players)
            {
                if (player == this)
                {
                    continue;
                }
                if (player.GetPreferredInstrument() == currInstrument)
                {
                    instrumentIsAvailable = false;
                    break;
                }
            }
            if (instrumentIsAvailable)
            {
                return currInstrument;
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
    protected virtual int LastDecisionsActions(Album currAlbum) {
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

            preferredInstrument = ChoosePreferredInstrumentActions(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeChoosingPreferredInstrument(currAlbum, sendResponsesDelay, preferredInstrument, true));
        }
        else
        {
            playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UISkillIconsButtons[(int)preferredInstrument],0.3f));
            yield return new WaitForSeconds(1.0f); //wait before pushing the other button
            playerMonoBehaviourFunctionalities.StartCoroutine(SimulateMouseClick(UIplayerActionButton,0.3f));
            if (emotionalModule != null)
            {
                //Fatima call
                emotionalModule.Perceive(new Name[] {
                EventHelper.PropertyChange("Character(Name)", name, name),
                EventHelper.PropertyChange("Album(Last)", "False", name),
                EventHelper.PropertyChange("State(Game)", "ChoosePreferredInstrument", name) });
                emotionalModule.Decide();
            }
        }
    }
    private IEnumerator ThinkBeforeLevelingUp(Album currAlbum, float delay, GameProperties.Instrument chosenLevelUpInstrument, bool isSendingResponse)
    {
        yield return new WaitForSeconds(delay);
        if (!isSendingResponse)
        {

            chosenLevelUpInstrument = LevelUpActions(currAlbum);
            if (emotionalModule != null)
            {
                //Fatima call
                if (chosenLevelUpInstrument == GameProperties.Instrument.MARKETING)
                {
                    emotionalModule.Perceive(new Name[] {
                        EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
                        EventHelper.PropertyChange("Action(Game)", "Defect", name),
                        EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
                    Debug.Log("DEFECT");
                }
                else
                {
                    emotionalModule.Perceive(new Name[] {
                        EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
                        EventHelper.PropertyChange("Action(Game)", "Cooperate", name),
                        EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
                    Debug.Log("COOPERATE");
                }
                emotionalModule.Decide();
            }


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

            if (emotionalModule != null)
            {
                //Fatima call
                emotionalModule.NumDices = skillSet[preferredInstrument];
                emotionalModule.Perceive(new Name[] {
                    EventHelper.PropertyChange("State(Game)", "PlayForInstrument", name)
                });
                emotionalModule.Decide();
            }
            

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
            if (emotionalModule != null)
            {
                //Fatima call
                if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
                {
                    emotionalModule.NumDices = skillSet[GameProperties.Instrument.MARKETING];
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
            }

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
        if (emotionalModule != null)
        {
            emotionalModule.FlushUtterance(text);
        }
    }

}