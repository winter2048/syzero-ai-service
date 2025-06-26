using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Domain.Entities;

namespace SyZero.AI.Core.Doc
{
    public class DocumentSlice : Entity
    {
        public long DocumentId { get; set; }

        [SugarColumn(ColumnDataType = "longtext")]
        public string Content { get; set; }
    }
}
