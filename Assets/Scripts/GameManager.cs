using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    private IUtilities gameUtilities;

    private int numRounds;
    public static List<Album> albums;

    private List<Player> players;

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


    void Awake()
    {
        gameUtilities = new RandomUtilities();
        albums = new List<Album>(numRounds);
        players = new List<Player>();

    }
    
    public void InitGame()
    {
        int numPlayers = players.Count;

        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
            currPlayer.ReceiveTokens(2);
        }
    }

    //warning: works only when using human players!
    private IEnumerator ChangeActivePlayerUI(UIPlayer player, float delay)
    {
        yield return new WaitForSeconds(delay);
        int numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            if (players[i] == player)
            {
                player.GetPlayerUI().SetActive(true);
                continue;
            }
            UIPlayer currPlayer = (UIPlayer) players[i];
            currPlayer.GetPlayerUI().SetActive(false);
        }
    }

    void Start()
    {
        numRounds = 5;

        if (!GameProperties.isSimulation)
        {
            players.Add(new UIPlayer("John", playerUIPrefab, canvas));
            players.Add(new UIPlayer("Mike", playerUIPrefab, canvas));
            players.Add(new UIPlayer("Bob", playerUIPrefab, canvas));
        }
        else
        {
            players.Add(new SimPlayer("SimPL1"));
            players.Add(new SimPlayer("SimPL2"));
            players.Add(new SimPlayer("SimPL3"));
        }

        InitGame();

        UIstartNewRoundButton.onClick.AddListener(delegate () {
            UInewRoundScreen.SetActive(false);
            StartGameRoundForAllPlayers(UIalbumNameText.text);
        });

        numPlayersToLevelUp = players.Count;
        numPlayersToPlayForInstrument = players.Count;
        numPlayersToStartLastDecisions = players.Count;

        if (GameProperties.isSimulation) //start imidiately in simulation
        {
            StartGameRoundForAllPlayers("SimAlbum");
        }
    }

    public void StartGameRoundForAllPlayers(string albumName)
    {
        if (albums.Count > 0)
        {
            GameObject currAlbumUI = albums[albums.Count - 1].GetAlbumUI();
            currAlbumUI.SetActive(false);
        }

        Album newAlbum = new Album(albumName, albumUIPrefab, canvas);
        newAlbum.GetAlbumUI().SetActive(true);
        albums.Add(newAlbum);

        int numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
            currPlayer.InitAlbumContributions();
            currPlayer.tokensBoughtOnCurrRound = 0;
        }

        StartLevelingUpPhase();
    }

   

    public void RollDicesForInstrument(Player currPlayer,GameProperties.Instrument instrument)
    {
        Debug.Log("RollDicesForInstrumentsAndMarketing");
        Album currAlbum = albums[albums.Count - 1];

        var skillSet = currPlayer.GetSkillSet();


        int newAlbumInstrumentValue = 0;
        int numTokensForInstrument = skillSet[instrument];
        for(int i=0; i<numTokensForInstrument; i++)
        {
            int randomIncrease = gameUtilities.RollTheDice(6);
            newAlbumInstrumentValue += randomIncrease;
        }
        currPlayer.SetAlbumContribution(instrument, newAlbumInstrumentValue);
        currAlbum.SetInstrumentValue(currPlayer.GetDiceRollInstrument(), newAlbumInstrumentValue);
    }

    private IEnumerator HideObjectAfterAWhile(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    public void CheckAlbumResult()
    {
        Debug.Log("CheckAlbumResult");
        Album currAlbum = albums[albums.Count - 1];

        int numPlayers = players.Count;
        int marketValue = gameUtilities.RollTheDice(40);
        UIcurrMarketValueText.text = marketValue.ToString();
        int newAlbumValue = currAlbum.CalcAlbumValue();
        
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
            numPlayersToLevelUp = players.Count;
        }
        //end of second phase; trigger album result
        if (numPlayersToPlayForInstrument == 0)
        {
            CheckAlbumResult();
            StartLastDecisionsPhase();

            numPlayersToPlayForInstrument = players.Count;
        }
        //end of second phase; trigger album result
        if (numPlayersToStartLastDecisions == 0)
        {
            if (!GameProperties.isSimulation) //if human player update his/her UI, start imidiately otherwise
            {
                StartCoroutine(ShowScreenWithDelay(UInewRoundScreen, 2.0f));
            }
            else
            {
                StartGameRoundForAllPlayers("SimAlbum");
            }
            numPlayersToStartLastDecisions = players.Count;
        }

        if (albums.Count == GameProperties.numberOfAlbumsPerGame)
        {
            GameSceneManager.LoadEndScene();
        }
    }


    public void StartLevelingUpPhase()
    {
        int numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
            currPlayer.LevelUpRequest();
        }
    }
    public void StartPlayForInstrumentPhase()
    {
        int numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
            currPlayer.PlayForInstrumentRequest();
        }
    }
    public void StartLastDecisionsPhase()
    {
        Album currAlbum = albums[albums.Count - 1];
        int numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
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
        GameProperties.Instrument rollDiceInstrument = invoker.GetDiceRollInstrument();
        if (rollDiceInstrument != (GameProperties.Instrument) (-1)) //if there is a roll dice instrument
        {
            RollDicesForInstrument(invoker, rollDiceInstrument);
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
        Player nextPlayer = players[(players.IndexOf(currPlayer) + 1) % players.Count];

        if (!GameProperties.isSimulation)
        {
            StartCoroutine(ChangeActivePlayerUI((UIPlayer) nextPlayer, 2.0f));
        }
    }
}
