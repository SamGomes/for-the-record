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

    public void Perceive (Name[] events)
    {
        rpc.Perceive(events);
    }

    public void Decide ()
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

    public void FlushRobotUtterance(string text)
    {
        thalamusConnector.PerformUtterance(text);
    }
}

public class RoboticPlayerCoopStrategy : AIPlayerCoopStrategy
{
    private EmotionalRoboticPlayer robot;

    public RoboticPlayerCoopStrategy(int id, string name) : base(name)
    {
        this.id = id;
        GameObject erp = new GameObject("EmotionalRoboticPlayer");
        robot = erp.AddComponent<EmotionalRoboticPlayer>();
        robot.InitThalamusConnectorOnPort(7000, name);
    }

    public void FlushRobotUtterance(string text)
    {
        robot.FlushRobotUtterance(text);
    }

    public override void ChoosePreferredInstrument(Album currAlbum)
    {
        robot.Perceive(new Name[] {
            EventHelper.PropertyChange("Character(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "ChoosePreferredInstrument", name) });
        base.ChoosePreferredInstrument(currAlbum);
        robot.Decide();
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
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (currentPlayer == this)
        {
            LevelUp(currAlbum);
        }
        else
        {
            if (currSpeakingPlayerId == id && currentPlayer.GetName() == "Player")
            {
                Debug.Log(name + ": É a vez do " + currentPlayer.GetName());
                robot.Perceive(new Name[] {
                    EventHelper.PropertyChange("CurrentPlayer(Name)", currentPlayer.GetName(), name),
                    EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
                robot.Decide();
            }
            else
            {
                Debug.Log(name + " gazes at " + currentPlayer.GetName());
            }
        }
    }
    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        robot.Perceive(new Name[] {
            EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
        robot.Decide();
    }
    public override void PlayForInstrument(Album currAlbum)
    {
        robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "PlayForInstrument", name) });
        base.PlayForInstrument(currAlbum);
        robot.Decide();
    }
    public override void LastDecisionsPhase(Album currAlbum)
    {
        base.LastDecisionsPhase(currAlbum);

        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name),
            EventHelper.PropertyChange("Album(Result)", "Success", name) });
        }
        else
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name),
            EventHelper.PropertyChange("Album(Result)", "Fail", name) });
        }
        robot.Decide();
    }

    protected override void InformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue)
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();
        if (currSpeakingPlayerId == id)
        {
            float luckFactor = (float)obtainedValue / (float)maxValue;

            if (luckFactor > 0.7)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Luck", invoker.GetName()) });
            }
            else if (luckFactor < 0.2)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "BadLuck", invoker.GetName()) });
            }
            else
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Neutral", invoker.GetName()) });
            }
            robot.Decide();
        }
    }
    protected override void InformAlbumResultActions(int albumValue, int marketValue)
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (albumValue >= marketValue)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Success", name) });
        }
        else
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Fail", name) });
        }

        if (currSpeakingPlayerId == id)
        {
            robot.Decide();
        }
        else
        {
            //gaze
        }
    }
    protected override void InformGameResultActions(GameProperties.GameState state)
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();
        if (state == GameProperties.GameState.VICTORY)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Victory", name) });
        }
        else if (state == GameProperties.GameState.LOSS)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Loss", name) });
        }

        if (currSpeakingPlayerId == id)
        {
            robot.Decide();
        }
        else
        {
            //gaze
        }
    }
    protected override void InformNewAlbumActions()
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();
        if (currSpeakingPlayerId == id)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "NewAlbum", name) });

            if (GameGlobals.albums.Count() == 0)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Career)", "Beginning", name) });
            }
            else if (GameGlobals.albums.Count() == 4)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Career)", "End", name) });
            }
            else
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Career)", "Middle", name) });
            }
            robot.Decide();
        }
    }
}

public class RoboticPlayerGreedyStrategy : AIPlayerGreedyStrategy
{
    private EmotionalRoboticPlayer robot;

    public RoboticPlayerGreedyStrategy(int id, string name) : base(name)
    {
        this.id = id;
        GameObject erp = new GameObject("EmotionalRoboticPlayer");
        robot = erp.AddComponent<EmotionalRoboticPlayer>();
        robot.InitThalamusConnectorOnPort(7002, name);
    }

    public void FlushRobotUtterance(string text)
    {
        robot.FlushRobotUtterance(text);
    }

    public override void ChoosePreferredInstrument(Album currAlbum)
    {
        robot.Perceive(new Name[] {
            EventHelper.PropertyChange("Character(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "ChoosePreferredInstrument", name) });
        base.ChoosePreferredInstrument(currAlbum);
        robot.Decide();
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
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (currentPlayer == this)
        {
            LevelUp(currAlbum);
        }
        else
        {
            if (currSpeakingPlayerId == id && currentPlayer.GetName() == "Player")
            {
                Debug.Log(name + ": É a vez do " + currentPlayer.GetName());
                robot.Perceive(new Name[] {
                    EventHelper.PropertyChange("CurrentPlayer(Name)", currentPlayer.GetName(), name),
                    EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
                robot.Decide();
            }
            else
            {
                Debug.Log(name + " gazes at " + currentPlayer.GetName());
            }
        }
    }
    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        robot.Perceive(new Name[] {
            EventHelper.PropertyChange("CurrentPlayer(Name)", name, name),
            EventHelper.PropertyChange("State(Game)", "LevelUp", name) });
        robot.Decide();
    }
    public override void PlayForInstrument(Album currAlbum)
    {
        robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "PlayForInstrument", name) });
        base.PlayForInstrument(currAlbum);
        robot.Decide();
    }
    public override void LastDecisionsPhase(Album currAlbum)
    {
        base.LastDecisionsPhase(currAlbum);

        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name),
            EventHelper.PropertyChange("Album(Result)", "Success", name) });
        }
        else
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "LastDecisionsPhase", name),
            EventHelper.PropertyChange("Album(Result)", "Fail", name) });
        }
        robot.Decide();
    }

    protected override void InformRollDicesValueActions(Player invoker, int maxValue, int obtainedValue)
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (currSpeakingPlayerId == id)
        {
            float luckFactor = (float)obtainedValue / (float)maxValue;

            if (luckFactor > 0.7)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Luck", invoker.GetName()) });
            }
            else if (luckFactor < 0.2)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "BadLuck", invoker.GetName()) });
            }
            else
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollInstrumentDice", name),
            EventHelper.PropertyChange("Roll(InstrumentDice)", "Neutral", invoker.GetName()) });
            }
            robot.Decide();
        }
    }
    protected override void InformAlbumResultActions(int albumValue, int marketValue)
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (albumValue >= marketValue)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Success", name) });
        }
        else
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "RollMarketDice", name),
            EventHelper.PropertyChange("Roll(MarketDice)", "Fail", name) });
        }

        if (currSpeakingPlayerId == id)
        {
            robot.Decide();
        }
        else
        {
            //gaze
        }
    }
    protected override void InformGameResultActions(GameProperties.GameState state)
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();

        if (state == GameProperties.GameState.VICTORY)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Victory", name) });
        }
        else if (state == GameProperties.GameState.LOSS)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "GameEnd", name),
            EventHelper.PropertyChange("Game(Result)", "Loss", name) });
        }

        if (currSpeakingPlayerId == id)
        {
            robot.Decide();
        }
        else
        {
            //gaze
        }
    }
    protected override void InformNewAlbumActions()
    {
        int currSpeakingPlayerId = gameManagerRef.GetCurrSpeakingPlayerId();
        if (currSpeakingPlayerId == id)
        {
            robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Game)", "NewAlbum", name) });

            if (GameGlobals.albums.Count() == 0)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Career)", "Beginning", name) });
            }
            else if (GameGlobals.albums.Count() == 4)
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Career)", "End", name) });
            }
            else
            {
                robot.Perceive(new Name[] {
            EventHelper.PropertyChange("State(Career)", "Middle", name) });
            }
            robot.Decide();
        }
    }
}