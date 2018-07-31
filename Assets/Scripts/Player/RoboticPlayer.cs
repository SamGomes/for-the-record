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

                    var possibleDialogs = iat.GetDialogueActions(currentState, nextState, meaning, style);
                    int randomUttIndex = UnityEngine.Random.Range(0, possibleDialogs.Count());
                    var dialog = possibleDialogs[randomUttIndex].Utterance;
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
    private int id;

    public RoboticPlayerCoopStrategy(int id, string name) : base(name)
    {
        this.id = id;
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
    protected override void ChoosePreferredInstrumentActions(Album currAlbum)
    {
        GameProperties.Instrument preferredIntrument = GameProperties.Instrument.BASS;

        foreach (Player player in GameGlobals.players)
        {
            if (player == this)
            {
                continue;
            }
            if (player.GetPreferredInstrument() == preferredIntrument)
            {
                base.ChoosePreferredInstrumentActions(currAlbum);
                return;
            }
        }
        ChangePreferredInstrument(preferredIntrument);
    }
    public override void LevelUpRequest(Album currAlbum)
    {
        Player currentPlayer = gameManagerRef.GetCurrentPlayer();
        base.LevelUp(currAlbum);
        if (id == gameManagerRef.GetCurrSpeakingPlayerId())
        {
            Debug.Log(name + ": É a vez do " + currentPlayer.GetName());
            robot.perceive(new Name[] {
                EventHelper.PropertyChange("CurrentPlayer(Name)", currentPlayer.GetName(), name),
                EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
            robot.decide();
        }
        else
        {
            Debug.Log(name + " gazes at " + currentPlayer.GetName());
        }
        
    }
    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
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

    protected override void InformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue, int speakingRobotId)
    {
        if (speakingRobotId == id)
        {
            float luckFactor = (float)obtainedValue / (float)maxValue;

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
    protected override void InformAlbumResultActions(int albumValue, int marketValue, int speakingRobotId)
    {
        if (albumValue >= marketValue)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Success", name) });
        }
        else
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Fail", name) });
        }

        if (speakingRobotId == id)
        {
            robot.decide();
        }
        else
        {
            //gaze
        }
    }
    protected override void InformGameResultActions(GameProperties.GameState state, int speakingRobotId)
    {
        if (state == GameProperties.GameState.VICTORY)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Victory", name) });
        }
        else if (state == GameProperties.GameState.LOSS)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Loss", name) });
        }

        if (speakingRobotId == id)
        {
            robot.decide();
        }
        else
        {
            //gaze
        }
    }
}

public class RoboticPlayerGreedyStrategy : AIPlayerGreedyStrategy
{
    private EmotionalRoboticPlayer robot;
    private int id;

    public RoboticPlayerGreedyStrategy(int id, string name) : base(name)
    {
        this.id = id;
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
    protected override void ChoosePreferredInstrumentActions(Album currAlbum)
    {
        GameProperties.Instrument preferredIntrument = GameProperties.Instrument.VOCALS;

        foreach (Player player in GameGlobals.players)
        {
            if (player == this)
            {
                continue;
            }
            if (player.GetPreferredInstrument() == preferredIntrument)
            {
                base.ChoosePreferredInstrumentActions(currAlbum);
                return;
            }
        }
        ChangePreferredInstrument(preferredIntrument);
    }
    public override void LevelUpRequest(Album currAlbum)
    {
        Player currentPlayer = gameManagerRef.GetCurrentPlayer();
        base.LevelUp(currAlbum);
        if (id == gameManagerRef.GetCurrSpeakingPlayerId())
        {
            Debug.Log(name + ": É a vez do " + currentPlayer.GetName());
            robot.perceive(new Name[] {
                EventHelper.PropertyChange("CurrentPlayer(Name)", currentPlayer.GetName(), name),
                EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
            robot.decide();
        }
        else
        {
            Debug.Log(name + " gazes at " + currentPlayer.GetName());
        }
    }
    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        robot.perceive(new Name[] {
            EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
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

    protected override void InformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue, int speakingRobotId)
    {
        if (speakingRobotId == id)
        {
            float luckFactor = (float)obtainedValue / (float)maxValue;

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
    protected override void InformAlbumResultActions(int albumValue, int marketValue, int speakingRobotId)
    {
        if (albumValue >= marketValue)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Success", name) });
        }
        else
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Fail", name) });
        }

        if (speakingRobotId == id)
        {
            robot.decide();
        }
        else
        {
            //gaze
        }
    }
    protected override void InformGameResultActions(GameProperties.GameState state, int speakingRobotId)
    {
        if (state == GameProperties.GameState.VICTORY)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Victory", name) });
        }
        else if (state == GameProperties.GameState.LOSS)
        {
            robot.perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Loss", name) });
        }

        if (speakingRobotId == id)
        {
            robot.decide();
        }
        else
        {
            //gaze
        }
    }
}
