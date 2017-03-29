using Microsoft.AspNetCore.Http;

using ServiceFramwork.Server.ServiceDescription;

namespace ServiceFramwork.Server.Http
{
    public static class HttpContextExtension
    {
        public static void SetDispatchOperation(this HttpContext context, OperationDescriptor operation)
        {
            context.Items["DispatchOperation"] = operation;
        }
        public static OperationDescriptor GetDispatchOperation(this HttpContext context)
        {
            object operation;
            if (context.Items.TryGetValue("", out operation))
            {
                return (OperationDescriptor)operation;
            }
            else
            {
                return null;
            }
        }
    }
}
