using CookComputing.XmlRpc;

public class UnityListener : XmlRpcListenerService, IFRTMessages
{
    private ThalamusConnector _thalamusCS;

    public UnityListener(ThalamusConnector thalamusCS)
    {
        _thalamusCS = thalamusCS;
    }

    public void PerformUtterance(string utterance, string[] tags, string[] tagsValues)
    {
        _thalamusCS.TypifiedPublisher.PerformUtteranceWithTags("", utterance, tags, tagsValues);
    }

    public void PlayAnimation(string animation)
    {
        //_thalamusCS.TypifiedPublisher.PlayAnimation(animation);
    }
        
    public void Dispose()
    {
    }
}
