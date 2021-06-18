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
