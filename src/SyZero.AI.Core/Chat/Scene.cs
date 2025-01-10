using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Domain.Entities;

namespace SyZero.AI.Core.Chat
{
    public class Scene : Entity
    {
        public string Name { get; set; }

        [SugarColumn(ColumnDataType = "longtext")]
        public string Describe { get; set; }

        [SugarColumn(ColumnDataType = "longtext")]
        public string Content { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreateTime { get; set; }

        public long? CreateUser { get; set; }
    }
}
