using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AssetManagerPackage;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using WellFormedNames;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class EmotionalModule : MonoBehaviour
{
    private float speechBalloonDelayInSeconds;

    private RolePlayCharacterAsset rpc;
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
        //Application.ExternalEval("console.log(\"rpcPath: " + rpcPath + "\")");
        rpc = RolePlayCharacterAsset.LoadFromFile(rpcPath);


        rpc.LoadAssociatedAssets();
        GameGlobals.FAtiMAIat.BindToRegistry(rpc.DynamicPropertiesRegistry);

        //start update thread
        StartCoroutine(UpdateCoroutine());

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

    public string StripSpeechSentence(string rawMessage)
    {
        Debug.Log("StripSpeechSentence - " + invoker.GetId() + "  " + invoker.GetPreferredInstrument());
        var strippedDialog = rawMessage;
        strippedDialog = strippedDialog.Replace("|dicesValue|", DicesValue.ToString());
        strippedDialog = strippedDialog.Replace("|numDices|", NumDices.ToString());
        strippedDialog = strippedDialog.Replace("|instrument|", invoker.GetPreferredInstrument().ToString().ToLower());
        strippedDialog = strippedDialog.Replace("|musicianRole|", Enum.GetName(typeof(GameProperties.MusicianRole), invoker.GetPreferredInstrument()).ToLower()); 
         strippedDialog = Regex.Replace(strippedDialog, @"<.*?>\s+|\s+<.*?>|\s+<.*?>\s+", "");
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

                    speak(invoker.GetName() + ": \"" + StripSpeechSentence(dialog) + "\"");

                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator UpdateCoroutine()
    {
        string currentBelief = rpc.GetBeliefValue("State(Game)");

        while (currentBelief != "Game(End)" && !isStopped)
        {
            rpc.Update();
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    void OnDestroy()
    {
        if (!isStopped)
        {
            StopCoroutine(UpdateCoroutine());
            isStopped = true;
        }
    }

    void OnApplicationQuit()
    {
        if (!isStopped)
        {
            StopCoroutine(UpdateCoroutine());
            isStopped = true;
        }
    }

    public void FlushUtterance(string text)
    {
        speak(StripSpeechSentence(text));
    }

    public void speak(string text)
    {
        if (!Speaks)
        {
            return;
        }
        StartCoroutine(invoker.DisplaySpeechBalloonForAWhile(text, this.speechBalloonDelayInSeconds));
    }

    public void GazeAt(string target)
    {
        Debug.Log("agent did not gaze.");
    }
}

