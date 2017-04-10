
using ServiceFramework.ServiceDescription;

namespace ServiceFramework.Server
{
    public interface IServiceManager
    {
        OperationDescriptor GetOperation(string serviceName, string operationName);
        OperationDescriptor[] GetAllOperations();
        //OperationDescriptor[] MatchWithPath(string url, IServicePathManager pathManager);
    }
}
