using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Domain.Entities;

namespace SyZero.AI.Core.Doc
{
    public class Document : Entity
    {
        public string Name { get; set; }

        public long DocumentCategoryId { get; set; }

        public string Type { get; set; }

        public string Source { get; set; }

        public bool IsVector { get; set; }

        public DateTime CreateTime { get; set; }

        public long? CreateUser { get; set; }
    }
}
