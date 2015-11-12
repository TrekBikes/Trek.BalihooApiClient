using System;

namespace Trek.BalihooApiClient
{
    public class Tactic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Channel { get; set; }
        public string Creative { get; set; }
    }
}