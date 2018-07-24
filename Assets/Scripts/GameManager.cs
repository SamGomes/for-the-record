using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    private int numMegaHits;

    private IUtilities gameUtilities;

    public GameObject canvas;

    private int numPlayersToLevelUp;
    private int numPlayersToPlayForInstrument;
    private int numPlayersToStartLastDecisions;
    private int numPlayersToChooseDiceRollInstrument;
    private bool canCheckAlbumResult;

    //------------ UI -----------------------------
    public GameObject playerUIPrefab;
    public GameObject albumUIPrefab;

    public GameObject UInewRoundScreen;
    public Button UIstartNewRoundButton;
    public Text UIalbumNameText;
    
    public GameObject UIRollDiceForInstrumentOverlay;
    public GameObject UIRollDiceForMarketValueScreen;

    public GameObject dice6UI;
    public GameObject dice20UI;

    public GameObject UIAlbumCollectionDisplay;

    public GameObject poppupPrefab;
    public PoppupScreenFunctionalities warningScreenRef;
    public PoppupScreenFunctionalities infoScreenRefAlbumLoss;
    public PoppupScreenFunctionalities infoScreenRefAlbumWin;

    private int currGameRound;

    private bool gameMainSceneFinished;
    private bool preferredInstrumentsChoosen;

    private Album currAlbum;

    private float diceRollDelay;

    void Awake()
    {
        gameUtilities = new RandomUtilities();


        ////mock to test
        GameProperties.gameLogManager.InitLogs();
        GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        GameGlobals.players = new List<Player>(GameProperties.numberOfPlayersPerGame);
        GameGlobals.players.Add(new UIPlayer("Coop Jeff"));
        GameGlobals.players.Add(new AIPlayerCoopStrategy("Greedy Kevin"));
        GameGlobals.players.Add(new UIPlayer("Balanced Sam"));
    }

    public void InitGame()
    {
        warningScreenRef = new PoppupScreenFunctionalities(poppupPrefab,canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(),Resources.Load<Sprite>("Textures/UI/Icons/Warning"), new Color(0.9f, 0.8f, 0.8f));

        infoScreenRefAlbumLoss = new PoppupScreenFunctionalities(poppupPrefab,canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(),Resources.Load<Sprite>("Textures/UI/Icons/InfoLoss"), new Color(0.9f, 0.8f, 0.8f));
        infoScreenRefAlbumWin = new PoppupScreenFunctionalities(poppupPrefab,canvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(),Resources.Load<Sprite>("Textures/UI/Icons/InfoWin"), new Color(0.8f, 0.9f, 0.8f));

        gameMainSceneFinished = false;
        preferredInstrumentsChoosen = false;

        diceRollDelay = 6.0f;

        canCheckAlbumResult = false;
        int numPlayers = GameGlobals.players.Count;

        Player currPlayer = null;
        for (int i = 0; i < numPlayers; i++)
        {
            currPlayer = GameGlobals.players[i];
            currPlayer.InitPlayer(playerUIPrefab, canvas, warningScreenRef);
            if ((currPlayer as UIPlayer) != null) //check if player has UI
            {

                ((UIPlayer)currPlayer).GetPlayerUI().transform.Translate(new Vector3(0, -i*170.0f, 0));
                //position UI correctly depending on players number (table layout)
                //float refAngle = (180.0f / (numPlayers - 1));
                //((UIPlayer)currPlayer).GetPlayerUI().transform.RotateAround(new Vector3(510, 490, 0), new Vector3(0, 0, 1), (i * refAngle));
                //((UIPlayer)currPlayer).GetPlayerUI().transform.Rotate(new Vector3(0, 0, 1), -(i*refAngle) + 90.0f);
            }
            currPlayer.ReceiveTokens(2);
        }
        if (currPlayer != null)
        {
            ChangeToNextPlayer(((UIPlayer)currPlayer)); //init marker to first player
        }

        currGameRound = 0; //first round
        numMegaHits = 0;
    }

    public int GetCurrGameRound()
    {
        return this.currGameRound;
    }

    //warning: works only when using human players!
    private IEnumerator ChangeActivePlayerUI(UIPlayer player, float delay)
    {
        player.GetPlayerUI().transform.SetAsLastSibling();
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

        UIstartNewRoundButton.onClick.AddListener(delegate () {
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
        List<Sprite> diceNumSprites = new List<Sprite>();
        
        for (int i = 0; i < numTokensForInstrument; i++)
        {
            int randomIncrease = gameUtilities.RollTheDice(6);
            newAlbumInstrumentValue += randomIncrease;

            Sprite currDiceNumberSprite = Resources.Load<Sprite>("Animations/RollDiceForInstrumentOverlay/dice6/sprites_3/endingAlternatives/" + randomIncrease);
            if (currDiceNumberSprite == null)
            {
                Debug.Log("cannot find sprite for dice number " + randomIncrease);
            }
            else
            {
                Debug.Log(randomIncrease);
                StartCoroutine(PlayDiceUI(i, 6, dice6UI, currDiceNumberSprite, "+"+randomIncrease+" Album Value", Color.yellow, diceRollDelay));
            }
        }

        return newAlbumInstrumentValue;
    }
    private IEnumerator PlayDiceUI(int sequenceNumber, int diceNum, GameObject diceImagePrefab, Sprite currDiceNumberSprite, string arrowTextString, Color arrowColor, float delayToClose) 
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

        //get and disable arrow animation until end of dice animation
        GameObject diceArrow = diceImage.transform.GetChild(0).gameObject;
        diceArrow.GetComponentInChildren<Image>().color = arrowColor;

        diceArrow.transform.Rotate(new Vector3(0, 0, 1), - diceRotation);
        Animator arrowAnimator = diceArrow.GetComponentInChildren<Animator>();
        arrowAnimator.speed = 0;
        Text arrowText = diceArrow.GetComponentInChildren<Text>();
        arrowText.text = arrowTextString;
        arrowText.color = arrowColor;


        while (!diceAnimator.GetCurrentAnimatorStateInfo(0).IsName("endState"))
        {
            yield return null;
        }
        diceImage.overrideSprite = currDiceNumberSprite;
        
        arrowAnimator.speed = 1;

        yield return new WaitForSeconds(delayToClose);
        
        UIRollDiceForInstrumentOverlay.SetActive(false);
        Destroy(diceImageClone);
    }

    public int RollDicesForMarketValue()
    {
        UIRollDiceForInstrumentOverlay.transform.Find("title/Text").GetComponent<Text>().text = "Rolling dices for market...";

        int marketValue = 0; 
        for(int i=0; i < 2; i++)
        {
            int randomIncrease = gameUtilities.RollTheDice(20);
            Sprite currDiceNumberSprite = Resources.Load<Sprite>("Animations/RollDiceForInstrumentOverlay/dice20/sprites/endingAlternatives/" + randomIncrease);
            StartCoroutine(PlayDiceUI(i, 20, dice20UI, currDiceNumberSprite, "+" + randomIncrease + " Market Value", Color.red, diceRollDelay));
            marketValue += randomIncrease;
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
            infoScreenRefAlbumWin.DisplayPoppupWithDelay("As your album value ("+ newAlbumValue +") was HIGHER than the market value ("+ marketValue +"), the album was successfully published! Congratulations! Everyone can choose to receive 3000 coins or to invest in their own marketing.", diceRollDelay);
            numMegaHits++;
        }
        else
        {
            currAlbum.SetMarketingState(GameProperties.AlbumMarketingState.FAIL);
            infoScreenRefAlbumLoss.DisplayPoppupWithDelay("As your album value (" + newAlbumValue + ") was LOWER than the market value (" + marketValue + "), the album could not be published. Although the band incurred in debt, everyone receives 1000 coins of the band savings.", diceRollDelay);
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
        if (this.gameMainSceneFinished)
        {
            return;
        }

        //end of first phase; trigger second phase
        if(!preferredInstrumentsChoosen && numPlayersToChooseDiceRollInstrument == 0)
        {
            StartLevelingUpPhase();
            //numPlayersToChooseDiceRollInstrument = GameGlobals.players.Count; //is not performed to ensure this phase is only played once
            preferredInstrumentsChoosen = true;

        }

        //end of first phase; trigger second phase
        if (numPlayersToLevelUp == 0)
        {
            StartPlayForInstrumentPhase();
            numPlayersToLevelUp = GameGlobals.players.Count;
        }
        
        //end of second phase;
        if (numPlayersToPlayForInstrument == 0)
        {
            //make phase UI active (this step is interim but must be done before last phase)
            UIRollDiceForMarketValueScreen.SetActive(true);
            if (canCheckAlbumResult || GameProperties.isSimulation)
            {
                CheckAlbumResult();
                canCheckAlbumResult = false;
                UIRollDiceForMarketValueScreen.SetActive(false);

                StartLastDecisionsPhase();
                numPlayersToPlayForInstrument = GameGlobals.players.Count;
            }
        }

        //end of third phase; trigger and log album result
        if (numPlayersToStartLastDecisions == 0)
        {
            int numPlayedAlbums = GameGlobals.albums.Count;

            //write curr game logs
            GameProperties.gameLogManager.WriteAlbumResultsToLog("0", GameGlobals.currGameId.ToString(), currGameRound.ToString(), currAlbum.GetId().ToString(), currAlbum.GetName(), currAlbum.GetMarketingState().ToString());
            foreach (Player player in GameGlobals.players)
            {
                GameProperties.gameLogManager.WritePlayerResultsToLog("0", GameGlobals.currGameId.ToString(), currGameRound.ToString(), player.GetId().ToString(), player.GetName(), player.GetMoney().ToString());
            }

            numPlayersToStartLastDecisions = GameGlobals.players.Count;
            currGameRound++;

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



            //reinit some things for next game if game result is known or max albums are achieved
            if (GameGlobals.currGameState != GameProperties.GameState.NOT_FINISHED)
            {
                currGameRound = 0;
                //GameGlobals.currGameState = GameProperties.GameState.NOT_FINISHED;
                Debug.Log("GameGlobals.currGameState: " + GameGlobals.currGameState);

                //move albums to root so they can be saved through scenes
                foreach (Album album in GameGlobals.albums)
                {
                    UIRemoveAlbumFromCollection(album);
                    Object.DontDestroyOnLoad(album.GetAlbumUI()); //can only be made after getting the object on root
                }

                if(GameGlobals.currGameState == GameProperties.GameState.LOSS)
                {
                    foreach(Player player in GameGlobals.players)
                    {
                        player.TakeAllMoney();
                    }
                }

                GameSceneManager.LoadEndScene();
                this.gameMainSceneFinished = true;
            }
        }

    }


    public void StartChoosePreferredInstrumentPhase()
    {
        ResetAllPlayers();
        int numPlayers = GameGlobals.players.Count;
        //for (int i = 0; i < numPlayers; i++)
        //{
        Player currPlayer = GameGlobals.players[0];
        currPlayer.ChoosePreferredInstrumentRequest(currAlbum);
        //}
    }
    public void StartLevelingUpPhase()
    {
        ResetAllPlayers();
        int numPlayers = GameGlobals.players.Count;
        //for (int i = 0; i < numPlayers; i++)
        //{
            Player currPlayer = GameGlobals.players[0];
            currPlayer.LevelUpRequest(currAlbum);
        //}
    }
    public void StartPlayForInstrumentPhase()
    {
        ResetAllPlayers();
        int numPlayers = GameGlobals.players.Count;
        //for (int i = 0; i < numPlayers; i++)
        //{
            Player currPlayer = GameGlobals.players[0];
            currPlayer.PlayForInstrumentRequest(currAlbum);
        //}
    }
    public void StartLastDecisionsPhase()
    {
        ResetAllPlayers();
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        int numPlayers = GameGlobals.players.Count;
        //for (int i = 0; i < numPlayers; i++)
        //{
            Player currPlayer = GameGlobals.players[0];
            currPlayer.LastDecisionsPhaseRequest(currAlbum);
        //}
    }


    //------------------------------------------Responses---------------------------------------
    public void ChoosePreferredInstrumentResponse(Player invoker)
    {
        Player nextPlayer = ChangeToNextPlayer(invoker);
        numPlayersToChooseDiceRollInstrument--;
        if (numPlayersToChooseDiceRollInstrument > 0)
        {
            nextPlayer.ChoosePreferredInstrumentRequest(currAlbum);
        }
    }
    public void LevelUpResponse(Player invoker)
    {
        Player nextPlayer = ChangeToNextPlayer(invoker);
        numPlayersToLevelUp--;
        if (numPlayersToLevelUp > 0)
        {
            nextPlayer.LevelUpRequest(currAlbum);
        }
    }
    public void PlayerPlayForInstrumentResponse(Player invoker)
    {
        GameProperties.Instrument rollDiceInstrument = invoker.GetDiceRollInstrument();
        if (rollDiceInstrument != GameProperties.Instrument.NONE) //if there is a roll dice instrument
        {
            int newAlbumInstrumentValue = RollDicesForInstrument(invoker, rollDiceInstrument);
            invoker.SetAlbumContribution(rollDiceInstrument, newAlbumInstrumentValue);
            currAlbum.SetInstrumentValue(invoker.GetDiceRollInstrument(), newAlbumInstrumentValue);

            currAlbum.CalcAlbumValue(); //update album value and ui after playing for instrument
        }

        Player nextPlayer = ChangeToNextPlayer(invoker);
        numPlayersToPlayForInstrument--;
        if (numPlayersToPlayForInstrument > 0)
        {
            nextPlayer.PlayForInstrumentRequest(currAlbum);
        }
    }
    public void LastDecisionsPhaseGet1000Response(Player invoker)
    {
        //receive 1000
        invoker.ReceiveMoney(GameProperties.tokenValue);

        Player nextPlayer = ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
        if (numPlayersToStartLastDecisions > 0)
        {
            nextPlayer.LastDecisionsPhaseRequest(currAlbum);
        }
    }
    public void LastDecisionsPhaseGet3000Response(Player invoker)
    {
        //receive 3000
        invoker.ReceiveMoney(GameProperties.tokenValue*3);

        Player nextPlayer = ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
        if (numPlayersToStartLastDecisions > 0)
        {
            nextPlayer.LastDecisionsPhaseRequest(currAlbum);
        }

    }
    public void LastDecisionsPhaseGetMarktingResponse(Player invoker)
    {
        //roll dices for markting
        int marktingValue = RollDicesForInstrument(invoker, GameProperties.Instrument.MARKETING);
            
        invoker.SetAlbumContribution(GameProperties.Instrument.MARKETING, marktingValue);
        invoker.ReceiveMoney(GameProperties.tokenValue * marktingValue);

        Player nextPlayer = ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
        if (numPlayersToStartLastDecisions > 0)
        {
            nextPlayer.LastDecisionsPhaseRequest(currAlbum);
        }
    }



    public Player ChangeToNextPlayer(Player currPlayer)
    {
        Player nextPlayer = GameGlobals.players[(GameGlobals.players.IndexOf(currPlayer) + 1) % GameGlobals.players.Count];
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

}
