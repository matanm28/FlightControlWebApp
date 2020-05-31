let dropArea = document.getElementById('drop-area');
['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false);
});

['dragenter', 'dragover'].forEach(eventName => {
    dropArea.addEventListener(eventName, highlight, false);
});
['dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, unhighlight, false);
});
dropArea.addEventListener('drop', handleDrop, false);

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

function highlight() {
    dropArea.classList.add('highlight');
}

function unhighlight() {
    dropArea.classList.remove('highlight');
}

function handleDrop(e) {
    let dt = e.dataTransfer;
    let files = dt.files;

    handleFiles(files);
}

function handleFiles(files) {
    ([...files]).forEach(uploadFile);
}

function uploadFile(file) {
    var url = 'api/FlightPlan';
    var xhr = new XMLHttpRequest();
    var formData = new FormData;
    xhr.open('POST', url);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.addEventListener('readystatechange',
        function(e) {
            if (xhr.readyState == 4 && (xhr.status == 200 || xhr.status == 201 || xhr.status == 202)) {
                console.log(e.srcElement.response);
            } else if (xhr.readyState == 4 && (xhr.status != 200 && xhr.status!=201 && xhr.status!=202)) {
                alert(e.srcElement.response);
            }
        });
    formData.append('file', file);
    xhr.send(file);
}
