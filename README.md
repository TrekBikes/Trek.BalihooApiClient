# C# Balihoo API Client
This is a partial implementation of [Balihoo's Local Connect API](https://github.com/balihoo/local-connect-client).

## Usage
```
// Use this if you don't have a Client API key
var client = new BalihooApiClient();
client.GenerateClientApiKey("{your GUID}", "{your brand}");

// or this if you do
var client = new BalihooApiClient("{your Client ID", "your Client API Key");

// Get Campaings (w/ Tactics)
var loctaionIds = new List<int> { 85104, 1103020 };
var result = client.GetCampaigns(locationIds);

// Get Location Email Report
var reportData = client.GetEmailReportData(locationIds);
```
