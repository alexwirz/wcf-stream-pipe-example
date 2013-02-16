using System.IO;
using System.ServiceModel;

namespace contract
{
    [ServiceContract]
    public interface Streaming
    {
        [OperationContract]
        void ReceiveStream(Stream stream);
    }
}
