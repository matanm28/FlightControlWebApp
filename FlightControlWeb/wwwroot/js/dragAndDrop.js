let dropArea = document.getElementById('drop-area');
//dropArea.addEventListener('dragenter', dragEnter);
//dropArea.addEventListener('dragleave', dragLeave);
//dropArea.addEventListener('dragover', dragOver);
//dropArea.addEventListener('drop', dropFile);
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


//function dragEnter(e) {
//    preventDefaults(e);
//    highlight();
//}

//function dragLeave(e) {
//    preventDefaults(e);
//}

//function dragOver(e) {
//    preventDefaults(e);
//}

//function dropFile(e) {
//    preventDefaults(e);
//}

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
    ([...files]).forEach(uploadFile2);
}

function uploadFile(file) {
    let url = 'api/FlightPlan';
    let formData = new FormData();
    formData.append('file', file);
    console.log(formData);
    console.log(file);
    fetch(url,
            {
                method: 'POST',
                body: file,
            })
        .then((e) => { alert("flight plan sent"); })
        .catch((e) => { alert("flight plan send error"); });
}

function uploadFile2(file) {
    var url = 'api/FlightPlan';
    var xhr = new XMLHttpRequest();
    var formData = new FormData();
    xhr.open('POST', url);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.addEventListener('readystatechange',
        function(e) {
            if (xhr.readyState == 4 && (xhr.status == 200 || xhr.status == 201 || xhr.status == 202)) {
                console.log(e.srcElement.response);
            } else if (xhr.readyState == 4 && !(xhr.status != 200 && xhr.status!=201 && xhr.status!=202)) {
                alert(e.srcElement.response);
            }
        });
    formData.append('file', file);
    xhr.send(file);
}

function uploadFile3(file) {
    var fd = new FormData();
    fd.append('file', file);

    $.ajax({
        url: 'api/FlightPlan',
        type: 'post',
        data: fd,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response != 0) {
                $("#img").attr("src", response);
                $(".preview img").show(); // Display image element
            } else {
                alert('file not uploaded');
            }
        },
    });
    
}