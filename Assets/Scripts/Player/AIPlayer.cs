using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : UIPlayer
{
    public AIPlayer(string name) : base(name) {}

    //public new void InitUI(GameObject playerUIPrefab, GameObject canvas, WarningScreenFunctionalities warningScreenRef)
    //{
    //    base.InitUI(playerUIPrefab, canvas, warningScreenRef);
    //    Button[] buttons = this.GetPlayerUI().GetComponentsInChildren<Button>();
    //    foreach(Button button in buttons)
    //    {
    //        button.interactable = false;
    //    }
    //}

   

    //defect
    //protected void DoNotPlayForInstruments()
    //{
    //    ChangeDiceRollInstrument(GameProperties.Instrument.NONE);
    //    if (!GameProperties.isSimulation)
    //    {
    //        base.PlayForInstrument();
    //        playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(2.0f));
    //    }
    //    else
    //    {
    //        SendPlayForInstrumentResponse();
    //    }
    //}

    //predicting hri-s
    public IEnumerator ThinkBeforeLevelingUp(float delay)
    {
        yield return new WaitForSeconds(delay);
        //yield return null;
        SendLevelUpResponse();
    }
    public IEnumerator ThinkBeforePlayForInstrument(float delay)
    {
        yield return new WaitForSeconds(delay);
        //yield return null;
        SendPlayForInstrumentResponse();
    }
    public IEnumerator ThinkBeforeLastDecisionPhase(int condition, float delay)
    {
        yield return new WaitForSeconds(delay);
        //yield return null;
        SendLastDecisionsPhaseResponse(condition);
    }
}

public class AIPlayerSimple : AIPlayer
{
    public AIPlayerSimple(string name) : base(name)
    {
    }

    public override void LevelUp()
    {
        SpendToken(GameProperties.Instrument.GUITAR);
        SpendToken(GameProperties.Instrument.GUITAR);

        if (!GameProperties.isSimulation)
        {
            base.LevelUp();
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(2.0f));
        }
        else
        {
            SendLevelUpResponse();
        }

    }

    public override void PlayForInstrument()
    {
        ChangeDiceRollInstrument(GameProperties.Instrument.GUITAR);

        if (!GameProperties.isSimulation)
        {
            base.LevelUp();
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(2.0f));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 0;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }

        if (!GameProperties.isSimulation)
        {
            base.LastDecisionsPhase(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(condition, 2.0f));
        }
        else
        {
            SendLastDecisionsPhaseResponse(condition);
        }
    }

}

public class AIPlayerCoopStrategy : AIPlayer
{
    GameProperties.Instrument preferredInstrument;

    public AIPlayerCoopStrategy(string name) : base(name)
    {
    }

    public override void LevelUp()
    {
        //if there is money left spend it
        if (money > 0)
        {
            BuyTokens(1);
        }

        //this strategy always plays for instruments, not for markting
        for (int numTokensSpent = 0; numTokensSpent < (this.numTokens + 1); numTokensSpent++)
        {
            GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.NONE;
            int smallestValueForInstrumenteOverall = -1;

            List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(this.GetSkillSet().Keys);
            for (int j = 0; j < (playerSkillSetKeys.Count - 1); j++) //exclude markting, thats why the (playerSkillSetKeys.Count - 1)
            {
                GameProperties.Instrument currInstrument = playerSkillSetKeys[j];

                //cannot evolve fully evolved skill
                if (skillSet[currInstrument] >= GameProperties.maximumSkillLevelPerInstrument)
                {
                    continue;
                }

                int biggestValueForInstrument = -1;
                for (int i = 0; i < GameGlobals.players.Count; i++)
                {
                    Player currPlayer = GameGlobals.players[i];
                    if (this == currPlayer)
                    {
                        continue;
                    }

                    int currValue = currPlayer.GetSkillSet()[playerSkillSetKeys[j]];
                    if (biggestValueForInstrument == -1 || currValue > biggestValueForInstrument)
                    {
                        biggestValueForInstrument = currValue;
                    }
                }
                if (smallestValueForInstrumenteOverall == -1 || biggestValueForInstrument < smallestValueForInstrumenteOverall)
                {
                    smallestValueForInstrumenteOverall = biggestValueForInstrument;
                    smallestInstrumentOverall = currInstrument;
                }
            }
            //if my preferred skill is less evolved than other of my skills, I change my mind! XD
            if (skillSet[preferredInstrument] < skillSet[smallestInstrumentOverall])
            {
                preferredInstrument = smallestInstrumentOverall;
            }
            SpendToken(smallestInstrumentOverall);
        }

        if (!GameProperties.isSimulation)
        {
            base.LevelUp();
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(2.0f));
        }
        else
        {
            SendLevelUpResponse();
        }
    }

    public override void PlayForInstrument()
    {
        ChangeDiceRollInstrument(preferredInstrument);
        if (!GameProperties.isSimulation)
        {
            base.PlayForInstrument();
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(2.0f));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 0;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }

        if (!GameProperties.isSimulation)
        {
            base.LastDecisionsPhase(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(condition, 2.0f));
        }
        else
        {
            SendLastDecisionsPhaseResponse(condition);
        }
    }

}

//this strategy always plays for markting except in the first play where it is cooperative. In the last decision it always trusts his markting.
public class AIPlayerGreedyStrategy : AIPlayer
{
    GameProperties.Instrument preferredInstrument;

    public AIPlayerGreedyStrategy(string name) : base(name)
    {
    }

    public override void LevelUp()
    {

        if (gameManagerRef.GetCurrGameRound() == 0)
        {
            //this strategy always plays for instruments, not for markting
            for (int numTokensSpent = 0; numTokensSpent < (this.numTokens + 1); numTokensSpent++)
            {
                GameProperties.Instrument smallestInstrumentOverall = GameProperties.Instrument.NONE;
                int smallestValueForInstrumenteOverall = -1;

                List<GameProperties.Instrument> playerSkillSetKeys = new List<GameProperties.Instrument>(this.GetSkillSet().Keys);
                for (int j = 0; j < (playerSkillSetKeys.Count - 1); j++) //exclude markting, thats why the (playerSkillSetKeys.Count - 1)
                {
                    GameProperties.Instrument currInstrument = playerSkillSetKeys[j];

                    //cannot evolve fully evolved skill
                    if (skillSet[currInstrument] >= GameProperties.maximumSkillLevelPerInstrument)
                    {
                        continue;
                    }

                    int biggestValueForInstrument = -1;
                    for (int i = 0; i < GameGlobals.players.Count; i++)
                    {
                        Player currPlayer = GameGlobals.players[i];
                        if (this == currPlayer)
                        {
                            continue;
                        }

                        int currValue = currPlayer.GetSkillSet()[playerSkillSetKeys[j]];
                        if (biggestValueForInstrument == -1 || currValue > biggestValueForInstrument)
                        {
                            biggestValueForInstrument = currValue;
                        }
                    }
                    if (smallestValueForInstrumenteOverall == -1 || biggestValueForInstrument < smallestValueForInstrumenteOverall)
                    {
                        smallestValueForInstrumenteOverall = biggestValueForInstrument;
                        smallestInstrumentOverall = currInstrument;
                    }
                }
                preferredInstrument = smallestInstrumentOverall;
                SpendToken(smallestInstrumentOverall);
            }
        }
        else
        {
            //if there is money left spend it
            if (money > 0 && skillSet[GameProperties.Instrument.MARKETING] < GameProperties.maximumSkillLevelPerInstrument)
            {
                BuyTokens(1);
            }

            //this strategy always plays for instruments, not for markting
            for (int numTokensSpent = 0; numTokensSpent < (this.numTokens + 1); numTokensSpent++)
            {
                SpendToken(GameProperties.Instrument.MARKETING);
            }
            
        }
        if (!GameProperties.isSimulation)
        {
            base.LevelUp();
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLevelingUp(2.0f));
        }
        else
        {
            SendLevelUpResponse();
        }
    }

    public override void PlayForInstrument()
    {
        ChangeDiceRollInstrument(preferredInstrument);
        if (!GameProperties.isSimulation)
        {
            base.PlayForInstrument();
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforePlayForInstrument(2.0f));
        }
        else
        {
            SendPlayForInstrumentResponse();
        }
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 1;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }

        if (!GameProperties.isSimulation)
        {
            base.LastDecisionsPhase(currAlbum);
            playerMonoBehaviourFunctionalities.StartCoroutine(ThinkBeforeLastDecisionPhase(condition, 2.0f));
        }
        else
        {
            SendLastDecisionsPhaseResponse(condition);
        }
    }

}
