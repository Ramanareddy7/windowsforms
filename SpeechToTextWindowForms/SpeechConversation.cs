using System;
using System.Collections.Generic;

namespace SpeechToTextWindowForms
{
    public class SpeechConversation
    {
        public string Agent { get; set; }
        public string Customer { get; set; }
        public string Conversation { get; set; }
        public DateTime StartTime { get; set; }
        public List<LineConversation> LineConversations { get; set; }
    }
    public class LineConversation
    {
        public int LineId { get; set; }
        public string Speaker { get; set; }
        public string LineText { get; set; }
    }
    public class ConversationSummary
    {
        public string ProblemStatement { get; set; }
        public string Solution { get; set; }
        public string CurrentSituation { get; set; }
        public string DesiredSituation { get; set; }
        public string Remarks { get; set; }
    }
}
