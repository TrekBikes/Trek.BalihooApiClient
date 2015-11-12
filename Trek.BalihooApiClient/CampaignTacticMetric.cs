using System.Collections.Generic;

namespace Trek.BalihooApiClient
{
    public class CampaignTacticMetric
    {
        public int CampaignId { get; set; }
        public List<int> TacticIds { get; set; } // No idea why this is an array in their response
        public string Channel { get; set; }
        public int? Sends { get; set; } // Email
        public int? Opens { get; set; } // Email
        public int? Clicks { get; set; } // Email, Paid Search
        public decimal? Spend { get; set; } // Paid Search, Display
        public int? Impressions { get; set; } // Display
    }
}