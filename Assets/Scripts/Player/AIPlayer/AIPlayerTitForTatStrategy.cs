using UnityEngine;
using UnityEngine.UI;


public class AIPlayerTitForTatStrategy : AIPlayer
{
    private bool didSomeoneDefectedLastRound;
    private bool didSomeoneDefectedThisRound;

    public AIPlayerTitForTatStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : this(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, GameProperties.Instrument.NONE) { }
    public AIPlayerTitForTatStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed, GameProperties.Instrument likedInstrument) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, likedInstrument)
    {
        didSomeoneDefectedThisRound = false;
    }

    protected override void InformLevelUpActions(Player invoker, GameProperties.Instrument leveledUpInstrument)
    {
        if (didSomeoneDefectedThisRound == false && invoker.GetPlayerType()!= GameProperties.PlayerType.TITFORTAT)
        {
            didSomeoneDefectedThisRound = (leveledUpInstrument == GameProperties.Instrument.MARKETING);
        }
    }


    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        GameProperties.Instrument levelUpInstrument = GameProperties.Instrument.NONE;
        //if someone defected than defect
        if (didSomeoneDefectedLastRound)
        {
            levelUpInstrument = GameProperties.Instrument.MARKETING;
        }
        else //on the other rounds see if album was a fail and play cooperatively, otherwise play for marketing
        {

            levelUpInstrument = preferredInstrument;
        }
        return levelUpInstrument;
    }
    protected override int LastDecisionsActions(Album currAlbum)
    {
        //abused this method to update the perception of the AI sry :(
        didSomeoneDefectedLastRound = didSomeoneDefectedThisRound;
        didSomeoneDefectedThisRound = false;
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaa: " + didSomeoneDefectedLastRound);

        int condition = 0;
        //if I have marketing skill, I use it always
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT && skillSet[GameProperties.Instrument.MARKETING] > 0)
        {
            condition = 1;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }
        return condition;
    }
}