using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Client;
using SyZero.AI.IApplication.Completion.Dto;

namespace SyZero.AI.IApplication.Completion
{
    public class CompletionAppServiceFallback : ICompletionAppService, IFallback
    {
        public Task<string> Send(CompletionDto completionDto)
        {
            throw new NotImplementedException();
        }

        public Task<string> Send2(string user)
        {
            throw new NotImplementedException();
        }
    }
}
