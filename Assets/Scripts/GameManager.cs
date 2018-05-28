using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private IUtilities gameUtilities;

    private int numRounds;
    private List<Album> albums;

    private List<Player> players;
    int currPlayerIndex;
    


    void Awake()
    {
        gameUtilities = new RandomUtilities();
        albums = new List<Album>(numRounds);
        players = new List<Player>();

    }

    void Start () {
        currPlayerIndex = 0;
        numRounds = 5;
        
        players.Add(new GreedyPlayer("John"));
        players.Add(new GreedyPlayer("Mike"));
        players.Add(new GreedyPlayer("Bob"));

        int numPlayers = players.Count;

        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = players[i];
            currPlayer.ReceiveTokens(2);

            currPlayer.ExecuteActionRequest();
        }
    }


	// Update is called once per frame
	void Update () {
        //simulate round
        if (Input.GetKeyDown(KeyCode.Space)){
            Album newAlbum = new Album("a1");
            int numPlayers = players.Count;

            for (int actionsTaken = 0; actionsTaken < GameProperties.allowedPlayerActionsPerAlbum; actionsTaken++)
            {
                for (int i = 0; i < numPlayers; i++)
                {
                    int approachedPlayerIndex = (currPlayerIndex + i) % numPlayers;
                    Player currPlayer = players[approachedPlayerIndex];

                    currPlayer.ExecuteActionRequest();

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
	}
}
