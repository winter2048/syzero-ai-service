using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Application.Service.Dto;

namespace SyZero.AI.IApplication.Chat.Dto
{
    public class SceneDto : EntityDto
    {
        public string Name { get; set; }

        public string Describe { get; set; }

        public List<ChatMessageDto> Content { get; set; }

        public DateTime? CreateTime { get; set; }

        public bool IsDefault { get; set; }
    }
}
