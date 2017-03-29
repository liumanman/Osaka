using ServiceFramwork.Server.ServiceDescription;

namespace ServiceFramwork.Server.Http
{
    public interface IServiceManager
    {
        OperationDescriptor GetOperation(string serviceName, string operationName);
        OperationDescriptor[] GetAllOperations();
        //OperationDescriptor[] MatchWithPath(string url, IServicePathManager pathManager);
    }
}
