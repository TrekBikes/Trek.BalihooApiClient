using System;

namespace Trek.BalihooApiClient
{
    public class EmailSentEvent
    {
        public int LocationKey { get; set; }
        public int TacticId { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}