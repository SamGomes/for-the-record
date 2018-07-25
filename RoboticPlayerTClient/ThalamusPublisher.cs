using EmoteCommonMessages;
using Thalamus;

public interface IFTRThalamusPublisher : IThalamusPublisher, IFMLSpeech
{  
}

public class ThalamusPublisher : IFTRThalamusPublisher
{
    private readonly dynamic _publisher;
    public ThalamusPublisher(dynamic publisher)
    {
        _publisher = publisher;
    }

    public void PerformUtterance(string id, string utterance, string category)
    {
        _publisher.PerformUtterance(id, utterance, category);
    }

    public void PerformUtteranceWithTags(string id, string utterance, string[] tagNames, string[] tagValues)
    {
        _publisher.PerformUtteranceWithTags(id, utterance, tagNames, tagValues);
    }

    public void PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
    {
        _publisher.PerformUtteranceFromLibrary(id, category, subcategory, tagNames, tagValues);
    }

    public void CancelUtterance(string id)
    {
        _publisher.CancelUtterance(id);
    }
}
