using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndScreenFunctionalities : MonoBehaviour
{

    public Button UIRestartGameButton;
    public GameObject UIAlbumCollectionDisplay;

    public GameObject UIVictoryOverlay;
    public GameObject UILossOverlay;

    public GameObject albumUIPrefab;

    private void RestartGame()
    {
        GameSceneManager.LoadStartScene();
        Debug.Log("numGamesToSimulate: " + GameProperties.numGamesToSimulate);

        GameProperties.numGamesToSimulate--;
        if (GameProperties.numGamesToSimulate == 0)
        {
            FileManager.CloseWriter();
        }
    }

    private IEnumerator HideAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        if (!GameProperties.isSimulation)
        {
            LoadEndScreenUIElements();
        }
    }

    private void LoadEndScreenUIElements()
    {
        if (GameGlobals.albums != null)
        {
            //disable rendering of some albums and selecting others
            int numAlbumsPlayed = GameGlobals.albums.Count;
            for (int i = 0; i < numAlbumsPlayed; i++)
            {
                Album currAlbum = GameGlobals.albums[i];
                GameObject currAlbumUI = currAlbum.GetAlbumUI();
                if ((numAlbumsPlayed - i) > 5) //maximum of 5 last albums rendered albums
                {
                    currAlbumUI.SetActive(false);
                    continue;
                }
                currAlbumUI.SetActive(true);

                Animator animator = currAlbumUI.GetComponentInChildren<Animator>();
                animator.Rebind();
                animator.Play(0);
                animator.speed = (i * 0.1f < animator.speed) ? animator.speed - i * 0.1f : animator.speed;

                currAlbumUI.transform.SetParent(UIAlbumCollectionDisplay.transform);
                currAlbumUI.transform.localPosition = new Vector3(0, 0, 0);
                currAlbumUI.transform.localScale = new Vector3(1, 1, 1);

                currAlbumUI.transform.Translate(new Vector3(i * 50.0f, 0, 0));
            }
        }

        UIRestartGameButton.onClick.AddListener(delegate () {
            RestartGame();
        });

    }

    // Use this for initialization
    void Start()
    {
        //mock
        GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        Album newAlbum = new Album("1", albumUIPrefab);
        GameGlobals.albums.Add(newAlbum);
        newAlbum = new Album("2", albumUIPrefab);
        GameGlobals.albums.Add(newAlbum);
        newAlbum = new Album("3", albumUIPrefab);
        GameGlobals.albums.Add(newAlbum);
        newAlbum = new Album("4", albumUIPrefab);
        GameGlobals.albums.Add(newAlbum);
        newAlbum = new Album("5", albumUIPrefab);
        GameGlobals.albums.Add(newAlbum);
        newAlbum = new Album("6", albumUIPrefab);
        GameGlobals.albums.Add(newAlbum);
        newAlbum = new Album("7", albumUIPrefab);
        GameGlobals.albums.Add(newAlbum);


        UIVictoryOverlay.SetActive(false);
        UILossOverlay.SetActive(false);

        if (GameGlobals.currGameState == GameProperties.GameState.VICTORY)
        {
            UIVictoryOverlay.SetActive(true);
            StartCoroutine(HideAfterDelay(UIVictoryOverlay, 5.0f));
        }
        else if(GameGlobals.currGameState == GameProperties.GameState.LOSS)
        {
            UILossOverlay.SetActive(true);
            StartCoroutine(HideAfterDelay(UILossOverlay, 5.0f));

        }
        StartCoroutine(HideAfterDelay(UILossOverlay, 0.0f));

        //else
        //{
        //    Debug.Log("[ERROR]: Game state returned NON FINISHED on game end!");
        //    return;
        //}

        if (GameProperties.isSimulation)
        {
            RestartGame();
        }

    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null &&
     EventSystem.current.currentSelectedGameObject.GetComponent<Album>() != null)
        {
            EventSystem.current.currentSelectedGameObject.transform.SetAsLastSibling();
        }
    }
}
    

