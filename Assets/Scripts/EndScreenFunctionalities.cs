using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenFunctionalities : MonoBehaviour
{

    public Button UIRestartGameButton;
    public GameObject UIAlbumCollectionDisplay;

    // Use this for initialization
    void Start()
    {
        UIRestartGameButton.onClick.AddListener(delegate () { GameSceneManager.LoadMainScene(); });
        foreach(Album album in GameManager.albums)
        {
            album.GetAlbumUI().gameObject.transform.parent = UIRestartGameButton.gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

