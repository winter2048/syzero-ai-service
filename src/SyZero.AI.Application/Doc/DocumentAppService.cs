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
    public class DocumentAppService : AsyncCrudAppService<Document, DocumentDto, PageAndSortFilterQueryDto, CreateDocumentDto>, IDocumentAppService
    {
        private readonly IRepository<Document> _documentRepository;

        public DocumentAppService(IRepository<Document> documentRepository) : base(documentRepository)
        {
            _documentRepository = documentRepository;
        }


    }
}
