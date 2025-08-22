elements = [];

document.addEventListener('DOMContentLoaded', (event) => {
    elements.forEach(element => {
        document_ready(element);
    });
});

function add_copy_element(element){
    elements.push(element);
}

function document_ready(element){
    var test = document.getElementById(element);
        
    if(test != null){
        test.onclick = copy_text(element);
    }
}

function copy_text(element){
    // Get the text field
    var copyText = document.getElementById(element);

    // Copy the text inside the text field
    navigator.clipboard.writeText(element.innerText);

    // Alert the copied text
    // alert("Copied the text: " + copyText.innerText);
}