using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyZero.OpenAI.Core.OpenAI.Dto
{
    public class AIModel
    {
        [JsonProperty("data")]
        public List<AIModelItem> Data { get; set; }
    }

    public class AIModelItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
