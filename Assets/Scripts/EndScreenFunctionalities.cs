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
        if (!(GameProperties.isSimulation && GameProperties.numGamesToSimulate > 0))
        {
            UIRestartGameButton.onClick.AddListener(delegate () { GameSceneManager.LoadMainScene(); });
            //foreach (Album album in GameManager.albums)
            //{
            //    album.GetAlbumUI().gameObject.transform.parent = UIRestartGameButton.gameObject.transform;
            //}
        }
        else
        {
            GameSceneManager.LoadMainScene();
            FileManager.WriteGameResultsToLog();
            Debug.Log("numGamesToSimulate: " + GameProperties.numGamesToSimulate);

            if (GameProperties.numGamesToSimulate==1)
            {
                FileManager.CloseWriter();
            }
            GameProperties.numGamesToSimulate--;
        }
    }
}
    

