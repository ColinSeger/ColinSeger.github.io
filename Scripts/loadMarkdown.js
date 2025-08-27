// const hljs = require('highlight.js');

class Content{
    constructor(filePath, target){
        this.filePath = filePath;
        this.target = target;
    }
}


let markdownFiles = [];
let htmlFiles = [];

// Function to load a markdown file
function load_markdown_file(filePath, target) {
    markdownFiles.push(new Content(filePath, target));
};

// Function to load a html file
function load_html_file(filePath, target) {
    htmlFiles.push(new Content(filePath, target));
};

// Ensure the DOM is fully loaded before executing the script
document.addEventListener('DOMContentLoaded', (event) => {
    // Call the function with the path to your markdown file
    markdownFiles.forEach(element => {
        fetch(element.filePath)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(markdown => {
                if (typeof marked === 'undefined') {
                    throw new Error('marked library is not loaded');
                }
                // Convert markdown to HTML
                const html = marked.parse(markdown);
                // Insert the HTML into the content div
                document.getElementById(element.target).innerHTML = html;
                hljs.highlightAll();
            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
    });
    
    // htmlFiles.forEach(element => {
    //     fetch(element.filePath).then(response =>{
    //         if(response.ok){
    //             const html = response.innerHTML;
    //             document.getElementById(element.target).innerHTML = html;
    //         }
    //         else{
    //             throw new Error('Could not find file');
    //         }
    //     });
    // });
    // document.querySelectorAll('pre code').forEach((el) => {
    //     hljs.highlightElement(el);
    // });
});
