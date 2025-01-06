using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyZero.OpenAI.Core.Ollama
{
    public class AIModel
    {
        [JsonProperty("models")]
        public List<AIModelItem> Models { get; set; }
    }

    public class AIModelItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
    }
}
