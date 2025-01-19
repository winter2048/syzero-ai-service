using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Application.Routing;
using SyZero.AI.IApplication.Completion.Dto;

namespace SyZero.AI.IApplication.Completion
{
    public interface ICompletionAppService : IApplicationServiceBase
    {
        [Get]
        Task<string> Send(CompletionDto completionDto);

        [Post("/users/{user}")]
        Task<string> Send2(string user);
    }
}
