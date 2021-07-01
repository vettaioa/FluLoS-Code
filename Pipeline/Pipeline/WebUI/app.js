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


// Helper Functions
/*
 * Example Usage:
 * WriteTable('', 'blibla', {'Bla': ['blub'], 'Bli': [4]})
 * WriteTable('', ['blibla', 'blablu'], {'Bla': ['blub', 'jup'], 'Bli': [4, 'nope']})
 */
const GetElement = (elementId) => {
    return document.getElementById(elementId)
}
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

    for (let i = 0; i < 3; ) {
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


// Callbacks
const StartRecording = (mediaRecorder) => {
    console.debug('start recording...');
    mediaRecorder.start();
    isRecording = true;
    microphoneBtn.classList.add('recording')
}
const StopRecording = (mediaRecorder) => {
    console.debug('stop recording');
    mediaRecorder.stop();
    isRecording = false;
    microphoneBtn.classList.remove('recording')
}

const SendRecording = (blob) => {
    fetch('/process', {
        method: 'post',
        body: blob,
    })
    .then(res => res.text())
    .then(data => {
        resultBox.innerHTML = data;
    })
}

// API Callbacks

let cache_lastSpeechToTextResult = [];
const OnSpeechToTextResult = (sttResult) => {
    console.debug('Received speechToTextResult', sttResult)
    cache_lastSpeechToTextResult = sttResult;
}

const OnContextResult = (contextResult) => {
    console.debug('Received contextResult', contextResult)

    // write speech to text section
    const cleanedTranscriptions = contextResult.map(c => c['LuisContext']['Message'] || null)
    WriteSpeechToTextResult(cache_lastSpeechToTextResult, cleanedTranscriptions)

    // write context extraction section
    const bestContext = contextResult[0]
    WriteContextResult(bestContext['LuisContext'], bestContext['RmlContext'])
}


// Main Code
const errorBox = document.getElementsByClassName('audioerr')[0]
const microphoneBtn = document.getElementsByClassName('microphone')[0]
const resultBox = document.getElementsByClassName('resultbox')[0]

let isRecording = false;

if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia
	&& MediaRecorder.isTypeSupported('audio/webm;codecs=opus')) {
	navigator.mediaDevices.getUserMedia({ audio: true })
		.then(function (stream) {
			const mediaRecorder = new MediaRecorder(stream, { mimeType: 'audio/webm;codecs=opus' })

			let chunks = []
			mediaRecorder.ondataavailable = function (e) {
				console.log('got chunk', e.data);
				chunks.push(e.data);
			}

			mediaRecorder.onstop = () => {
				const blob = new Blob(chunks, { 'type': 'audio/webm;codecs=opus' });
				SendRecording(blob);
			}

			microphoneBtn.onclick = () => {
				if (!isRecording) {
					chunks = []
					StartRecording(mediaRecorder);
				} else {
					StopRecording(mediaRecorder, chunks);
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

OnSpeechToTextResult(stts)
OnContextResult(contexts)

