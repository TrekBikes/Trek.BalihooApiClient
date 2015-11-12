using System;
using System.Collections.Generic;

namespace Trek.BalihooApiClient
{

    public class Campaign
    {
        public string LocationId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }

        public List<Tactic> Tactics { get; set;}
    }
}