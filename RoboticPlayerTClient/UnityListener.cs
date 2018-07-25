using CookComputing.XmlRpc;

public class UnityListener : XmlRpcListenerService, IFRTMessages
{
    private ThalamusConnector _thalamusCS;

    public UnityListener(ThalamusConnector thalamusCS)
    {
        _thalamusCS = thalamusCS;
    }

    public void PerformUtterance(string utterance)
    {
        _thalamusCS.TypifiedPublisher.PerformUtteranceWithTags("", utterance, new string[]{ }, new string[] { });
    }

    public void PlayAnimation(string animation)
    {
        //_thalamusCS.TypifiedPublisher.PlayAnimation(animation);
    }
        
    public void Dispose()
    {
    }
}
