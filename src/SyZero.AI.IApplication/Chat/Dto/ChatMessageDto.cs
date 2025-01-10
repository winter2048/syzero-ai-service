using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyZero.AI.IApplication.Chat.Dto
{
    public class ChatMessageDto
    {
        public ChatMessageDto() { }

        public ChatMessageDto(MessageRoleEnum role, string content) { 
            this.Role = role;
            this.Content = content;
            this.Date = DateTime.Now;
        }

        public MessageRoleEnum Role { get; set; }

        public string Content { get; set; }

        public DateTime? Date { get; set; }
    }
}
