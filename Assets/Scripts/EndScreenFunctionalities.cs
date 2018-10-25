using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class EndScreenFunctionalities : MonoBehaviour
{

    public GameObject poppupPrefab;


    public Button UIRestartGameButton;
    public Button UIEndGameButton;

    public GameObject UIAlbumCollectionDisplay;
    public GameObject UIIndividualTable;
    public GameObject UIIndividualTableEntryPrefab;

    public GameObject UIVictoryOverlay;
    public GameObject UILossOverlay;
    public GameObject UIFinishedGameOverlay;

    public GameObject mainScene;

    public GameObject albumUIPrefab;


    [DllImport("__Internal")]
    private static extern void EnableCopyToClipboard(string text);

    private void RestartGame()
    {
        foreach(Album album in GameGlobals.albums)
        {
            Object.Destroy(album.GetAlbumUI());
        }
        GameSceneManager.LoadStartScene();
        Debug.Log("numGamesToSimulate: " + (GameProperties.configurableProperties.numGamesToSimulate - GameGlobals.currGameId));
    }

    private IEnumerator LoadMainScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        mainScene.SetActive(true);
        LoadEndScreenUIElements();        
    }

    private void LoadEndScreenUIElements()
    {
        PoppupScreenFunctionalities infoPoppupNeutralRef = new PoppupScreenFunctionalities(false, null, null, poppupPrefab, mainScene, this.GetComponent<MonoBehaviourFunctionalities>(), Resources.Load<Sprite>("Textures/UI/Icons/Info"), new Color(0.9f, 0.9f, 0.9f));

        GameGlobals.players.Sort(SortPlayersByMoney);
        int numPlayers = GameGlobals.players.Count;
        for (int i = 0; i < numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            GameObject newTableEntry = Object.Instantiate(UIIndividualTableEntryPrefab, UIIndividualTable.transform);
            newTableEntry.GetComponentsInChildren<Text>()[0].text = currPlayer.GetName();
            newTableEntry.GetComponentsInChildren<Text>()[1].text = currPlayer.GetMoney().ToString();
        }
        
        //Text UIEndGameButtonText = UIEndGameButton.GetComponentInChildren<Text>();
        Text UIRestartGameButtonText = UIRestartGameButton.GetComponentInChildren<Text>();
        if (GameProperties.configurableProperties.isAutomaticalBriefing)
        {
            if (GameGlobals.currGameId > GameProperties.configurableProperties.numTutorialGamesToPlay)
            {
                infoPoppupNeutralRef.DisplayPoppup("You reached the end of the experiment game. Your final task is to fill our questionnaires. To be able to do so, just copy your game code (included below). Thank you for your time!");
                //UIEndGameButton.gameObject.SetActive(true);
                //UIEndGameButton.interactable = true;
                //UIEndGameButtonText.text = "Final Notes";
                UIEndGameButton.gameObject.SetActive(false);
                UIEndGameButton.interactable = false;
                UIRestartGameButton.gameObject.SetActive(false);
                UIRestartGameButton.interactable = false;

                UIFinishedGameOverlay.SetActive(true);
            }
            else
            {
                infoPoppupNeutralRef.DisplayPoppup("You reached the end of the tutorial game. We assume that you are prepared for the experiment game. Good luck!");
                UIRestartGameButton.gameObject.SetActive(true);
                UIRestartGameButton.interactable = true;
                if (GameGlobals.currGameId == GameProperties.configurableProperties.numTutorialGamesToPlay)
                {
                    UIRestartGameButtonText.text = "Ready for experiment game";
                }
                else
                {
                    UIRestartGameButtonText.text = "Ready for another tutorial game";
                }
                UIEndGameButton.gameObject.SetActive(false);
                UIEndGameButton.interactable = false;
            }

        }
        else
        {
            UIRestartGameButton.gameObject.SetActive(true);
            UIRestartGameButton.interactable = true;
            UIRestartGameButtonText.text = "Restart Game";
            UIEndGameButton.gameObject.SetActive(false);
            UIEndGameButton.interactable = false;
        }
        


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

                currAlbumUI.transform.Translate(new Vector3(i * Screen.width*0.03f, 0, 0));
            }
            //write gameId in finish experiment text
            UIFinishedGameOverlay.transform.Find("text/gameId").GetComponent<Text>().text = GameGlobals.currSessionId;
            Button UICopyGameIdButton = UIFinishedGameOverlay.transform.Find("text/copyGameIdButton").GetComponent<Button>();

#if UNITY_EDITOR
            UICopyGameIdButton.onClick.AddListener(delegate ()
            {
                //copy Id to clipboard
                TextEditor te = new TextEditor();
                te.text = GameGlobals.currSessionId;
                te.SelectAll();
                te.Copy();
            });
#elif UNITY_WEBGL
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(UICopyGameIdButton.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            UICopyGameIdButton.onClick.AddListener(delegate ()
            {
                //enable click on button for clipboard copying
                EnableCopyToClipboard(GameGlobals.currSessionId);
            });
#endif

        }

        UIRestartGameButton.onClick.AddListener(delegate () {
            RestartGame();
        });

       // UIEndGameButton.onClick.AddListener(delegate () {
            //mainScene.SetActive(false);

           
            UIFinishedGameOverlay.SetActive(true);
        //});
    }

    //in order to sort the players list by money earned
    public int SortPlayersByMoney(Player p1, Player p2)
    {
        return -1*(p1.GetMoney()).CompareTo(p2.GetMoney());
    }


    // Use this for initialization
    void Start()
    {
        ////mock
        //GameGlobals.currSessionId = System.DateTime.Now.ToString("yyyy/MM/dd/HH-mm-ss");
        //GameGlobals.gameLogManager = new DebugLogManager();
        //GameGlobals.gameLogManager.InitLogs();
        //GameGlobals.albums = new System.Collections.Generic.List<Album>(GameProperties.numberOfAlbumsPerGame);
        //Album newAlbum = new Album("1", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("2", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("3", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("4", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("5", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("6", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("7", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //GameGlobals.players = new System.Collections.Generic.List<Player>(GameProperties.numberOfPlayersPerGame);
        //GameGlobals.players.Add(new UIPlayer(0, "PL1"));
        //GameGlobals.players.Add(new UIPlayer(1, "PL2"));
        //GameGlobals.players.Add(new UIPlayer(2, "PL3"));
        //GameGlobals.currGameState = GameProperties.GameState.VICTORY;
        //GameGlobals.currGameId = 2;

        GameGlobals.gameLogManager.UpdateGameResultInLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameState.ToString());


        UIVictoryOverlay.SetActive(false);
        UILossOverlay.SetActive(false);
        UIFinishedGameOverlay.SetActive(false);
        mainScene.SetActive(false);

        int numAlbumsPlayed = GameGlobals.albums.Count;
        for (int i = 0; i < numAlbumsPlayed; i++)
        {
            GameGlobals.albums[i].GetAlbumUI().SetActive(false);
        }


        if (GameGlobals.currGameState == GameProperties.GameState.VICTORY)
        {
            UIVictoryOverlay.SetActive(true);
        }
        else if (GameGlobals.currGameState == GameProperties.GameState.LOSS)
        {
            UILossOverlay.SetActive(true);

        }
        else
        {
            Debug.Log("[ERROR]: Game state returned NON FINISHED on game end!");
            return;
        }



        //GameGlobals.gameLogManager.WriteGameToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameProperties.currGameParameterization.id, GameGlobals.currGameState.ToString());
        GameGlobals.gameLogManager.EndLogs();



        if (GameProperties.configurableProperties.isSimulation)
        {
            if (GameGlobals.currGameId < GameProperties.configurableProperties.numGamesToSimulate)
            {
                RestartGame();
            }
            return;
        }
        else
        {

            StartCoroutine(LoadMainScreenAfterDelay(5.0f));
        }
    }


    
}
    

