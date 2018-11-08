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
            if (GameGlobals.currGameId >= GameProperties.configurableProperties.numSessionGames)
            {
                //infoPoppupNeutralRef.DisplayPoppup("You reached the end of the experiment. You should now fill in the first questionnaire and you need to memorize the following code and also your score.");
                infoPoppupNeutralRef.DisplayPoppup("You reached the end of the second game. You should memorize both your score and the following code, and fill the second questionnaire to finish the experiment.");
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
                //infoPoppupNeutralRef.DisplayPoppup("You reached the end of one of the games to play in this session. We assume that you are prepared for the experiment game. Good luck!");
                infoPoppupNeutralRef.DisplayPoppup("You reached the end of the first game. You should memorize your score and fill the first questionnaire. Please, do not move to next game until the questionnaire mentions you to do so.");
                UIRestartGameButton.gameObject.SetActive(true);
                UIRestartGameButton.interactable = true;
                UIRestartGameButtonText.text = "Next game";
                
                UIEndGameButton.gameObject.SetActive(false);
                UIEndGameButton.interactable = false;

                UIFinishedGameOverlay.SetActive(false);
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
            UIFinishedGameOverlay.transform.Find("text/WebGLInputField/Text").GetComponent<Text>().text = GameGlobals.currSessionId;
            Button UICopyGameIdButton = UIFinishedGameOverlay.transform.Find("text/copyGameIdButton").GetComponent<Button>();
            Button UINextGameButton = UIFinishedGameOverlay.transform.Find("text/nextGameButton").GetComponent<Button>();

            UINextGameButton.onClick.AddListener(delegate () {
                RestartGame();
            });

        }

        UIRestartGameButton.onClick.AddListener(delegate () {
            RestartGame();
        });
       
    }

    //in order to sort the players list by money earned
    public int SortPlayersByMoney(Player p1, Player p2)
    {
        return -1*(p1.GetMoney()).CompareTo(p2.GetMoney());
    }


    // Use this for initialization
    void Start()
    {
        //mock
        //GameProperties.configurableProperties = new DynamicallyConfigurableGameProperties();
        //GameProperties.configurableProperties.numSessionGames = 3;
        //GameProperties.configurableProperties.isAutomaticalBriefing = true;
        //GameGlobals.currSessionId = System.DateTime.Now.ToString("yyyy/MM/dd/HH-mm-ss");
        //GameGlobals.gameLogManager = new DebugLogManager();
        //GameGlobals.gameLogManager.InitLogs();
        //GameGlobals.albums = new List<Album>(5);
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
        //GameGlobals.players = new List<Player>(5);
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
    

