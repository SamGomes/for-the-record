using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    private int numMegaHits;

    public GameObject canvas;

    private int numPlayersToLevelUp;
    private int numPlayersToPlayForInstrument;
    private int numPlayersToStartLastDecisions;
    private int numPlayersToChooseDiceRollInstrument;

    private bool canSelectToCheckAlbumResult;
    private bool canCheckAlbumResult;
    private bool checkedAlbumResult;

    //------------ UI -----------------------------
    public GameObject playerUIPrefab;
    public GameObject albumUIPrefab;

    public GameObject UInewRoundScreen;
    public Button UIadvanceRoundButton;
    public Text UIalbumNameText;
    
    public GameObject UIRollDiceForInstrumentOverlay;
    public GameObject UIRollDiceForMarketValueScreen;

    public GameObject PopUpChangeDecisionPrefab;

    public GameObject dice6UI;
    public GameObject dice20UI;

    public GameObject diceArrowPrefab;


    public GameObject UIAlbumCollectionDisplay;
    public GameObject UIPrototypeArea;

    public GameObject poppupPrefab;
    public PoppupScreenFunctionalities warningPoppupRef;
    public PoppupScreenFunctionalities infoPoppupNeutralRef;
    public PoppupScreenFunctionalities infoPoppupLossRef;
    public PoppupScreenFunctionalities infoPoppupWinRef;

    public PoppupScreenFunctionalities endPoppupWinRef;
    public PoppupScreenFunctionalities endPoppupLossRef;


    private bool gameMainSceneFinished;
    private int interruptionRequests; //changed whenever an interruption occurs (either a poppup, warning, etc.)
    private bool preferredInstrumentsChoosen;

    private bool choosePreferedInstrumentResponseReceived;
    private bool playForInstrumentResponseReceived;
    private bool levelUpResponseReceived;
    private bool lastDecisionResponseReceived;
    private bool beSureResponseReceived;

    private int currPlayerIndex;
    private int currSpeakingPlayerId;

    private Album currAlbum;

    private float diceRollDelay;

    private int marketLimit;
    private int currNumberOfMarketDices;

    void Awake()
    {
        GameGlobals.gameManager = this;
        //mock to test
        //GameGlobals.gameLogManager.InitLogs();
        //GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        //GameGlobals.players = new List<Player>(GameProperties.numberOfPlayersPerGame);
        //GameGlobals.players.Add(new UIPlayer("Coop Jeff"));
        //GameGlobals.players.Add(new UIPlayer("Greedy Kevin"));
        //GameGlobals.players.Add(new UIPlayer("Balanced Sam"));
        //GameGlobals.gameDiceNG = new VictoryDiceNG();
    }

    public void InterruptGame()
    {
        interruptionRequests++;
    }
    public void ContinueGame()
    {
        interruptionRequests--;
    }

    public void InitGame()
    {
        choosePreferedInstrumentResponseReceived = false;
        playForInstrumentResponseReceived = false;
        levelUpResponseReceived = false;
        lastDecisionResponseReceived = false;
        beSureResponseReceived = false;
        currPlayerIndex = 0;

        interruptionRequests = 0;

        warningPoppupRef = new PoppupScreenFunctionalities(poppupPrefab,canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(),Resources.Load<Sprite>("Textures/UI/Icons/Warning"), new Color(0.9f, 0.8f, 0.8f), "Audio/snap");

        infoPoppupLossRef = new PoppupScreenFunctionalities(poppupPrefab,canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(),Resources.Load<Sprite>("Textures/UI/Icons/InfoLoss"), new Color(0.9f, 0.8f, 0.8f), "Audio/albumLoss");
        infoPoppupWinRef = new PoppupScreenFunctionalities(poppupPrefab,canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(),Resources.Load<Sprite>("Textures/UI/Icons/InfoWin"), new Color(0.9f, 0.9f, 0.8f), "Audio/albumVictory");
        infoPoppupNeutralRef = new PoppupScreenFunctionalities(poppupPrefab,canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(),Resources.Load<Sprite>("Textures/UI/Icons/Info"), new Color(0.9f, 0.9f, 0.9f), "Audio/snap");

        //these poppups load the end scene
        endPoppupLossRef = new PoppupScreenFunctionalities(poppupPrefab, canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(), Resources.Load<Sprite>("Textures/UI/Icons/InfoLoss"), new Color(0.9f, 0.8f, 0.8f), delegate() { /*end game*/ if (this.gameMainSceneFinished) GameSceneManager.LoadEndScene(); return 0; });
        endPoppupWinRef = new PoppupScreenFunctionalities(poppupPrefab, canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(), Resources.Load<Sprite>("Textures/UI/Icons/InfoWin"), new Color(0.9f, 0.9f, 0.8f), delegate () { /*end game*/ if (this.gameMainSceneFinished) GameSceneManager.LoadEndScene(); return 0; });

        gameMainSceneFinished = false;
        preferredInstrumentsChoosen = false;

        //diceRollDelay = UIRollDiceForInstrumentOverlay.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        diceRollDelay = 6.0f;

        canCheckAlbumResult = false;
        checkedAlbumResult = false;
        canSelectToCheckAlbumResult = true;
        int numPlayers = GameGlobals.players.Count;

        Player currPlayer = null;
        for (int i = 0; i < numPlayers; i++)
        {
            currPlayer = GameGlobals.players[i];
            currPlayer.InitPlayer(playerUIPrefab, PopUpChangeDecisionPrefab, canvas, warningPoppupRef);
            if ((currPlayer as UIPlayer) != null) //check if player has UI
            {

                ((UIPlayer)currPlayer).GetPlayerUI().transform.Translate(new Vector3(0, -i*170.0f, 0));
                //position UI correctly depending on players number (table layout)
                //float refAngle = (180.0f / (numPlayers - 1));
                //((UIPlayer)currPlayer).GetPlayerUI().transform.RotateAround(new Vector3(510, 490, 0), new Vector3(0, 0, 1), (i * refAngle));
                //((UIPlayer)currPlayer).GetPlayerUI().transform.Rotate(new Vector3(0, 0, 1), -(i*refAngle) + 90.0f);
            }
            currPlayer.ReceiveTokens(1);
        }

        ChangeActivePlayerUI(((UIPlayer)(GameGlobals.players[0])), 2.0f);

        GameGlobals.currGameRoundId = 0; //first round
        numMegaHits = 0;

        marketLimit = Mathf.FloorToInt(GameProperties.numberOfAlbumsPerGame * 4.0f / 5.0f) - 1;
        currNumberOfMarketDices = GameProperties.initNumberMarketDices;
    }

    //warning: works only when using human players!
    private IEnumerator ChangeActivePlayerUI(UIPlayer player, float delay)
    {
        player.GetPlayerUI().transform.SetAsLastSibling();
        player.GetPlayerSureUI().transform.SetAsLastSibling();
        //yield return new WaitForSeconds(delay);
        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            if (GameGlobals.players[i] == player)
            {
                player.GetPlayerMarkerUI().SetActive(true);
                player.GetPlayerDisablerUI().SetActive(true);
                continue;
            }
            UIPlayer currPlayer = (UIPlayer)GameGlobals.players[i];
            currPlayer.GetPlayerMarkerUI().SetActive(false);
            currPlayer.GetPlayerDisablerUI().SetActive(false);
        }
        return null;
    }

    void Start()
    {
        InitGame();

        UIadvanceRoundButton.onClick.AddListener(delegate () {
                UInewRoundScreen.SetActive(false);
                StartGameRoundForAllPlayers(UIalbumNameText.text);
        });

        numPlayersToChooseDiceRollInstrument = GameGlobals.players.Count;
        numPlayersToLevelUp = GameGlobals.players.Count;
        numPlayersToPlayForInstrument = GameGlobals.players.Count;
        numPlayersToStartLastDecisions = GameGlobals.players.Count;

        GameGlobals.currGameState = GameProperties.GameState.NOT_FINISHED;

        if (GameProperties.isSimulation) //start imidiately in simulation
        {
            StartGameRoundForAllPlayers("SimAlbum");
        }
        else
        {
            UIRollDiceForInstrumentOverlay.SetActive(false);
            UIRollDiceForMarketValueScreen.SetActive(false);

            Button rollDiceForMarketButton = UIRollDiceForMarketValueScreen.transform.Find("rollDiceForMarketButton").GetComponent<Button>();
            rollDiceForMarketButton.onClick.AddListener(delegate () {
                canCheckAlbumResult = true;
            });

        }

        //players talk about the initial album
        currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);
        foreach (var player in GameGlobals.players)
        {
            player.InformNewAlbum();
        }


    }

    public void StartGameRoundForAllPlayers(string albumName)
    {
        Album newAlbum = new Album(albumName, albumUIPrefab);
        newAlbum.GetAlbumUI().SetActive(true);
        GameGlobals.albums.Add(newAlbum);
        this.currAlbum = newAlbum;

        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.InitAlbumContributions();
        }

        if (!preferredInstrumentsChoosen)
        {
            StartChoosePreferredInstrumentPhase();
        }
        else
        {
            StartLevelingUpPhase();
        }
    }



    public int RollDicesForInstrument(Player currPlayer, GameProperties.Instrument instrument)
    {
        var skillSet = currPlayer.GetSkillSet();

        int newAlbumInstrumentValue = 0;
        int numTokensForInstrument = skillSet[instrument];

        //UI stuff
        UIRollDiceForInstrumentOverlay.transform.Find("title/Text").GetComponent<Text>().text = currPlayer.GetName() + " rolling "+ numTokensForInstrument + " dice(s) for " + instrument.ToString() + " ...";

        int[] rolledDiceNumbers = new int[numTokensForInstrument]; //save each rolled dice number to display in the UI

        for (int i = 0; i < numTokensForInstrument; i++)
        {
            int randomIncrease = GameGlobals.gameDiceNG.RollTheDice(currPlayer, instrument, 6, i, numTokensForInstrument);
            rolledDiceNumbers[i] = randomIncrease;
            newAlbumInstrumentValue += randomIncrease;
        }
        if (!GameProperties.isSimulation)
        {
            string arrowText = "";
            if(instrument == GameProperties.Instrument.MARKETING)
            {
                arrowText = "+" + newAlbumInstrumentValue * GameProperties.marketingPointValue + " $";
            }
            else
            {
                arrowText = "+ " + newAlbumInstrumentValue + " Album Value";
            }

            StartCoroutine(PlayDiceUIs(currPlayer, newAlbumInstrumentValue, rolledDiceNumbers, 6, dice6UI, "Animations/RollDiceForInstrumentOverlay/dice6/sprites_3/endingAlternatives/", Color.yellow, arrowText, diceRollDelay));
        }

        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), currPlayer.GetId().ToString(), currPlayer.GetName().ToString(), "ROLLED_INSTRUMENT_DICES", "-", newAlbumInstrumentValue.ToString());
        return newAlbumInstrumentValue;
    }


    private IEnumerator PlayDiceUIs(Player diceThrower, int totalDicesValue, int[] rolledDiceNumbers, int diceNum, GameObject diceImagePrefab, string diceNumberSpritesPath, Color diceArrowColor, string diceArrowText, float delayToClose)
    //the sequence number aims to void dice overlaps as it represents the order for which this dice is going to be rolled. We do not want to roll a dice two times for the same place
    {
        InterruptGame();
        int numDiceRolls = rolledDiceNumbers.Length;
        for (int i = 0; i < numDiceRolls; i++)
        {
            int currDiceNumber = rolledDiceNumbers[i];
            Sprite currDiceNumberSprite = Resources.Load<Sprite>(diceNumberSpritesPath + currDiceNumber);
            if (currDiceNumberSprite == null)
            {
                Debug.Log("cannot find sprite for dice number " + currDiceNumber);
            }
            else
            {
                Debug.Log(rolledDiceNumbers[i]);
                StartCoroutine(PlayDiceUI(diceThrower, i, diceNum, diceImagePrefab, currDiceNumberSprite, delayToClose));
            }
        }

        //players see the dice result
        currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);
        foreach (var player in GameGlobals.players)
        {
            player.InformRollDicesValue(diceThrower, numDiceRolls * diceNum, totalDicesValue); //max value = the max dice number * number of rolls
        }

        //get and disable arrow animation until end of dice animation
        GameObject diceArrowClone = Instantiate(diceArrowPrefab, UIRollDiceForInstrumentOverlay.transform);
        diceArrowClone.GetComponentInChildren<Image>().color = diceArrowColor;
        
        Animator arrowAnimator = diceArrowClone.GetComponentInChildren<Animator>();
        //arrowAnimator.speed = 0;
        Text arrowText = diceArrowClone.GetComponentInChildren<Text>();
        arrowText.text = diceArrowText;
        arrowText.color = diceArrowColor;


        yield return new WaitForSeconds(delayToClose);

        Destroy(diceArrowClone);

        ContinueGame();
        UIRollDiceForInstrumentOverlay.SetActive(false);
    }

    private IEnumerator PlayDiceUI(Player diceThrower, int sequenceNumber, int diceNum, GameObject diceImagePrefab, Sprite currDiceNumberSprite, float delayToClose)
    //the sequence number aims to void dice overlaps as it represents the order for which this dice is going to be rolled. We do not want to roll a dice two times for the same place
    {
        UIRollDiceForInstrumentOverlay.SetActive(true);
        GameObject diceImageClone = Instantiate(diceImagePrefab, UIRollDiceForInstrumentOverlay.transform);

        Image diceImage = diceImageClone.GetComponent<Image>();
        Animator diceAnimator = diceImage.GetComponent<Animator>();

        sequenceNumber = (sequenceNumber % 2==0)? sequenceNumber : -sequenceNumber;

        diceImage.transform.Translate(new Vector3(Random.Range(-80.0f, 80.0f), Random.Range(-80.0f, 80.0f), 0));

        float diceRotation = sequenceNumber * (360.0f / diceNum);

        diceImage.transform.Rotate(new Vector3(0, 0, 1), diceRotation);
        diceImage.overrideSprite = null;
        diceAnimator.Rebind();
        diceAnimator.Play(0);
        diceAnimator.speed = Random.Range(0.8f,1.4f);

        while (!diceAnimator.GetCurrentAnimatorStateInfo(0).IsName("endState"))
        {
            yield return null;
        }
        diceImage.overrideSprite = currDiceNumberSprite;
        
        //arrowAnimator.speed = 1;

        yield return new WaitForSeconds(delayToClose + (1.0f - diceAnimator.speed)*delayToClose);
        Destroy(diceImageClone);
    }

    //assuming the first player rolls the market dices
    public int RollDicesForMarketValue()
    {
        UIRollDiceForInstrumentOverlay.transform.Find("title/Text").GetComponent<Text>().text = "Rolling dices for market...";

        int marketValue = 0;
        int[] rolledDiceNumbers = new int[currNumberOfMarketDices];
        for(int i=0; i < currNumberOfMarketDices; i++)
        {
            int randomIncrease = GameGlobals.gameDiceNG.RollTheDice(this.GetCurrentPlayer(), GameProperties.Instrument.NONE, 20, i, currNumberOfMarketDices);
            rolledDiceNumbers[i] = randomIncrease;
            marketValue += randomIncrease;
        }
        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), "-", "-", "ROLLED_MARKET_DICES", "-", marketValue.ToString());

        if (!GameProperties.isSimulation)
        {
            StartCoroutine(PlayDiceUIs(GameGlobals.players[0], marketValue, rolledDiceNumbers, 20, dice20UI, "Animations/RollDiceForInstrumentOverlay/dice20/sprites/endingAlternatives/", Color.red, "Market Value: " + marketValue, diceRollDelay));
        }

        return marketValue;
    }
    
    public void CheckAlbumResult()
    {
        int numAlbums = GameGlobals.albums.Count;
        int numPlayers = GameGlobals.players.Count;
        
        int newAlbumValue = currAlbum.CalcAlbumValue();
        int marketValue = RollDicesForMarketValue();
        if (newAlbumValue >= marketValue)
        {
            currAlbum.SetMarketingState(GameProperties.AlbumMarketingState.MEGA_HIT);
            numMegaHits++;
        }
        else
        {
            currAlbum.SetMarketingState(GameProperties.AlbumMarketingState.FAIL);
        }

        if (!GameProperties.isSimulation)
        {
            if (newAlbumValue >= marketValue)
            {
                infoPoppupWinRef.DisplayPoppupWithDelay("As your album value (" + newAlbumValue + ") was EQUAL or HIGHER than the market value (" + marketValue + "), the album was successfully published! Congratulations! Everyone can choose to receive 3000 $ or to receive based on their own marketing skill.", diceRollDelay*0.8f); //the delay is reduced to account for dices animation
            }
            else
            {
                infoPoppupLossRef.DisplayPoppupWithDelay("As your album value (" + newAlbumValue + ") was LOWER than the market value (" + marketValue + "), the album could not be published. Everyone receives 0 $.", diceRollDelay * 0.8f);
            }
        }


        //players see the album result
        currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);
        foreach (var player in GameGlobals.players)
        {
            player.InformAlbumResult(newAlbumValue, marketValue);
        }


        //check for game loss (collapse) or victory on album registry
        float victoryThreshold = Mathf.Ceil(GameProperties.numberOfAlbumsPerGame / 2.0f);
        float numAlbumsLeft = (float)(GameProperties.numberOfAlbumsPerGame - numAlbums);
        if (numAlbumsLeft < victoryThreshold - numMegaHits)
        {
            GameGlobals.currGameState = GameProperties.GameState.LOSS;
        }
        else
        {
            if(numAlbumsLeft == 0)
            {
                GameGlobals.currGameState = GameProperties.GameState.VICTORY;
            }
        }

        this.checkedAlbumResult = true;
    }

    private void ResetAllPlayers()
    {
        foreach (Player player in GameGlobals.players)
        {
            player.ResetPlayer();
        }
    }

    // wait for all players to exit one phase and start other phase
    void Update () {
        
        //avoid rerun in this case because load scene is asyncronous
        if (this.gameMainSceneFinished || this.interruptionRequests>0)
        {
            //Debug.Log("pause...");
            return;
        }
        

        //middle of the phases
        if (choosePreferedInstrumentResponseReceived)
        {
            currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);

            choosePreferedInstrumentResponseReceived = false;
            Player currPlayer = GameGlobals.players[currPlayerIndex];
            Player nextPlayer = ChangeToNextPlayer(currPlayer);
            numPlayersToChooseDiceRollInstrument--;
            if (numPlayersToChooseDiceRollInstrument > 0)
            {
                foreach (var player in GameGlobals.players)
                {
                    player.InformChoosePreferredInstrument(nextPlayer);
                }
                

                nextPlayer.ChoosePreferredInstrumentRequest(currAlbum);
            }
        }
        if (levelUpResponseReceived) 
        {
            currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);

            levelUpResponseReceived = false;
            Player currPlayer = GameGlobals.players[currPlayerIndex];
            Player nextPlayer = ChangeToNextPlayer(currPlayer);
            numPlayersToLevelUp--;
            if (numPlayersToLevelUp > 0)
            {
                foreach (var player in GameGlobals.players)
                {
                    player.InformLevelUp();
                }

                nextPlayer.LevelUpRequest(currAlbum);
                
            }
        }

        if (beSureResponseReceived)
        {
            beSureResponseReceived = false;
            //TODO: Remove debug
            Debug.Log("SUUUUUUUUUUUUUUURE");

            levelUpResponseReceived = true;
        }

        if (playForInstrumentResponseReceived)
        {
            currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);

            playForInstrumentResponseReceived = false;
            Player currPlayer = GameGlobals.players[currPlayerIndex];
            Player nextPlayer = ChangeToNextPlayer(currPlayer);
            numPlayersToPlayForInstrument--;
            if (numPlayersToPlayForInstrument > 0)
            {
                foreach (var player in GameGlobals.players)
                {
                    player.InformPlayForInstrument(nextPlayer);
                }

                nextPlayer.PlayForInstrumentRequest(currAlbum);
            }
        }
        if (lastDecisionResponseReceived)
        {
            currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);

            lastDecisionResponseReceived = false;
            Player currPlayer = GameGlobals.players[currPlayerIndex];
            Player nextPlayer = ChangeToNextPlayer(currPlayer);
            numPlayersToStartLastDecisions--;
            if (numPlayersToStartLastDecisions > 0)
            {
                foreach (var player in GameGlobals.players)
                {
                    player.InformLastDecision(nextPlayer);
                }

                nextPlayer.LastDecisionsPhaseRequest(currAlbum);
            }
        }

        //end of first phase; trigger second phase
        if (!preferredInstrumentsChoosen && numPlayersToChooseDiceRollInstrument == 0)
        {

            Debug.Log("running1...");
            StartPlayForInstrumentPhase(); //choose instrument phase skips level up phase
            //numPlayersToChooseDiceRollInstrument = GameGlobals.players.Count; //is not performed to ensure this phase is only played once
            preferredInstrumentsChoosen = true;
        }

        //end of second phase; trigger third phase
        if (numPlayersToLevelUp == 0)
        {
            Debug.Log("running2...");
            StartPlayForInstrumentPhase();
            numPlayersToLevelUp = GameGlobals.players.Count;
        }
        
        //end of third phase;
        if (numPlayersToPlayForInstrument == 0)
        {
            if (checkedAlbumResult)
            {
                Debug.Log("running3...");
                checkedAlbumResult = false;
                StartLastDecisionsPhase();
                numPlayersToPlayForInstrument = GameGlobals.players.Count;
            }
            else if(canSelectToCheckAlbumResult)
            {
                //make phase UI active (this step is interim but must be done before last phase)
                UIRollDiceForMarketValueScreen.SetActive(true);
                canSelectToCheckAlbumResult = false;
            }
            
            if (canCheckAlbumResult || GameProperties.isSimulation)
            {
                CheckAlbumResult();
                canCheckAlbumResult = false;
                canSelectToCheckAlbumResult = true;
                UIRollDiceForMarketValueScreen.SetActive(false);
            }
            
        }

        //end of forth phase; trigger and log album result
        if (numPlayersToStartLastDecisions == 0)
        {
            Debug.Log("running4...");
            int numPlayedAlbums = GameGlobals.albums.Count;

            //write curr game logs
            GameGlobals.gameLogManager.WriteAlbumResultsToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), currAlbum.GetId().ToString(), currAlbum.GetName(), currAlbum.GetMarketingState().ToString());
            foreach (Player player in GameGlobals.players)
            {
                GameGlobals.gameLogManager.WritePlayerResultsToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), player.GetId().ToString(), player.GetName(), player.GetMoney().ToString());
            }

            numPlayersToStartLastDecisions = GameGlobals.players.Count;
            GameGlobals.currGameRoundId++;

            //start next game round whenever ready
            if (!GameProperties.isSimulation)
            {
                UIAddAlbumToCollection(currAlbum);
                UInewRoundScreen.SetActive(true);
            }
            else
            {
                StartGameRoundForAllPlayers("SimAlbum");
            }

            if (GameGlobals.albums.Count < GameProperties.numberOfAlbumsPerGame)
            {
                currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);
                foreach (var player in GameGlobals.players)
                {
                    player.InformNewAlbum();
                }
            }


            //enter international market on the next album, increase the number of dices played for market
            if (GameGlobals.currGameRoundId == marketLimit)
            {
                currNumberOfMarketDices++;

                //poppups are not displayed on simulations
                if (!GameProperties.isSimulation)
                {
                    infoPoppupNeutralRef.DisplayPoppup("You gained some experience publishing your last albums and so you will try your luck on the international market. From now on, 3 dices (instead of 2) are rolled for the market.");
                }
            }


            //reinit some things for next game if game result is known or max albums are achieved
            if (GameGlobals.currGameState != GameProperties.GameState.NOT_FINISHED)
            {
                GameGlobals.currGameRoundId = 0;
                //GameGlobals.currGameState = GameProperties.GameState.NOT_FINISHED;
                Debug.Log("GameGlobals.currGameState: " + GameGlobals.currGameState);

                //move albums to root so they can be saved through scenes
                foreach (Album album in GameGlobals.albums)
                {
                    UIRemoveAlbumFromCollection(album);
                    album.GetAlbumUI().SetActive(false); //do not show albums before final scene
                    Object.DontDestroyOnLoad(album.GetAlbumUI()); //can only be made after getting the object on root
                }

                if(GameGlobals.currGameState == GameProperties.GameState.LOSS)
                {
                    foreach(Player player in GameGlobals.players)
                    {
                        player.TakeAllMoney();
                    }

                    if (!GameProperties.isSimulation)
                    {
                        endPoppupLossRef.DisplayPoppup("The band incurred in too much debt! No more albums can be produced!");
                    }
                }
                else
                {
                    if (!GameProperties.isSimulation)
                    {
                        endPoppupWinRef.DisplayPoppup("The band had a successful journey! Congratulations!");
                    }
                }


                //players see the game result
                currSpeakingPlayerId = Random.Range(0, GameGlobals.numberOfSpeakingPlayers);
                foreach (Player player in GameGlobals.players)
                {
                    player.InformGameResult(GameGlobals.currGameState);
                }



                UIadvanceRoundButton.gameObject.SetActive(false);
                UIPrototypeArea.gameObject.SetActive(false);
                this.gameMainSceneFinished = true;
                
                if (GameProperties.isSimulation)
                {
                    GameSceneManager.LoadEndScene();
                }
            }
        }

    }


    public void StartChoosePreferredInstrumentPhase()
    {
        ResetAllPlayers();
        int numPlayers = GameGlobals.players.Count;
        GameGlobals.players[0].ChoosePreferredInstrumentRequest(currAlbum);
    }
    public void StartLevelingUpPhase()
    {
        ResetAllPlayers();
        int numPlayers = GameGlobals.players.Count;
        GameGlobals.players[0].LevelUpRequest(currAlbum);
    }
    public void StartPlayForInstrumentPhase()
    {
        ResetAllPlayers();
        int numPlayers = GameGlobals.players.Count;
        GameGlobals.players[0].PlayForInstrumentRequest(currAlbum);
    }
    public void StartLastDecisionsPhase()
    {
        ResetAllPlayers();
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        int numPlayers = GameGlobals.players.Count;
        GameGlobals.players[0].LastDecisionsPhaseRequest(currAlbum);
    }


    //------------------------------------------Responses---------------------------------------
    public void ChoosePreferredInstrumentResponse(Player invoker)
    {
        //auto level up after choosing instrument
        invoker.SpendToken(invoker.GetPreferredInstrument());

        choosePreferedInstrumentResponseReceived = true;
    }
    public void LevelUpResponse(Player invoker)
    {
        if(invoker.lastInstrumentToken == GameProperties.Instrument.MARKETING)
        {
            foreach (var player in GameGlobals.players)
            {
                player.InformPlayingSelfish(invoker);
            }
        }
        invoker.AskBeSure();
    }
    public void PlayerPlayForInstrumentResponse(Player invoker)
    {
        GameProperties.Instrument rollDiceInstrument = invoker.GetDiceRollInstrument();
        if (rollDiceInstrument != GameProperties.Instrument.NONE) //if there is a roll dice instrument
        {
            int newAlbumInstrumentValue = RollDicesForInstrument(invoker, rollDiceInstrument);
            invoker.SetAlbumContribution(rollDiceInstrument, newAlbumInstrumentValue);
            currAlbum.SetInstrumentValue(invoker.GetDiceRollInstrument(), newAlbumInstrumentValue);
        }
        playForInstrumentResponseReceived = true;
    }
    public void LastDecisionsPhaseGet0Response(Player invoker)
    {
    //    //receive 1000
    //    invoker.ReceiveMoney(GameProperties.tokenValue);
        lastDecisionResponseReceived = true;
    }
    public void LastDecisionsPhaseGet3000Response(Player invoker)
    {
        //receive 3000
        invoker.ReceiveMoney(3000);
        lastDecisionResponseReceived = true;
    }
    public void LastDecisionsPhaseGetMarketingResponse(Player invoker)
    {
        //roll dices for marketing
        int marketingValue = RollDicesForInstrument(invoker, GameProperties.Instrument.MARKETING);
        invoker.SetAlbumContribution(GameProperties.Instrument.MARKETING, marketingValue);
        invoker.ReceiveMoney(GameProperties.marketingPointValue * marketingValue);

        lastDecisionResponseReceived = true;
    }

    public void BeSureResponse(Player invoker, bool value)
    {
        var state = "KEEP";
        if (!value)
        {
            state = "CHANGED";
        }
        GameGlobals.gameLogManager.WriteChangeDecisionToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), invoker.GetId().ToString(), invoker.GetName().ToString(), invoker.lastInstrumentToken.ToString(), state);

        if (!value)
        {
            invoker.ToggleToken();
        }

        beSureResponseReceived = true;
    }


    public Player ChangeToNextPlayer(Player currPlayer)
    {
        currPlayerIndex = (currPlayerIndex + 1) % GameGlobals.players.Count;
        Player nextPlayer = GameGlobals.players[currPlayerIndex];
        ChangeActivePlayerUI((UIPlayer) nextPlayer, 2.0f);
        return nextPlayer;
    }

    public void UIAddAlbumToCollection(Album albumToAdd)
    {
        int albumsSize = GameGlobals.albums.Count;
        GameObject currAlbumUI = albumToAdd.GetAlbumUI();

        Animator animator = currAlbumUI.GetComponentInChildren<Animator>();
        animator.Rebind();
        animator.Play(0);

        currAlbumUI.transform.SetParent(UIAlbumCollectionDisplay.transform);
        currAlbumUI.transform.localPosition = new Vector3(0, 0, 0);
        currAlbumUI.transform.localScale = new Vector3(1, 1, 1);

        currAlbumUI.transform.Translate(new Vector3(albumsSize * 50.0f, 0, 0));
    }
    public void UIRemoveAlbumFromCollection(Album albumToRemove)
    {
        GameObject currAlbumUI = albumToRemove.GetAlbumUI();
        currAlbumUI.transform.SetParent(null);
    }


    public Player GetCurrentPlayer()
    {
        return GameGlobals.players[this.currPlayerIndex];
    }
    public int GetCurrSpeakingPlayerId()
    {
        return this.currSpeakingPlayerId;
    }

}
