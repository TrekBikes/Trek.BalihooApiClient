using System;
using System.Collections.Generic;
using System.Linq;

namespace Trek.BalihooApiClient.Sample
{
    class Program
    {
        static void Main()
        {
            // https://github.com/balihoo/local-connect-client

            var client = new BalihooApiClient();
            //client.GenerateClientApiKey("", "");

            // Not sure where this list of location Ids are supposed to come from
            var list = new List<int> { 85104, 1103020 };


            var result = client.GetCampaigns(list);


            var tactics = (from location in result
                           from campaign in location.Value
                           from tactic in campaign.Tactics
                           select tactic.Id).Distinct().ToList();

            foreach (var tacticId in tactics)
            {
                var tacticMetrics = client.GetTacticMetrics(list, tacticId);
            }

            // Get Location Email Report
            var reportResults = client.GetEmailReportData(list);
            
            Console.ReadLine();
        }
    }

}
