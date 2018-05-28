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
        
        players.Add(new Player("John"));
        players.Add(new Player("Mike"));
        players.Add(new Player("Bob"));
	}
	
    void SimulateRound(int approachedPlayerIndex)
    {

        gameUtilities.RollTheDice();
        Debug.Log("simulateRound");
    }


	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)){
            Album newAlbum = new Album("a1");
            int numPlayers = players.Count;

            for (int i = 0; i < numPlayers; i++)
            {
                int approachedPlayerIndex = (currPlayerIndex + i) % numPlayers;
                Player currPlayer = players[approachedPlayerIndex];

                currPlayer.SpendToken(GameProperties.Instrument.GUITAR);
                currPlayer.SpendToken(GameProperties.Instrument.MARKTING);

                int albumValueIncrease = 0;
                for(int j=0; j<currPlayer.GetSkillSet().Count; j++)
                {
                    albumValueIncrease += gameUtilities.RollTheDice(6);
                }

                newAlbum.SetValue(newAlbum.GetValue() + albumValueIncrease);
            }

            int marketValue = gameUtilities.RollTheDice(40);
            if (newAlbum.GetValue() >= marketValue)
            {
                newAlbum.SetMarketingState(GameProperties.AlbumMarketingState.MEGA_HIT);
                for (int i = 0; i < numPlayers; i++)
                {
                    players[i].ReceiveMoney()
                }
            }
            else
            {

            }

        }
	}
}
