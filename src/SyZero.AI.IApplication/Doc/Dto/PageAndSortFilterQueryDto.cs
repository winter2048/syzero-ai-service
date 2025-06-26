using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Application.Service.Dto;

namespace SyZero.AI.IApplication.Doc.Dto
{
    public class PageAndSortFilterQueryDto : PageAndSortQueryDto
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public long? CategoryId { get; set; }
    }
}
