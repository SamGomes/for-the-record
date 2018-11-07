using UnityEngine;


public class AIPlayerUnbalancedStrategy : AIPlayer
{
    public AIPlayerUnbalancedStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : this(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, GameProperties.Instrument.NONE) { }
    public AIPlayerUnbalancedStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed, GameProperties.Instrument likedInstrument) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, likedInstrument)
    {
        this.type = GameProperties.PlayerType.UNBALANCED;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        //on first round put one token on instrument
        if (GameGlobals.currGameRoundId == 0)
        {
            return preferredInstrument;
        }
        else //on the other rounds see if album was a fail and play cooperatively, otherwise play for marketing
        {
            Album lastAlbum = GameGlobals.albums[GameGlobals.albums.Count - 2];
            if (lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
            {
                //greedy strategy
                return GameProperties.Instrument.MARKETING;
            }
            else if (lastAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
            {
                //coop strategy
                return preferredInstrument;
            }

            return GameProperties.Instrument.NONE; //hey code, try reaching this if u can :)
        }
    }
    //protected override int LastDecisionsActions(Album currAlbum)
    //{
    //    int condition = 0;
    //    //if I have marketing skill, I use it always
    //    if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT && skillSet[GameProperties.Instrument.MARKETING] > 0)
    //    {
    //        condition = 1;
    //    }
    //    if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
    //    {
    //        condition = 2;
    //    }
    //    return condition;
    //}
}

