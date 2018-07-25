using CookComputing.XmlRpc;

public interface IFTRUnityCaller
{
    void ReceiveInformation();
}

public interface IFTRUnityCallerRpc : IFTRUnityCaller, IXmlRpcProxy
{
    [XmlRpcMethod]
    new void ReceiveInformation();
}
