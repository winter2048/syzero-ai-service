using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyZero.AI.IApplication.Chat.Dto
{
    public class ChatSessionDto
    {
        public string Id { get; set; }

        public List<ChatMessageDto> Messages { get; set; }
    }
}
