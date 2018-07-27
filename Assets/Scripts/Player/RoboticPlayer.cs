using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using WellFormedNames;
using System.Threading;
using System.Collections;
using UnityEngine.SceneManagement;

public class EmotionalRoboticPlayer : MonoBehaviour
{
    private ThalamusConnector thalamusConnector = null;

    private IntegratedAuthoringToolAsset iat;
    private RolePlayCharacterAsset rpc;
    private Thread rpcThread;
    private bool isStopped;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        isStopped = false;
        iat = IntegratedAuthoringToolAsset.LoadFromFile("Assets/Scenarios/ForTheRecord.iat");
        rpc = RolePlayCharacterAsset.LoadFromFile(iat.GetAllCharacterSources().FirstOrDefault().Source);
        rpc.LoadAssociatedAssets();
        iat.BindToRegistry(rpc.DynamicPropertiesRegistry);
        rpcThread = new Thread(UpdateCoroutine);
        rpcThread.Start();
    }

    public void InitThalamusConnectorOnPort(int port, string name)
    {
        thalamusConnector = new ThalamusConnector(port);
        this.name = name;
    }

    public void perceive (Name[] events)
    {
        rpc.Perceive(events);
    }

    public void decide ()
    {
        IEnumerable<ActionLibrary.IAction> possibleActions = rpc.Decide();

        if (possibleActions == null)
        {
            Console.WriteLine("No action");
            //saveToFile();
            return;
        }
        else
        {
            ActionLibrary.IAction chosenAction = possibleActions.FirstOrDefault();
            //saveToFile();


            switch (chosenAction.Key.ToString())
            {
                case "Speak":
                    Name currentState = chosenAction.Parameters[0];
                    Name nextState = chosenAction.Parameters[1];
                    Name meaning = chosenAction.Parameters[2];
                    Name style = chosenAction.Parameters[3];

                    var possibleDialogs = iat.GetDialogueActions(currentState, nextState, meaning, style);
                    var dialog = possibleDialogs[0].Utterance;
                    if (thalamusConnector != null)
                    {
                        thalamusConnector.PerformUtterance(dialog);
                        Debug.Log(name + " is saying " + dialog);
                    }
                    else
                    {
                        Debug.Log("ERROR: ThalamusConnector not defined yet.");
                    }
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
}

public class RoboticPlayerCoopStrategy : AIPlayerCoopStrategy
{
    private EmotionalRoboticPlayer robot;

    public RoboticPlayerCoopStrategy(GameObject parentGameObject, string name) : base(name)
    {
        GameObject erp = new GameObject("EmotionalRoboticPlayer");
        robot = erp.AddComponent<EmotionalRoboticPlayer>();
        robot.InitThalamusConnectorOnPort(7000, name);
    }

    public override void ChoosePreferredInstrument(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("Character(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "ChoosePreferredInstrument", name) });
        base.ChoosePreferredInstrument(currAlbum);
        robot.decide();
    }

    public override void LevelUp(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
        base.LevelUp(currAlbum);
        robot.decide();
    }

    public override void PlayForInstrument(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "PlayForInstrument", name) });
        base.PlayForInstrument(currAlbum);
        robot.decide();
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name) });
        base.LastDecisionsPhase(currAlbum);
        robot.decide();
    }

    public override void InformRollDicesValue(Player invoker, int maxValue, int obtainedValue)
    {
        float luckFactor = (float) obtainedValue / (float) maxValue;

        if (luckFactor > 0.7)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Luck", invoker.GetName()) });
        }
        else if (luckFactor > 0.35)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Neutral", invoker.GetName()) });
        }
        else
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "BadLuck", invoker.GetName()) });
        }
        robot.decide();
    }
}

public class RoboticPlayerGreedyStrategy : AIPlayerGreedyStrategy
{
    private EmotionalRoboticPlayer robot;

    public RoboticPlayerGreedyStrategy(string name) : base(name)
    {
        GameObject erp = new GameObject("EmotionalRoboticPlayer");
        robot = erp.AddComponent<EmotionalRoboticPlayer>();
        robot.InitThalamusConnectorOnPort(7002, name);
    }

    public override void ChoosePreferredInstrument(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("Character(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "ChoosePreferredInstrument", name) });
        base.ChoosePreferredInstrument(currAlbum);
        robot.decide();
    }

    public override void LevelUp(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
        base.LevelUp(currAlbum);
        robot.decide();
    }

    public override void PlayForInstrument(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "PlayForInstrument", name) });
        base.PlayForInstrument(currAlbum);
        robot.decide();
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name) });
        base.LastDecisionsPhase(currAlbum);
        robot.decide();
    }
}
