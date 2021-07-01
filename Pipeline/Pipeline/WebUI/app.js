//PSEUDO-DATA
const stts = JSON.parse(`
[
  "alitalia three zero one descend to flight level three zero zero traffic",
  "alitalia three zero one descend to flight level ah three zero zero traffic",
  "alitalia three zero one descend flight level three zero zero traffic"
]
`)
const contexts = JSON.parse(`
[
  {
    "LuisContext": {
      "Message": "alitalia 301 descend to flight level 300 traffic",
      "CallSign": {
        "Airline": "alitalia",
        "FlightNumber": "301"
      },
      "Intents": {
        "None": {
          "Score": 0.0049089547
        },
        "Contact": {
          "Frequency": null,
          "Place": null,
          "Score": 0.0018584969
        },
        "FlightLevel": {
          "Instruction": "Descend",
          "Level": "300",
          "Score": 0.9978382
        },
        "Squawk": {
          "Code": null,
          "Score": 0.0021387925
        },
        "Turn": {
          "Direction": null,
          "Degrees": null,
          "Heading": null,
          "Place": null,
          "Score": 0.0016026356
        }
      }
    },
    "RmlContext": {
      "Message": "alitalia 301 descend to flight level 300 traffic",
      "CallSign": {
        "Airline": "alitalia",
        "FlightNumber": "301"
      },
      "Intents": {
        "FlightLevel": {
          "Instruction": "Descend",
          "Level": "300",
          "Score": 1.0
        }
      }
    }
  },
  {
    "LuisContext": {
      "Message": "alitalia 301 descend to flight level 300 traffic",
      "CallSign": {
        "Airline": "alitalia",
        "FlightNumber": "301"
      },
      "Intents": {
        "None": {
          "Score": 0.0049089547
        },
        "Contact": {
          "Frequency": null,
          "Place": null,
          "Score": 0.0018584969
        },
        "FlightLevel": {
          "Instruction": "Descend",
          "Level": "300",
          "Score": 0.9978382
        },
        "Squawk": {
          "Code": null,
          "Score": 0.0021387925
        },
        "Turn": {
          "Direction": null,
          "Degrees": null,
          "Heading": null,
          "Place": null,
          "Score": 0.0016026356
        }
      }
    },
    "RmlContext": {
      "Message": "alitalia 301 descend to flight level 300 traffic",
      "CallSign": {
        "Airline": "alitalia",
        "FlightNumber": "301"
      },
      "Intents": {
        "FlightLevel": {
          "Instruction": "Descend",
          "Level": "300",
          "Score": 1.0
        }
      }
    }
  },
  {
    "LuisContext": {
      "Message": "alitalia 301 descend flight level 300 traffic",
      "CallSign": {
        "Airline": "alitalia",
        "FlightNumber": "301"
      },
      "Intents": {
        "None": {
          "Score": 0.0063280193
        },
        "Contact": {
          "Frequency": null,
          "Place": null,
          "Score": 0.0018313368
        },
        "FlightLevel": {
          "Instruction": "Descend",
          "Level": "300",
          "Score": 0.99427915
        },
        "Squawk": {
          "Code": null,
          "Score": 0.0022202933
        },
        "Turn": {
          "Direction": null,
          "Degrees": null,
          "Heading": null,
          "Place": null,
          "Score": 0.001554251
        }
      }
    },
    "RmlContext": {
      "Message": "alitalia 301 descend flight level 300 traffic",
      "CallSign": {
        "Airline": "alitalia",
        "FlightNumber": "301"
      },
      "Intents": {
        "FlightLevel": {
          "Instruction": "Descend",
          "Level": "300",
          "Score": 1.0
        }
      }
    }
  }
]
`)

const evaluationflags = JSON.parse(`
{
  "LuisEvaluations": [
    {
      "RadarAirplane": null,
      "SquawkResult": "Invalid",
      "ContactResult": "Invalid",
      "FlightLevelResult": "FlightLevelValid",
      "TurnResult": "Invalid"
    },
    {
      "RadarAirplane": null,
      "SquawkResult": "Invalid",
      "ContactResult": "Invalid",
      "FlightLevelResult": "FlightLevelValid",
      "TurnResult": "Invalid"
    },
    {
      "RadarAirplane": null,
      "SquawkResult": "Invalid",
      "ContactResult": "Invalid",
      "FlightLevelResult": "FlightLevelValid",
      "TurnResult": "Invalid"
    }
  ],
  "RmlEvaluations": [
    {
      "RadarAirplane": null,
      "SquawkResult": "Invalid",
      "ContactResult": "Invalid",
      "FlightLevelResult": "FlightLevelValid",
      "TurnResult": "Invalid"
    },
    {
      "RadarAirplane": null,
      "SquawkResult": "Invalid",
      "ContactResult": "Invalid",
      "FlightLevelResult": "FlightLevelValid",
      "TurnResult": "Invalid"
    },
    {
      "RadarAirplane": null,
      "SquawkResult": "Invalid",
      "ContactResult": "Invalid",
      "FlightLevelResult": "FlightLevelValid",
      "TurnResult": "Invalid"
    }
  ]
}
`)


// Helper Functions

const GetElement = (elementId) => {
    return document.getElementById(elementId)
}
/*
 * Example Usage:
 * WriteTable('', 'blibla', {'Bla': ['blub'], 'Bli': [4]})
 * WriteTable('', ['blibla', 'blablu'], {'Bla': ['blub', 'jup'], 'Bli': [4, 'nope']})
 */
const WriteTable = (parentElement, tableHeader, keyValues) => {
    const ToTableCell = (data) => {
        if (data === undefined || data === null) {
            return `<td><i>${data}</i></td>`
        }
        return `<td>${data}</td>`
    }

    const header = typeof tableHeader === 'string'
        ? `<tr class="text-center"><th colspan="2">${tableHeader}</th></tr>`
        : ('<thead><tr>' + tableHeader.map(h => `<th>${h}</th>`).join('') + '</tr></thead>');
    const rows = Object.entries(keyValues).map(e => {
        const cols = typeof e[1] === 'string' || typeof e[1] === 'number'
            ? ToTableCell(e[1])
            : e[1]?.map(c => ToTableCell(c))?.join('')
        return `<tr><th>${e[0]}</th>${cols}</tr>`
    }).join('\n');

    parentElement.innerHTML = `<table class="table">\n${header}\n${rows}</table>`;
}
const CleanElement = (parentElement) => {
    parentElement.innerHTML = '';
}

const WriteSpeechToTextResult = (transcriptions, cleanedTranscriptions) => {
    const header = ['#', 'Speech-To-Text (literal) result', 'Cleaned Transcription'];
    const data = {1: [], 2: [], 3: []}

    for (let i = 0; i < 3; i++) {
        data[i + 1] = [transcriptions[i], cleanedTranscriptions?.[i] || '-']
    }

    if (data[1].length > 0) {
        WriteTable(GetElement('result-speechtotext'), header, data);
    } else {
        CleanElement(GetElement('result-speechtotext'));
    }
}

const WriteContextResult = (luisContext, rmlContext) => {

    const WriteCallSigns = () => { // write CallSigns
        const luisCallSign = luisContext['CallSign']
        const rmlCallSign = rmlContext['CallSign']

        if (luisCallSign) {
            WriteTable(GetElement('result-luis-callsign'), 'CallSign', luisCallSign);
        } else {
            CleanElement(GetElement('result-luis-callsign'));
        }

        if (rmlCallSign) {
            WriteTable(GetElement('result-rml-callsign'), 'CallSign', rmlCallSign);
        } else {
            CleanElement(GetElement('result-rml-callsign'));
        }
        
    };

    const WriteIntent = (intent) => {
        const intentLc = intent.toLowerCase();

        const luisIntent = luisContext['Intents'][intent]
        const rmlIntent = rmlContext['Intents'][intent]

        console.log('writeintent', intent, {luisIntent, rmlIntent})

        if (luisIntent && luisIntent['Score'] > 0.1) {
            WriteTable(GetElement(`result-luis-${intentLc}`), intent, luisIntent);
        } else {
            CleanElement(GetElement(`result-luis-${intentLc}`));
        }

        if (rmlIntent) {
            WriteTable(GetElement(`result-rml-${intentLc}`), intent, rmlIntent);
        } else {
            CleanElement(GetElement(`result-rml-${intentLc}`));
        }
    }

    WriteCallSigns();
    ['FlightLevel', 'Turn', 'Contact', 'Squawk'].forEach(intent => WriteIntent(intent))
    
}

const WriteEvaluationResult = (rmlEvaluationFlags, rmlContext) => {
    const tableHeaders = [
        'Validation',
        'Extracted Value',
    ];
    const tableContent = [
        {'Airline': 'Invalid'}
    ];
}


// Callbacks
const StartRecording = (recorder) => {
    console.debug('start recording...');
    recorder.record();
    isRecording = true;
    microphoneBtn.classList.add('recording')
}
const StopRecording = (recorder) => {
    console.debug('stop recording');
    recorder.stop();
    isRecording = false;
    microphoneBtn.classList.remove('recording')

    // recorder.js calls callback with blob
    recorder.exportWAV(SendRecording);
}

const SendRecording = (blob) => {
    fetch('/process', {
        method: 'post',
        body: blob,
        mimeType: 'audio/wav',
    })
    .then(res => res.text())
    .then(data => {
        console.log('received UID :D', data) // TODO: hier noch handling for start von retry prozess einfügen
        resultBox.innerHTML = data;
    })
}

// API Callbacks

let cache_lastSpeechToTextResult = [];
let cache_lastContextResult = [];

const OnSpeechToTextResult = (sttResult) => {
    console.debug('Received speechToTextResult', sttResult)

    cache_lastSpeechToTextResult = sttResult;
}

const OnContextResult = (contextResult) => {
    console.debug('Received contextResult', contextResult)

    // write speech to text section
    const cleanedTranscriptions = contextResult.map(c => c['LuisContext']['Message'] || null)
    WriteSpeechToTextResult(cache_lastSpeechToTextResult, cleanedTranscriptions)

    //// write context extraction section
    const bestContext = contextResult[0]
    WriteContextResult(bestContext['LuisContext'], bestContext['RmlContext'])

    cache_lastContextResult = contextResult;
}

const OnEvaluationResult = (evaluationResult) => {
    console.debug('Received evaluationResult', evaluationResult)

    const bestFlagsRml = evaluationResult['RmlEvaluations'][0];
    const bestContextRml = cache_lastContextResult[0]['RmlContext'];
    WriteEvaluationResult(bestFlagsRml, bestContextRml);
}

// Main Code
const errorBox = document.getElementsByClassName('audioerr')[0]
const microphoneBtn = document.getElementsByClassName('microphone')[0]
const resultBox = document.getElementsByClassName('resultbox')[0]

let isRecording = false;

if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
	navigator.mediaDevices.getUserMedia({ audio: true })
		.then(function (stream) {
			const mediaRecorder = new MediaRecorder(stream, { mimeType: 'audio/webm;codecs=opus' })

            const audioContext = new window.AudioContext;/*new (window.AudioContext || window.webkitAudioContext);*/

            const recorder = new Recorder(audioContext.createMediaStreamSource(stream), { numChannels: 1 });

			microphoneBtn.onclick = () => {
				if (!isRecording) {
                    StartRecording(recorder);
				} else {
                    StopRecording(recorder);
				}
			};
		})
		.catch(function (err) {
			errorBox.innerHTML = '<b>Audio Support Error</b>:<br>' + err;
			errorBox.classList.remove('d-none')
		})
} else {
	errorBox.innerHTML = '<b>Your browser doesn\'t support audio input</b>';
	errorBox.classList.remove('d-none')
}

// try to get the response
//function tryFetch(func, retries) {
//    return new Promise(function (resolve, reject) {
//        var tryDownload = function (attempts) {
//            try {
//                downloadItem(url);
//                resolve();
//            } catch (e) {
//                if (attempts == 0) {
//                    reject(e);
//                } else {
//                    setTimeout(function () {
//                        tryDownload(attempts - 1);
//                    }, 1000);
//                }
//            }
//        };
//        tryDownload(retries);
//    });
//}

OnSpeechToTextResult(stts)
OnContextResult(contexts)
OnEvaluationResult(evaluationflags)

