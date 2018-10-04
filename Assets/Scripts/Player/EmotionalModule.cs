using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AssetManagerPackage;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using WellFormedNames;
using System.Threading;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class EmotionalModule : MonoBehaviour
{
    private float speechBalloonDelayInSeconds;

    private RolePlayCharacterAsset rpc;
    private Thread rpcThread;
    private bool isStopped;

    public int DicesValue { get; internal set; }
    public int NumDices { get; internal set; }


    public bool Speaks { get; internal set; }
    private UIPlayer invoker; //when no speech the object is passed so that text is displayed
    private GameObject speechBalloon;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    { 
        isStopped = false;

        string rpcPath = GameGlobals.FAtiMAIat.GetAllCharacterSources().FirstOrDefault().Source;
        Application.ExternalEval("console.log(\"rpcPath: " + rpcPath + "\")");
        rpc = RolePlayCharacterAsset.LoadFromFile(rpcPath);


        rpc.LoadAssociatedAssets();
        GameGlobals.FAtiMAIat.BindToRegistry(rpc.DynamicPropertiesRegistry);
        rpcThread = new Thread(UpdateCoroutine);
        rpcThread.Start();

        speechBalloonDelayInSeconds = 3.0f;
    }

    public void ReceiveInvoker(UIPlayer invoker)
    {
        this.invoker = invoker;
        speechBalloon = invoker.GetSpeechBaloonUI();
    }

    public void Perceive(Name[] events)
    {
        rpc.Perceive(events);
    }

    public IEnumerator DisplaySpeechBalloonForAWhile(string message, float delay)
    {
        speechBalloon.GetComponentInChildren<Text>().text = message;
        speechBalloon.SetActive(true);
        yield return new WaitForSeconds(delay);
        if(speechBalloon.GetComponentInChildren<Text>().text == message) //to compensate if the balloon is already spawned
        {
            speechBalloon.SetActive(false);
        }
    }

    public string StripSpeechSentence(string rawMessage)
    {
        var strippedDialog = rawMessage;
        strippedDialog = strippedDialog.Replace("|dicesValue|", DicesValue.ToString());
        strippedDialog = strippedDialog.Replace("|numDices|", NumDices.ToString());
        strippedDialog = strippedDialog.Replace("|instrument|", invoker.GetPreferredInstrument().ToString().ToLower());
        strippedDialog = Regex.Replace(strippedDialog, "<.*?> | <.*?>", "");
        return strippedDialog;
    }

    public void Decide()
    {
        IEnumerable<ActionLibrary.IAction> possibleActions = rpc.Decide();
        ActionLibrary.IAction chosenAction = possibleActions.FirstOrDefault();


        if (chosenAction == null)
        {
            Console.WriteLine("No action");
            //saveToFile();
            return;
        }
        else
        {
            //saveToFile();


            switch (chosenAction.Key.ToString())
            {
                case "Speak":

                    Name currentState = chosenAction.Parameters[0];
                    Name nextState = chosenAction.Parameters[1];
                    Name meaning = chosenAction.Parameters[2];
                    Name style = chosenAction.Parameters[3];

                    var possibleDialogs = GameGlobals.FAtiMAIat.GetDialogueActions(currentState, nextState, meaning, style);
                    int randomUttIndex = UnityEngine.Random.Range(0, possibleDialogs.Count());
                    var dialog = possibleDialogs[randomUttIndex].Utterance;

                    StartCoroutine(DisplaySpeechBalloonForAWhile(invoker.GetName() + ": \"" + StripSpeechSentence(dialog) + "\"", speechBalloonDelayInSeconds));

                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateCoroutine()
    {
        string currentBelief = rpc.GetBeliefValue("State(Game)");

        while (currentBelief != "Game(End)" && !isStopped)
        {
            rpc.Update();
            Thread.Sleep(100);
        }
    }

    void OnDestroy()
    {
        if (!isStopped)
        {
            rpcThread.Abort();
            isStopped = true;
        }
    }

    void OnApplicationQuit()
    {
        if (!isStopped)
        {
            rpcThread.Abort();
            isStopped = true;
        }
    }

    public void FlushUtterance(string text)
    {
        StartCoroutine(DisplaySpeechBalloonForAWhile(StripSpeechSentence(text), 2.0f));
    }

    public void GazeAt(string target)
    {
        Debug.Log("agent did not gaze.");
    }
}

