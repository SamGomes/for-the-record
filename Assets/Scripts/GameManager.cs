using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    private IUtilities gameUtilities;

    private int numRounds;
    private List<Album> albums;

    private List<Player> players;
    int currPlayerIndex;
 

    public GameObject playerUIPrefab;
    public GameObject albumUIPrefab;
    public GameObject canvas;

    public Text UIcurrMarketValueText;
    private int currMarketValue;

    public Button UIshowCurrAlbumButton;
    private bool showingCurrAlbumUI;

    private int numPlaysToBeDoneOnCurrRound;

    void Awake()
    {
        gameUtilities = new RandomUtilities();
        albums = new List<Album>(numRounds);
        players = new List<Player>();

    }

    void Start () {
        numPlaysToBeDoneOnCurrRound = -1;
        currPlayerIndex = 0;
        numRounds = 5;
        
        players.Add(new HumanPlayer("John", playerUIPrefab, canvas));
        players.Add(new HumanPlayer("Mike", playerUIPrefab, canvas));
        players.Add(new HumanPlayer("Bob", playerUIPrefab, canvas));

        InitGame();

        UIshowCurrAlbumButton.onClick.AddListener(delegate { ShowHideCurrAlbumUI(); });
        showingCurrAlbumUI = false;
    }
    

    public void InitGame()
    {
        int numPlayers = players.Count;

        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
            currPlayer.ReceiveTokens(2);
            currPlayer.UpdateUI();
        }
    }

    public void ChangeActivePlayerUI(Player player)
    {
        int numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            if (players[i] == player)
            {
                player.GetPlayerUI().SetActive(true);
                continue;
            }
            Player currPlayer = players[i];
            currPlayer.GetPlayerUI().SetActive(false);
        }
    }

    

    public void StartGameRoundForAllPlayers()
    {
        int numPlayers = players.Count;
        numPlaysToBeDoneOnCurrRound = 0;
        for (int actionsTaken = 0; actionsTaken < GameProperties.allowedPlayerActionsPerAlbum; actionsTaken++)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                int approachedPlayerIndex = (currPlayerIndex + i) % numPlayers;
                Player currPlayer = players[approachedPlayerIndex];

                Debug.Log("StartGameRoundForThisPlayers...");
                currPlayer.ExecuteActionRequest();
                numPlaysToBeDoneOnCurrRound++;
            }
        }
    }
    public void RollDicesForInstrumentsAndMarketing(Player currPlayer)
    {
        Debug.Log("RollDicesForInstrumentsAndMarketing");
        Album currAlbum = albums[albums.Count - 1];

        var skillSet = currPlayer.GetSkillSet();
        List<GameProperties.Instrument> currPlayerKeys = new List<GameProperties.Instrument>(skillSet.Keys);
        for (int j = 0; j < currPlayerKeys.Count; j++)
        {
            GameProperties.Instrument currInstrument = currPlayerKeys[j];
            if (currInstrument == GameProperties.Instrument.MARKTING)
            {
                continue;
            }
            int newAlbumInstrumentValue = 0;

            int numTokensForInstrument = skillSet[currInstrument];

            for(int i=0; i<numTokensForInstrument; i++)
            {
                int randomIncrease = gameUtilities.RollTheDice(6);
                newAlbumInstrumentValue += randomIncrease;
            }
            currPlayer.SetAlbumContribution(currInstrument, newAlbumInstrumentValue);

            currAlbum.SetInstrumentValue(currPlayer.GetPreferredInstrument(), newAlbumInstrumentValue);
        }
        for (int j = 0; j < currPlayer.GetSkillSet()[GameProperties.Instrument.MARKTING]; j++)
        {
            currPlayer.ReceiveMoney(GameProperties.tokenValue * gameUtilities.RollTheDice(6));
        }


    }

    IEnumerator hideObjectAfterAWhile(GameObject obj, float time)
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
        int newAlbumValue = currAlbum.GetAlbumValue();
        
        if (newAlbumValue >= marketValue)
        {
            currAlbum.SetMarketingState(GameProperties.AlbumMarketingState.MEGA_HIT);
        }
        else
        {
            currAlbum.SetMarketingState(GameProperties.AlbumMarketingState.FAIL);
        }

        for (int i = 0; i < numPlayers; i++)
        {
            if(currAlbum.GetMarketingState()== GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                players[i].ReceiveMoney(GameProperties.tokenValue * newAlbumValue);
            }
            else
            {
                players[i].ReceiveMoney(GameProperties.tokenValue);
            }
            players[i].ReceiveTokens(2);
        }

        //display album ui on round end
        GameObject currAlbumUI = albums[albums.Count - 1].GetAlbumUI();
        currAlbumUI.SetActive(true);
        StartCoroutine(hideObjectAfterAWhile(currAlbumUI, 5.0f));
    }

    public void CurrPlayerActionExecuted(Player invoker)
    {
        RollDicesForInstrumentsAndMarketing(invoker);
        Debug.Log((players.IndexOf(invoker) + 1) % players.Count);
        ChangeActivePlayerUI(players[(players.IndexOf(invoker) + 1) % players.Count]); //compute next player
        numPlaysToBeDoneOnCurrRound--;
    }

    public void ShowHideCurrAlbumUI()
    {
        if (albums.Count == 0)
        {
            return;
        }
        GameObject currAlbumUI = albums[albums.Count - 1].GetAlbumUI();
        currAlbumUI.SetActive(!showingCurrAlbumUI);
        showingCurrAlbumUI = !showingCurrAlbumUI;
    }


    // Update is called once per frame
    void Update () {
        //simulate round
        if (Input.GetKeyDown(KeyCode.N)){
            Album newAlbum = new Album("newAlbum", albumUIPrefab, canvas);
            newAlbum.GetAlbumUI().SetActive(false);
            albums.Add(newAlbum);

            StartGameRoundForAllPlayers();
        }

        if (numPlaysToBeDoneOnCurrRound == 0)
        {
            CheckAlbumResult();
            numPlaysToBeDoneOnCurrRound = -1;
        }
    }
}
