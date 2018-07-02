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

    public WarningScreenFunctionalities warningScreenRef;

    private int currGameRound;

    void Awake()
    {
        gameUtilities = new RandomUtilities();


        //mock to test
        GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        GameGlobals.players = new List<Player>(GameProperties.numberOfPlayersPerGame);
        GameGlobals.players.Add(new UIPlayer("PL1"));
        GameGlobals.players.Add(new UIPlayer("PL2"));
        GameGlobals.players.Add(new UIPlayer("PL3"));
    }

    public void InitGame()
    {
        canCheckAlbumResult = false;
        int numPlayers = GameGlobals.players.Count;

        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.InitGameData();
            if ((currPlayer as UIPlayer) != null) //check if player has UI
            {
                ((UIPlayer)currPlayer).InitUI(playerUIPrefab, canvas, warningScreenRef);
            }
            currPlayer.ReceiveTokens(2);
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
        //yield return new WaitForSeconds(delay);
        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            if (GameGlobals.players[i] == player)
            {
                player.GetPlayerUI().SetActive(true);
                continue;
            }
            UIPlayer currPlayer = (UIPlayer)GameGlobals.players[i];
            currPlayer.GetPlayerUI().SetActive(false);
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
        if (GameGlobals.albums.Count > 0)
        {
            GameObject currAlbumUI = GameGlobals.albums[GameGlobals.albums.Count - 1].GetAlbumUI();
            currAlbumUI.SetActive(false);
        }

        Album newAlbum = new Album(albumName, albumUIPrefab);
        newAlbum.GetAlbumUI().SetActive(true);
        GameGlobals.albums.Add(newAlbum);

        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.InitAlbumContributions();
        }

        StartLevelingUpPhase();
    }



    public int RollDicesForInstrument(Player currPlayer, GameProperties.Instrument instrument)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];

        var skillSet = currPlayer.GetSkillSet();

        int newAlbumInstrumentValue = 0;
        int numTokensForInstrument = skillSet[instrument];

        //UI stuff
        UIRollDiceForInstrumentOverlay.transform.Find("title/Text").GetComponent<Text>().text = "Rolling dice "+ numTokensForInstrument + " times for " + instrument.ToString() + " ...";
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
                StartCoroutine(PlayDiceUI(i, 6, dice6UI, currDiceNumberSprite, 2.0f));
            }
        }

        return newAlbumInstrumentValue;
    }
    private IEnumerator PlayDiceUI(int sequenceNumber, int diceNum, GameObject diceImagePrefab, Sprite currDiceNumberSprite, float delayToClose) 
    //the sequence number aims to void dice overlaps as it represents the order for which this dice is going to be rolled. We do not want to roll a dice two times for the same place
    {
        UIRollDiceForInstrumentOverlay.SetActive(true);
        GameObject diceImageClone = Instantiate(diceImagePrefab, UIRollDiceForInstrumentOverlay.transform);

        Image diceImage = diceImageClone.GetComponent<Image>();
        Animator diceAnimator = diceImage.GetComponent<Animator>();

        sequenceNumber = (sequenceNumber % 2==0)? sequenceNumber : -sequenceNumber;

        diceImage.transform.Translate(new Vector3(Random.Range(-80.0f, 80.0f), Random.Range(-80.0f, 80.0f), 0));

        diceImage.transform.Rotate(new Vector3(0, 0, 1), sequenceNumber * (360.0f / diceNum));
        diceImage.overrideSprite = null;
        diceAnimator.Rebind();
        diceAnimator.Play(0);
        diceAnimator.speed = Random.Range(0.8f,1.4f);
        while (!diceAnimator.GetCurrentAnimatorStateInfo(0).IsName("endState"))
        {
            yield return null;
        }
        diceImage.overrideSprite = currDiceNumberSprite;

        yield return new WaitForSeconds(delayToClose);

        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        UIRollDiceForInstrumentOverlay.SetActive(false);
        Destroy(diceImageClone);
    }

    public int RollDicesForMarketValue()
    {
        int marketValue = 0; 
        for(int i=0; i < 2; i++)
        {
            int randomIncrease = gameUtilities.RollTheDice(20);
            Sprite currDiceNumberSprite = Resources.Load<Sprite>("Animations/RollDiceForInstrumentOverlay/dice20/sprites/endingAlternatives/" + randomIncrease);
            StartCoroutine(PlayDiceUI(i, 20, dice20UI, currDiceNumberSprite, 4.0f));

            marketValue += randomIncrease;
        }
        return marketValue;
    }
    
    public void CheckAlbumResult()
    {
        int numAlbums = GameGlobals.albums.Count;
        Album currAlbum = GameGlobals.albums[numAlbums - 1];

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

        //check for victory or loss on album registry
        float victoryThreshold = Mathf.Ceil(GameProperties.numberOfAlbumsPerGame / 2.0f);
        if ((float)numMegaHits > victoryThreshold)
        {
            GameGlobals.currGameState = GameProperties.GameState.VICTORY;
        }
        else
        {
            float numAlbumsLeft = (float)(GameProperties.numberOfAlbumsPerGame - numAlbums);
            if (numAlbumsLeft < victoryThreshold-numMegaHits)
            {
                GameGlobals.currGameState = GameProperties.GameState.LOSS;
            }
        }
    }


    // wait for all players to exit one phase and start other phase
    void Update () {

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
            if (canCheckAlbumResult)
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
            Album currAlbum = GameGlobals.albums[numPlayedAlbums - 1];

            //write curr game logs
            FileManager.WriteAlbumResultsToLog(GameGlobals.currGameId.ToString(), currGameRound.ToString(), currAlbum.GetId().ToString(), currAlbum.GetName(), currAlbum.GetMarketingState().ToString());
            foreach(Player player in GameGlobals.players)
            {
                FileManager.WritePlayerResultsToLog(GameGlobals.currGameId.ToString(), currGameRound.ToString(), player.GetId().ToString(), player.GetName(), player.GetMoney().ToString());
            }

            //reinit some things for next game if game result is known or max albums are achieved
            if (GameGlobals.currGameState != GameProperties.GameState.NOT_FINISHED  || numPlayedAlbums >= GameProperties.numberOfAlbumsPerGame)
            {
                currGameRound=0;
                GameSceneManager.LoadEndScene();
                return;
            }
            numPlayersToStartLastDecisions = GameGlobals.players.Count;
            currGameRound++;

            //start next game round whenever ready
            if (!GameProperties.isSimulation)
            {
                //disable old album before loading new game round screen
                if (GameGlobals.albums.Count > 0)
                {
                    GameObject currAlbumUI = GameGlobals.albums[GameGlobals.albums.Count - 1].GetAlbumUI();
                    currAlbumUI.SetActive(false);
                }
                UInewRoundScreen.SetActive(true);
            }
            else
            {
                StartGameRoundForAllPlayers("SimAlbum");
            }
        }

    }


    public void StartLevelingUpPhase()
    {
        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.LevelUpRequest();
        }
    }
    public void StartPlayForInstrumentPhase()
    {
        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.PlayForInstrumentRequest();
        }
    }
    public void StartLastDecisionsPhase()
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.LastDecisionsPhaseRequest(currAlbum);
        }

    }

    public void LevelUpResponse(Player invoker)
    {
        ChangeToNextPlayer(invoker);
        numPlayersToLevelUp--;
    }
    public void PlayerPlayForInstrumentResponse(Player invoker)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        GameProperties.Instrument rollDiceInstrument = invoker.GetDiceRollInstrument();
        if (rollDiceInstrument != GameProperties.Instrument.NONE) //if there is a roll dice instrument
        {
            int newAlbumInstrumentValue = RollDicesForInstrument(invoker, rollDiceInstrument);
            invoker.SetAlbumContribution(rollDiceInstrument, newAlbumInstrumentValue);
            currAlbum.SetInstrumentValue(invoker.GetDiceRollInstrument(), newAlbumInstrumentValue);

            currAlbum.CalcAlbumValue(); //update album value and ui after playing for instrument
        }

        ChangeToNextPlayer(invoker);
        numPlayersToPlayForInstrument--;
    }
    public void LastDecisionsPhaseGet1000Response(Player invoker)
    {
        //receive 1000
        invoker.ReceiveMoney(GameProperties.tokenValue);
        invoker.ReceiveTokens(1);
        ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
    }
    public void LastDecisionsPhaseGet3000Response(Player invoker)
    {
        //receive 3000
        invoker.ReceiveMoney(GameProperties.tokenValue*3);
        invoker.ReceiveTokens(1);
        ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
    }
    public void LastDecisionsPhaseGetMarktingResponse(Player invoker)
    {
        //roll dices for markting
        int marktingValue = RollDicesForInstrument(invoker, GameProperties.Instrument.MARKTING);
            
        invoker.SetAlbumContribution(GameProperties.Instrument.MARKTING, marktingValue);
        invoker.ReceiveMoney(GameProperties.tokenValue * marktingValue);
        ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
    }



    public void ChangeToNextPlayer(Player currPlayer)
    {
        Player nextPlayer = GameGlobals.players[(GameGlobals.players.IndexOf(currPlayer) + 1) % GameGlobals.players.Count];
        ChangeActivePlayerUI((UIPlayer) nextPlayer, 2.0f);
    }
}
