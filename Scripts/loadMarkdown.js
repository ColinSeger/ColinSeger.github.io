
class Content{
    constructor(filePath, target){
        this.filePath = filePath;
        this.target = target;
    }
}


let contents = [];

// Function to load a markdown file
function load_markdown_file(filePath, target) {
    contents.push(new Content(filePath, target));
};

// function load_all_recent(){
//     var fs = require('fs');
//     var files = fs.readdirSync('/RecentProjects');
    
//     files.forEach(file => {
//         fetch(file)
//             .then(response => {
//                 if (!response.ok) {
//                     throw new Error('Network response was not ok');
//                 }
//                 return response.text();
//             })
//             .then(markdown => {
//                 if (typeof marked === 'undefined') {
//                     throw new Error('marked library is not loaded');
//                 }
//                 // Convert markdown to HTML
//                 const html = marked.parse(markdown);

//                 var placement = document.getElementById("right_side");

//                 // create a new div element
//                 const newDiv = document.createElement("div");

//                 newDiv.appendChild(html);

//                 document.body.insertBefore(newDiv, placement);
//             })
//             .catch(error => {
//                 console.error('There has been a problem with your fetch operation:', error);
//             });
//     });
// };

// Ensure the DOM is fully loaded before executing the script
document.addEventListener('DOMContentLoaded', (event) => {
    // Call the function with the path to your markdown file
    contents.forEach(element => {
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
            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
    });
    // load_all_recent();
});
