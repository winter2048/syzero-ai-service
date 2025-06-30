using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.AI.Core.Doc;
using SyZero.AI.IApplication.Doc;
using SyZero.AI.IApplication.Doc.Dto;
using SyZero.Application.Service;
using SyZero.Domain.Repository;

namespace SyZero.AI.Application.Doc
{
    public class DocumentCategoryAppService : AsyncCrudAppService<DocumentCategory, DocumentCategoryDto, PageAndSortFilterQueryDto, CreateDocumentCategoryDto>, IDocumentCategoryAppService
    {
        private readonly IRepository<DocumentCategory> _documentCategoryRepository;

        public DocumentCategoryAppService(IRepository<DocumentCategory> documentCategoryRepository) : base(documentCategoryRepository)
        {
            _documentCategoryRepository = documentCategoryRepository;
        }


    }
}
