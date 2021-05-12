# ✈️ FluLoS - Fluglotsen Spracherkennung

Code Repository of the BA "Spracherkennung für Fluglotsen" (2021)

Based on [Custom Speech](https://speech.microsoft.com/customspeech), *RML* and [LUIS](https://luis.ai)

### Repo Structure
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

<!--
# Contributors
![](https://avatars.githubusercontent.com/u/78963050?s=20) vettaioa
![](https://avatars.githubusercontent.com/u/40953430?s=20) hauptpas
-->