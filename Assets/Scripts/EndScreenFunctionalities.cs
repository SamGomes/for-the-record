using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenFunctionalities : MonoBehaviour
{

    public Button UIRestartGameButton;
    public GameObject UIAlbumCollectionDisplay;

    public GameObject UIVictoryOverlay;
    public GameObject UILossOverlay;



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
            int numAlbumsPlayed = GameGlobals.albums.Count;
            for (int i = 0; i < 20; i++)
            {
                if (i == numAlbumsPlayed)
                {
                    break;
                }

                Album currAlbum = GameGlobals.albums[i];
                GameObject currAlbumUI = currAlbum.GetAlbumUI();
                currAlbumUI.SetActive(true);

                Animator animator = currAlbumUI.GetComponentInChildren<Animator>();
                animator.Play(0);
                animator.speed = (i * 0.1f < animator.speed) ? animator.speed - i * 0.1f : animator.speed;

                currAlbumUI.transform.SetParent(UIAlbumCollectionDisplay.transform);
                currAlbumUI.transform.localPosition = new Vector3(0, 0, 0);

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
        StartCoroutine(HideAfterDelay(UILossOverlay, 5.0f));

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
}
    

