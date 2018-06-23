using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    private IUtilities gameUtilities;

    public GameObject playerUIPrefab;
    public GameObject albumUIPrefab;
    public GameObject canvas;

    private int numPlayersToLevelUp;
    private int numPlayersToPlayForInstrument;
    private int numPlayersToStartLastDecisions;

    //------------ UI -----------------------------

    public GameObject UInewRoundScreen;
    public Button UIstartNewRoundButton;
    public Text UIalbumNameText;

    public Text UIcurrMarketValueText;


    public GameObject UIRollDiceForInstrumentOverlay;
    public GameObject UIRollDiceForMarketValueOverlay;


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
        int numPlayers = GameGlobals.players.Count;

        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.InitGameData();
            if ((currPlayer as UIPlayer) != null) //check if player has UI
            {
                ((UIPlayer)currPlayer).InitUI(playerUIPrefab, canvas);
            }
            currPlayer.ReceiveTokens(2);
        }

        UIRollDiceForInstrumentOverlay.SetActive(false);
        UIRollDiceForMarketValueOverlay.SetActive(false);

        currGameRound = 0; //first round
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

        if (GameProperties.isSimulation) //start imidiately in simulation
        {
            StartGameRoundForAllPlayers("SimAlbum");
        }
    }

    public void StartGameRoundForAllPlayers(string albumName)
    {
        if (GameGlobals.albums.Count > 0)
        {
            GameObject currAlbumUI = GameGlobals.albums[GameGlobals.albums.Count - 1].GetAlbumUI();
            currAlbumUI.SetActive(false);
        }

        Album newAlbum = new Album(albumName, albumUIPrefab, canvas);
        newAlbum.GetAlbumUI().SetActive(true);
        GameGlobals.albums.Add(newAlbum);

        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            currPlayer.InitAlbumContributions();
            currPlayer.tokensBoughtOnCurrRound = 0;
        }

        StartLevelingUpPhase();
    }



    public int RollDicesForInstrument(Player currPlayer, GameProperties.Instrument instrument)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];

        var skillSet = currPlayer.GetSkillSet();

        //UI stuff
        Image diceImage = UIRollDiceForInstrumentOverlay.transform.Find("dice6").GetComponent<Image>();
        Animator diceAnimator = diceImage.GetComponent<Animator>();
        List<Sprite> diceNumSprites = new List<Sprite>();


        int newAlbumInstrumentValue = 0;
        int numTokensForInstrument = skillSet[instrument];
        for (int i = 0; i < numTokensForInstrument; i++)
        {
            int randomIncrease = gameUtilities.RollTheDice(6);
            newAlbumInstrumentValue += randomIncrease;

            diceNumSprites.Add(Resources.Load<Sprite>("Animations/dice6/sprites_3/endingAlternatives/" + randomIncrease));
            Debug.Log(randomIncrease);

        }
        StartCoroutine(PlayDiceUI(diceImage.GetComponent<Animator>(), diceImage, diceNumSprites, 3.0f));

        return newAlbumInstrumentValue;
    }
    private IEnumerator PlayDiceUI(Animator diceAnimator, Image diceImage, List<Sprite> diceNumberSprites, float delayToClose)
    {
        if (diceNumberSprites.Count > 0)
        {
            Debug.Log("rolling dice ui...");
            UIRollDiceForInstrumentOverlay.SetActive(true);
            diceAnimator.Play(0);
            while (!diceAnimator.GetCurrentAnimatorStateInfo(0).IsName("endState"))
            {
                yield break;
            }

            diceImage.overrideSprite = diceNumberSprites[0];
            yield return new WaitForSeconds(delayToClose);
            diceNumberSprites.RemoveAt(0);
            StartCoroutine(PlayDiceUI(diceAnimator, diceImage, diceNumberSprites, delayToClose));
            UIRollDiceForInstrumentOverlay.SetActive(false);
        }
    }


    public int RollDicesForMarketValue()
    {
        int marketValue = gameUtilities.RollTheDice(40);
        UIcurrMarketValueText.text = marketValue.ToString();
        return marketValue;
    }

    private IEnumerator HideObjectAfterAWhile(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    public void CheckAlbumResult()
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];

        int numPlayers = GameGlobals.players.Count;
        
        int newAlbumValue = currAlbum.CalcAlbumValue();
        int marketValue = RollDicesForMarketValue();
        
        if (newAlbumValue >= marketValue)
        {
            currAlbum.SetMarketingState(GameProperties.AlbumMarketingState.MEGA_HIT);
        }
        else
        {
            currAlbum.SetMarketingState(GameProperties.AlbumMarketingState.FAIL);
        }
        
    }


    private IEnumerator ShowScreenWithDelay(GameObject screen, float delay)
    {
        yield return new WaitForSeconds(delay);
        screen.SetActive(true);
    }

    // wait for all players to exit one phase and start other phase
    void Update () {

        //end of first phase; trigger second phase
        if (numPlayersToLevelUp == 0)
        {
            StartPlayForInstrumentPhase();
            numPlayersToLevelUp = GameGlobals.players.Count;
        }

        //end of second phase; trigger album result
        if (numPlayersToPlayForInstrument == 0)
        {
            CheckAlbumResult();
            StartLastDecisionsPhase();

            numPlayersToPlayForInstrument = GameGlobals.players.Count;
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

            //reinit some things for next game round
            if (numPlayedAlbums >= GameProperties.numberOfAlbumsPerGame)
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
                StartCoroutine(ShowScreenWithDelay(UInewRoundScreen, 2.0f));
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
        if (rollDiceInstrument != (GameProperties.Instrument) (-1)) //if there is a roll dice instrument
        {
            int newAlbumInstrumentValue = RollDicesForInstrument(invoker, rollDiceInstrument);
            invoker.SetAlbumContribution(rollDiceInstrument, newAlbumInstrumentValue);
            currAlbum.SetInstrumentValue(invoker.GetDiceRollInstrument(), newAlbumInstrumentValue);
        }
        ChangeToNextPlayer(invoker);
        numPlayersToPlayForInstrument--;
    }
    public void LastDecisionsPhaseGet1000Response(Player invoker)
    {
        //receive 1000
        invoker.ReceiveMoney(GameProperties.tokenValue);
        ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
    }
    public void LastDecisionsPhaseGet3000Response(Player invoker)
    {
        //receive 3000
        invoker.ReceiveMoney(GameProperties.tokenValue*3);
        ChangeToNextPlayer(invoker);
        numPlayersToStartLastDecisions--;
    }
    public void LastDecisionsPhaseGetMarktingResponse(Player invoker)
    {
        //roll dices for markting
        int marktingValue = 0;
        for (int k = 0; k < invoker.GetSkillSet()[GameProperties.Instrument.MARKTING]; k++)
        {
            int randomIncrease = gameUtilities.RollTheDice(6);
            marktingValue += randomIncrease;
        }
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
