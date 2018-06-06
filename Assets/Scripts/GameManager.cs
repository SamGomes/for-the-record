using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    private IUtilities gameUtilities;

    private int numRounds;
    private List<Album> albums;

    private List<Player> players;

    public GameObject playerUIPrefab;
    public GameObject albumUIPrefab;
    public GameObject canvas;

    private int numPlayersToLevelUp;
    private int numPlayersToPlayForInstrument;

    //------------ UI -----------------------------

    public GameObject UInewRoundScreen;
    public Button UIstartNewRoundButton;
    public Text UIalbumNameText;

    public Text UIcurrMarketValueText;
    private int currMarketValue;


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
            currPlayer.UpdateUI();
        }
    }

    private IEnumerator ChangeActivePlayerUI(Player player, float delay)
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
            Player currPlayer = players[i];
            currPlayer.GetPlayerUI().SetActive(false);
        }
    }

    void Start()
    {
        
        numRounds = 5;

        players.Add(new HumanPlayer("John", playerUIPrefab, canvas));
        players.Add(new HumanPlayer("Mike", playerUIPrefab, canvas));
        players.Add(new HumanPlayer("Bob", playerUIPrefab, canvas));

        InitGame();

        UIstartNewRoundButton.onClick.AddListener(delegate () {
            UInewRoundScreen.SetActive(false);
            StartGameRoundForAllPlayers(UIalbumNameText.text);
        });

        numPlayersToLevelUp = players.Count;
        numPlayersToPlayForInstrument = players.Count;
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
        }

        StartLevelingUpPhase();
        //for (int actionsTaken = 0; actionsTaken < GameProperties.allowedPlayerActionsPerAlbum; actionsTaken++)
        //{
        //    for (int i = 0; i < numPlayers; i++)
        //    {
        //        int approachedPlayerIndex = (currPlayerIndex + i) % numPlayers;
        //        Player currPlayer = players[approachedPlayerIndex];

        //        Debug.Log("StartGameRoundForThisPlayers...");
        //        LevelingUpPhase();
        //        numPlaysToBeDoneOnCurrRound++;
        //    }
        //}
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

        for (int i = 0; i < numPlayers; i++)
        {
            if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                //roll dices for markting
                int marktingValue = 0;
                for (int k = 0; k < players[i].GetSkillSet()[GameProperties.Instrument.MARKTING]; k++)
                {
                    int randomIncrease = gameUtilities.RollTheDice(6);
                    marktingValue += randomIncrease;
                }
                players[i].ReceiveMoney(GameProperties.tokenValue * marktingValue);
                
                ////receive 3000
                //players[i].ReceiveMoney(GameProperties.tokenValue * 3);
            }
            else
            {
                //receive 1000
                players[i].ReceiveMoney(GameProperties.tokenValue);
            }
            players[i].ReceiveTokens(2);
        }
    }


    private IEnumerator showScreenWithDelay(GameObject screen, float delay)
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
            StartCoroutine(showScreenWithDelay(UInewRoundScreen, 2.0f));

            numPlayersToPlayForInstrument = players.Count;
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
    public void LevelUpResponse(Player invoker)
    {
        ChangeToNextPlayer(invoker);
        numPlayersToLevelUp--;
    }
    public void PlayerPlayForInstrumentResponse(Player invoker)
    {
        GameProperties.Instrument rollDiceInstrument = invoker.GetDiceRollInstrument();
        RollDicesForInstrument(invoker, rollDiceInstrument);

        ChangeToNextPlayer(invoker);
        numPlayersToPlayForInstrument--;
    }

    public void ChangeToNextPlayer(Player currPlayer)
    {
        Player nextPlayer = players[(players.IndexOf(currPlayer) + 1) % players.Count];
        StartCoroutine(ChangeActivePlayerUI(nextPlayer, 2.0f));
    }
}
