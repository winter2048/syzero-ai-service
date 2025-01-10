using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SyZero.AI.Core
{
    public enum AIChatModel
    {
        [Description("OpenAI|gpt-3.5-turbo")]
        OpenAI_GPT35Turbo,

        [Description("OpenAI|gpt-4o-mini")]
        OpenAI_GPT4OMini,

        [Description("OpenAI|gpt-4o")]
        OpenAI_GPT4O,

        [Description("OpenAI|gpt-4-turboi")]
        OpenAI_GPT4Turbo,

        [Description("Ollama|qwen2.5:3b")]
        Ollama_Qwen25_3B,

        [Description("Ollama|qwen2.5:7b")]
        Ollama_Qwen25_7B,

        [Description("Ollama|qwen2.5:14b")]
        Ollama_Qwen25_14B
    }

    public enum AIEmbeddingModel
    {
        [Description("OpenAI|text-embedding-3-large")]
        OpenAI_Text_Embedding3_Large,

        [Description("OpenAI|text-embedding-3-small")]
        OpenAI_Text_Embedding3_Small,

        [Description("Ollama|nomic-embed-text")]
        Ollama_Nomic_Embed_Text,

        [Description("Ollama|all-minilm")]
        Ollama_All_Minilm,

        [Description("Ollama|mxbai-embed-large")]
        Ollame_Mxbai_Embed_Large
    }

    public static class AIEnumExtension
    {
        public static string ToModel(this AIEmbeddingModel model)
        {
            return model.ToDescription().Split("|")[1];
        }

        public static string ToModel(this AIChatModel model)
        {
            return model.ToDescription().Split("|")[1];
        }

        public static bool IsOpenAI(this string model)
        {
            return model.StartsWith(AIProvider.OpenAI.ToString());
        }

        public static bool IsOllama(this string model)
        {
            return model.StartsWith(AIProvider.Ollama.ToString());
        }

        public static bool IsOpenAI(this AIEmbeddingModel model)
        {
            return model.ToDescription().StartsWith(AIProvider.OpenAI.ToString());
        }

        public static bool IsOllama(this AIEmbeddingModel model)
        {
            return model.ToDescription().StartsWith(AIProvider.Ollama.ToString());
        }

        public static string ToModel(this string model)
        {
            return model.Split("|")[1];
        }
    }
}
