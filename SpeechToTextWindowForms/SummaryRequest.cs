using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToTextWindowForms
{
    public class SummaryRequest
    {
        public SummaryRequest()
        {
            this.temperature = 0.7f;
            this.max_tokens = 64;
            this.top_p = 1.0f;
            this.frequency_penalty = 0.0f;
            this.presence_penalty = 0.0f;
        }
        public string prompt { get; set; }
        public float temperature { get; set; }
        public int max_tokens { get; set; }
        public float top_p { get; set; }
        public float frequency_penalty { get; set; }
        public float presence_penalty { get; set; }
    }

    public class SummaryResult
    {
        public string id { get; set; }
        public string _object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public Choice[] choices { get; set; }
    }
    public class Choice
    {
        public string text { get; set; }
        public int index { get; set; }
        public object logprobs { get; set; }
        public string finish_reason { get; set; }
    }
}
