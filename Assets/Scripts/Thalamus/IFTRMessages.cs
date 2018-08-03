using CookComputing.XmlRpc;

public interface IFTRMessagesRpc : IFTRMessages, IXmlRpcProxy { }

public interface IFTRMessages
{
    void Dispose();

    [XmlRpcMethod]
    void PerformUtterance(string utterance, string[] tags, string[] tagsValues);

    [XmlRpcMethod]
    void PlayAnimation(string animation);

}