// The initialize function is required for all add-ins.
Office.initialize = function (reason) {

    // Checks for the DOM to load using the jQuery ready function.
    $(document).ready(function () {

        // Execute sendFile when submit is clicked
        $('#sendFile').click(function () {
            sendFile();
        });

        $('#getFile').click(function () {
            getFile();
        });

        // Update status
        updateStatus("Ready to send file.");
    });
}

// Create a function for writing to the status div.
function updateStatus(message) {
    var statusInfo = $('#status');
    statusInfo.innerHTML += message + "<br/>";
}

// Get all of the content from a PowerPoint or Word document in 100-KB chunks of text.
function sendFile() {

    Office.context.document.getFileAsync(
        "compressed",
        { sliceSize: 500000 },
        function (result) {

            if (result.status == Office.AsyncResultStatus.Succeeded) {

                // Get the File object from the result.
                var myFile = result.value;
                var state = {
                    file: myFile,
                    counter: 0,
                    sliceCount: myFile.sliceCount
                };

                updateStatus("Getting file of " + myFile.size + " bytes");
                getSlice(state);
            }
            else {
                updateStatus(result.status);
            }
        }
    );
}

// Get a slice from the file and then call sendSlice.
function getSlice(state) {

    state.file.getSliceAsync(
        state.counter,
        function (result) {
            if (result.status == Office.AsyncResultStatus.Succeeded) {

                updateStatus("Sending piece " + (state.counter + 1) + " of " + state.sliceCount);

                sendSlice(result.value, state);

            }
            else {
                updateStatus(result.status);
            }
        }
    );
}

function sendSlice(slice, state) {

    var data = slice.data;

    // If the slice contains data, create an HTTP request.
    if (data) {

        // Encode the slice data, a byte array, as a Base64 string.
        // NOTE: The implementation of myEncodeBase64(input) function isn't
        // included with this example. For information about Base64 encoding with
        // JavaScript, see https://developer.mozilla.org/docs/Web/JavaScript/Base64_encoding_and_decoding.

        // my tip : Files in the "compressed" format will return a byte array that can be transformed to a base64-encoded string if required.
        // data is a byte array
        //var fileData = myEncodeBase64(data);
        //var fileData = base64EncArr(data);
        var fileData = base64js.fromByteArray(data);

        //alert(fileData); // "QmFzZSA2NCDigJQgTW96aWxsYSBEZXZlbG9wZXIgTmV0d29yaw=="


        // Create a new HTTP request. You need to send the request
        // to a webpage that can receive a post.
        // 비동기 처리
        var request = new XMLHttpRequest();

        // Create a handler function to update the status
        // when the request has been sent.
        request.onreadystatechange = function () {

            if (request.readyState == 4) {

                updateStatus("Sent " + slice.size + " bytes.");
                state.counter++;

                if (state.counter < state.sliceCount) {
                    getSlice(state);
                }
                else {
                    closeFile(state);
                }
            }
        }

        request.open("POST", "api/Receiver/");
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.setRequestHeader("Slice-Number", slice.index);

        // Send the file as the body of an HTTP POST
        // request to the web server.
        request.send(JSON.stringify({ "value": fileData, "fileFullName": Office.context.document.url, "index": slice.index }));
        //request.send(fileData);
    }
}

function closeFile(state) {
    // Close the file when you're done with it.
    state.file.closeAsync(function (result) {

        // If the result returns as a success, the
        // file has been successfully closed.
        if (result.status == "succeeded") {
            updateStatus("File closed.");
        }
        else {
            updateStatus("File couldn't be closed.");
        }
    });
}


function getFile() {

    // 배열로 선언
    docdataSlices = [];

    var request = new XMLHttpRequest();

    request.onreadystatechange = function () {

        if (request.readyState == 4) {
            if (request.status == 200) {
                //서버 응답 결과에 따라 알맞은 작업 처리
                var resultJSON = JSON.parse(request.responseText);
                for (i = 0; i < resultJSON.length; i++) {
                    docdataSlices[i] = resultJSON[i].Value;
                }
                onGotAllSlices(docdataSlices);
            }
            else
            {
                updateStatus("문제 발생:" + request.status);
            }
        }
    }

    request.open("GET", "api/Receiver/");
    request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    request.send();
}

function onGotAllSlices(docdataSlices) {
    let docdata = [];
    for (let i = 0; i < docdataSlices.length; i++) {
        docdata = docdata.concat(docdataSlices[i]);
    }
    const byteArray = new Uint8Array(docdata);
    const blob = new Blob([byteArray]);
    const a = document.createElement('a');
    url = URL.createObjectURL(blob);

    a.href = url;
    a.download = 'WordTest.docx';
    document.body.appendChild(a);
    a.click();
    setTimeout(function () {
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }, 0);



}
