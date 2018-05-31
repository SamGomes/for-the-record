using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class GameManager : MonoBehaviour {

    private IUtilities gameUtilities;

    private int numRounds;
    private List<Album> albums;

    private List<Player> players;
    int currPlayerIndex;

    bool isPaused;

    public GameObject playerUIPrefab;
    public GameObject canvas;

    Thread gameRoundThread;
    ManualResetEvent gameRoundThreadResetEvent;

    void Awake()
    {
        gameRoundThread = new Thread(PlayGameRound);
        gameRoundThreadResetEvent = new ManualResetEvent(false);

        gameUtilities = new RandomUtilities();
        albums = new List<Album>(numRounds);
        players = new List<Player>();

    }

    void Start () {
        isPaused = false;
        currPlayerIndex = 0;
        numRounds = 5;
        
        players.Add(new HumanPlayer("John", playerUIPrefab, canvas));
        players.Add(new HumanPlayer("Mike", playerUIPrefab, canvas));
        players.Add(new HumanPlayer("Bob", playerUIPrefab, canvas));

        InitGame();
    }

    private void PauseGameThread()
    {
        gameRoundThreadResetEvent.WaitOne();
    }
    private void ResumeGameThread()
    {
        gameRoundThreadResetEvent.Set();
    }

    public void PlayerActionExecuted()
    {
        ResumeGameThread();
    }

    public void InitGame()
    {
        int numPlayers = players.Count;

        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
            currPlayer.ReceiveTokens(2);
            currPlayer.ExecuteActionRequest();
            PauseGameThread();
        }
    }
    
    public void PlayGameRound()
    {
        Album newAlbum = new Album("a1");
        int numPlayers = players.Count;

        for (int actionsTaken = 0; actionsTaken < GameProperties.allowedPlayerActionsPerAlbum; actionsTaken++)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                int approachedPlayerIndex = (currPlayerIndex + i) % numPlayers;
                Player currPlayer = players[approachedPlayerIndex];

                Debug.Log("waitingForPlayer...");
                currPlayer.ExecuteActionRequest();
                PauseGameThread();
                Debug.Log("actionExecuted");

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

                    int randomIncrease = gameUtilities.RollTheDice(6);
                    newAlbumInstrumentValue += randomIncrease * skillSet[currInstrument];

                    newAlbum.SetInstrumentValue(currPlayer.GetPreferredInstrument(), newAlbumInstrumentValue);
                }
                for (int j = 0; j < currPlayer.GetSkillSet()[GameProperties.Instrument.MARKTING]; j++)
                {
                    currPlayer.ReceiveMoney(GameProperties.tokenValue * gameUtilities.RollTheDice(6));
                }
            }

            int marketValue = gameUtilities.RollTheDice(40);
            int newAlbumValue = newAlbum.GetAlbumValue();
            if (newAlbumValue >= marketValue)
            {
                newAlbum.SetMarketingState(GameProperties.AlbumMarketingState.MEGA_HIT);
                for (int i = 0; i < numPlayers; i++)
                {
                    players[i].ReceiveMoney(GameProperties.tokenValue * newAlbumValue);
                }

            }
        }
    }


    // Update is called once per frame
    void Update () {
        //simulate round
        if (Input.GetKeyDown(KeyCode.Space)){
            gameRoundThread.Start();
        }
	}
}
