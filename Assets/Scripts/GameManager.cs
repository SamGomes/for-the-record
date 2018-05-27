using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private int numRounds;
    private List<GameRound> gameRounds;
    private List<Album> albums;

    private List<Player> players;

	// Use this for initialization
	void Start () {
        numRounds = 5;

        albums = new List<Album>(numRounds);

        players = new List<Player>();
        players.Add(new Player("John"));
        players.Add(new Player("Sara"));
        players.Add(new Player("Bob"));
	}
	
    void SimulateRound()
    {
        Debug.Log("simulateRound");
    }


	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Space)){
            SimulateRound();
        }
	}
}
