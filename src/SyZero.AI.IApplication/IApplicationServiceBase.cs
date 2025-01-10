using SyZero.Application.Attributes;
using SyZero.Application.Service;

namespace SyZero.AI.IApplication
{
    [DynamicWebApi]
    public interface IApplicationServiceBase : IApplicationService, IDynamicWebApi
    {
    }
}



