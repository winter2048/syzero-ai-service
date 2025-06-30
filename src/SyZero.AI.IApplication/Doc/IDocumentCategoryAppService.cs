using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.AI.IApplication.Doc.Dto;
using SyZero.Application.Service;

namespace SyZero.AI.IApplication.Doc
{
    public interface IDocumentCategoryAppService : IAsyncCrudAppService<DocumentCategoryDto, PageAndSortFilterQueryDto, CreateDocumentCategoryDto>, IApplicationServiceBase
    {
    }
}
