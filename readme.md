# ✈️ FluLoS - Fluglotsen Spracherkennung

Code Repository of the BA "[Spracherkennung für Fluglotsen" (2021)](https://ba-pub.engineering.zhaw.ch/BA_WebPublication/Flyer.pdf?version=Bachelorarbeit2021&code=BA21_rege_04&language=en)

Based on [Custom Speech](https://speech.microsoft.com/customspeech), *RML* and [LUIS](https://luis.ai)

## Requirements
Operating System:	Windows 10+ (minor changes needed for Linux or macOS)
Environment:		.NET 5, Python 3+ (with miniconda or similar)
Azure Services:		Custom Speech, LUIS

## Repo Structure
```
.
├── calculation/    Scripts used to generate statistics for our thesis (python)
├── CleanUp/        CleanUp solution (C# .NET 5 wrapper for a python3 script)
├── data/           Resource Data for all the following folders
├── DeltaList/      DeltaList solution (C# .NET 5)
├── Evaluation/     Evaluation solution with plausibility checks incl. radar (C# .NET 5)
├── LUIS/           LUIS (C# .NET 5)
├── Pipeline/       Pipeline Solution - connecting all other solutions (C# .NET 5)
├── RML/            Regex Markup Language solution (C# .NET 5)
├── SharedModel/    Model class library used by multiple solutions (C# .NET 5)
└── SpeechToText/   SpeechToText solution (C# .NET 5)
```

## Setup

### Python Environment
1. Change to CleanUp Folder: `cd CleanUp/CleanUp`
2. Create Environment from environment.yml: `conda env create -f environment.yml`
3. Activate Environment: `conda activate TextCleanUp`
### Keys File
For this app to work, a file "flulos_credentials.json" must be copied into the root of the repository:
```json
{
	"S2T_subscription": "<AzureCustomSpeechSubscriptionKey>",
	"S2T_endpoint": "<AzureCustomSpeechEndpointID>",
	"LUIS_subscription": "<AzureLuisSubscriptionKey>",
	"LUIS_appid": "<AzureLuisAppId>"
}
```
### .NET Solution
1. Open `Pipeline/Pipeline.sln`
2. Change `Pipeline/Pipeline/configuration.json` according to your needs
2. Build solution

For the Web UI (`"RunWebPipeline": true` in `configuration.json`) it is required to run a `netsh` configuration command:
```
netsh http add urlacl url=http://+:8080/ user=DOMAIN\USERNAME
```

## Contributors
- vettaioa
- hauptpas
