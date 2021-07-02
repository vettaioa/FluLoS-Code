// Helper Functions
const GetElement = (elementId) => {
    return document.getElementById(elementId)
}
/*
 * Example Usage:
 * WriteTable(GetElement(''), 'blibla', {'Bla': ['blub'], 'Bli': [4]})
 * WriteTable(GetElement(''), ['blibla', 'blablu'], {'Bla': ['blub', 'jup'], 'Bli': [4, 'nope']})
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
            : e[1]?.map(c => ToTableCell(c))?.join('') || '<td><i>-</i></td>'
        return `<tr><th>${e[0]}</th>${cols}</tr>`
    }).join('\n');
    console.log('WriteTable', {header, rows})
    parentElement.innerHTML = `<table class="table">\n${header}\n${rows}</table>`;
}
const CleanElement = (parentElement) => {
    parentElement.innerHTML = '';
}

const WriteAirspace = (airspace) => {
    const tableHeaders = ['Identification', 'Airline', 'Level', 'Speed', 'Vert', '']
    const tableContent = {};

    for (const plane of airspace['airplanes']) {
        if (plane?.Airplane?.Flight?.FlightIdentification) {
            tableContent[plane.Airplane.Flight.FlightIdentification] = [
                plane.Airplane.Flight.Airline?.Callsign || plane.Airplane.Flight.Airline?.Name || '<i>n/a</i>',
                plane.Position?.Altitude ? parseInt(plane.Position?.Altitude / 100) : '<i>n/a</i>',
                plane.Position?.Speed,
                plane.Position?.VerticalRate,
                plane.Position?.OnGround? 'Gnd' : 'Air',
            ];
        }
    }

    WriteTable(GetElement('airspace-planes'), tableHeaders, tableContent)
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

    const WriteCallSigns = () => {
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

        //console.log('writeintent', intent, {luisIntent, rmlIntent})

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

const WriteEvaluationResult = (rmlEvaluationFlags, luisEvaluationFlags) => {
    const invalidToDash = (value) => value == 'Invalid' ? '-' : value;

    const tableHeaders = [
        'Validation',
        'Result',
    ];
    const tableContentRml = {
        'CallSign': rmlEvaluationFlags['RadarAirplane'] ? 'In Airspace' : '<i>Not in Airspace</i>',
        'FlightLevel Intent': invalidToDash(rmlEvaluationFlags['FlightLevelResult']),
        'Turn Intent': invalidToDash(rmlEvaluationFlags['TurnResult']),
        'Contact Intent': invalidToDash(rmlEvaluationFlags['ContactResult']),
        'Squawk Intent': invalidToDash(rmlEvaluationFlags['SquawkResult']),
    }

    WriteTable(GetElement(`result-evaluation-rml`), tableHeaders, tableContentRml);

    const tableContentLuis = {
        'CallSign': luisEvaluationFlags['RadarAirplane'] ? 'In Airspace' : '<i>Not in Airspace</i>',
        'FlightLevel Intent': invalidToDash(luisEvaluationFlags['FlightLevelResult']),
        'Turn Intent': invalidToDash(luisEvaluationFlags['TurnResult']),
        'Contact Intent': invalidToDash(luisEvaluationFlags['ContactResult']),
        'Squawk Intent': invalidToDash(luisEvaluationFlags['SquawkResult']),
    }

    WriteTable(GetElement(`result-evaluation-luis`), tableHeaders, tableContentLuis);
}

const WriteFinalResult = (finalRmlContext, finalLuisContext) => {

    const WriteCallSign = () => {
        const luisCallSign = finalLuisContext['CallSign']
        const rmlCallSign = finalRmlContext['CallSign']

        if (luisCallSign) {
            WriteTable(GetElement('result-final-luis-callsign'), 'CallSign', luisCallSign);
        } else {
            CleanElement(GetElement('result-final-luis-callsign'));
        }

        if (rmlCallSign) {
            WriteTable(GetElement('result-final-rml-callsign'), 'CallSign', rmlCallSign);
        } else {
            CleanElement(GetElement('result-final-rml-callsign'));
        }

    };

    const WriteIntent = (intent) => {
        const intentLc = intent.toLowerCase();

        const luisIntent = finalLuisContext['Intents'][intent]
        const rmlIntent = finalRmlContext['Intents'][intent]

        if (luisIntent && luisIntent['Score'] > 0.1) {
            WriteTable(GetElement(`result-final-luis-${intentLc}`), intent, luisIntent);
        } else {
            CleanElement(GetElement(`result-final-luis-${intentLc}`));
        }

        if (rmlIntent) {
            WriteTable(GetElement(`result-final-rml-${intentLc}`), intent, rmlIntent);
        } else {
            CleanElement(GetElement(`result-final-rml-${intentLc}`));
        }
    }

    WriteCallSign();
    ['FlightLevel', 'Turn', 'Contact', 'Squawk'].forEach(intent => WriteIntent(intent))

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
    .then(uid => {
        console.log('received UID :D', uid) // TODO: hier noch handling for start von retry prozess einfügen
        statusBox.innerHTML = 'Pipeline is Running';
        GetPipelineOutput(uid);
    })
}

const UpdateAirspace = () => {
    fetch('/airspace', {
        method: 'get',
    })
    .then(res => res.json())
    .then(data => {
        console.debug('received airspace', data)
        WriteAirspace(data);
    })
}


const GetPipelineOutput = async (uid) => {
    const LoadOutput = (type, maxRetries = 10) => {
        return new Promise((resolve, reject) => {
            const tryFetch = (retries) => {
                fetch(`/output?uid=${uid}&type=${type}`, {
                    method: 'get',
                })
                .then(res => res.json()) // if not json => catch() is executed
                .then(data => {
                //console.debug('received Pipeline Output ' + type, data);
                if (data) {
                    resolve(data)
                } else {
                    throw new Error('No data received')
                }
                })
                .catch((e) => {
                    if (retries > 0) {
                        setTimeout(() => tryFetch(retries - 1), 750);
                    } else {
                        reject(e);
                    }
                })
            }
            tryFetch(maxRetries);
        })
    }

    let resTranscription, resContext, resEvaluationFlags, resValidatedMerged;

    resTranscription = await LoadOutput('transcription', 20);
    OnSpeechToTextResult(resTranscription);

    resContext = await LoadOutput('context');
    OnContextResult(resContext, resTranscription);

    resEvaluationFlags = await LoadOutput('evaluationflags');
    OnEvaluationResult(resEvaluationFlags);

    resValidatedMerged = await LoadOutput('validatedmerged');
    OnFinalResult(resValidatedMerged);
}

// API Callbacks
const OnSpeechToTextResult = (sttResult) => {
    statusBox.innerHTML = 'Speech To Text is Done'
    console.debug('Received speechToTextResult', sttResult)
}

const OnContextResult = (contextResult, sttResult) => {
    statusBox.innerHTML = 'Context Extraction (Luis and RML) is Done'
    console.debug('Received contextResult', contextResult)

    // write speech to text section
    const cleanedTranscriptions = contextResult.map(c => c['LuisContext']['Message'] || null)
    WriteSpeechToTextResult(sttResult, cleanedTranscriptions)

    //// write context extraction section
    const bestContext = contextResult[0]
    WriteContextResult(bestContext['LuisContext'], bestContext['RmlContext'])
}

const OnEvaluationResult = (evaluationResult) => {
    statusBox.innerHTML = 'Evaluation is Done'
    console.debug('Received evaluationResult', evaluationResult)

    const bestFlagsRml = evaluationResult['RmlEvaluations'][0];
    const bestFlagsLuis = evaluationResult['LuisEvaluations'][0];
    WriteEvaluationResult(bestFlagsRml, bestFlagsLuis);
}

const OnFinalResult = (finalResult) => {
    statusBox.innerHTML = ''

    WriteFinalResult(finalResult['RmlContext'], finalResult['LuisContext']);
}

// Main Code
const errorBox = document.getElementsByClassName('audioerr')[0]
const microphoneBtn = document.getElementsByClassName('microphone')[0]
const statusBox = document.getElementsByClassName('statusbox')[0]

let isRecording = false;

if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
	navigator.mediaDevices.getUserMedia({ audio: true })
		.then(function (stream) {
            const audioContext = new window.AudioContext;/*new (window.AudioContext || window.webkitAudioContext);*/

            const recorder = new Recorder(audioContext.createMediaStreamSource(stream), { numChannels: 2 });

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

UpdateAirspace();

