using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenFunctionalities : MonoBehaviour {

    public Button UIStartGameButton;

	// Use this for initialization
	void Start () {
        UIStartGameButton.onClick.AddListener(delegate () { GameSceneManager.LoadMainScene(); });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
