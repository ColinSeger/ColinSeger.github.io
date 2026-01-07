// const hljs = require('highlight.js');

class Content {
  constructor(file_path, target, context, expanded_content, left_side) {
    this.file_path = file_path;
    this.title = target;
    this.context = context;
    this.expanded_content = expanded_content;
    this.left_side = left_side;
  }
}

let markdownFiles = [];
let htmlFiles = [];

// Function to load a markdown file
function load_markdown_file(file_path, target) {
  markdownFiles.push(new Content(file_path, target, ""));
}

async function load_file(file_path){
  const response = await fetch(file_path);
  if (!response.ok) {
    throw new Error("Network response was not ok");
    // return "Network response was not ok";
  }
  return response.text();
}

async function parse_markdown_file(file_path){
  const file_contents = await load_file(file_path);
  const html = marked.parse(file_contents);
  if (typeof marked === "undefined") {
    throw new Error("marked library is not loaded");
    return file_contents;
  }
  return html;
}

async function parse_and_apply_id(file_path, target){
  const html = parse_markdown_file(file_path);
  html.then((response) =>{
    document.getElementById(target).innerHTML = response;
    hljs.highlightAll();
  });
  // hljs.highlightAll();
}

async function load_defaults(){
  const content = [];
  content.push(new Content("ProjectSummary/in_bloom.md", "In Bloom", "Game Project 3", 'ProjectPages/InBloom/InBloom.md', 'ProjectPages/InBloom/InBloomLeft.md'));
  content.push(new Content("ProjectSummary/dark_descent.md", "Dark Descent", "Game Project 2", 'ProjectPages/DarkDescent/DarkDescent.md', 'ProjectPages/DarkDescent/DDLeft.md'));
  content.push(new Content("ProjectSummary/grow_bot.md", "Grow Bot", "Game Project 1", 'ProjectPages/GrowBot/GrowBot.md', 'ProjectPages/GrowBot/GrowBotLeft.md'));

  const right_container = document.getElementById("right_side");
  const left_container = document.getElementById("left_side");
  right_container.innerHTML = "<h2>Highlighted Projects</h2>";

  const left = await parse_markdown_file("Description/description.md");
  left_container.innerHTML = left;

  const template = document.getElementById("template_test");

  for(const element of content){
    const html = await parse_markdown_file(element.file_path);
    const cloned = template.content.cloneNode(true);

    const title = cloned.querySelector(".title");
    const context = cloned.querySelector(".context");
    const text = cloned.querySelector(".text");
    const button = cloned.querySelector(".load_text");

    title.innerHTML = element.title;
    context.innerHTML = element.context;
    text.innerHTML = html;
    button.addEventListener("click", parse_and_apply_id.bind(null, element.expanded_content, 'right_side'));
    button.addEventListener("click", parse_and_apply_id.bind(null, element.left_side, 'left_side'));

    right_container.appendChild(cloned);
    hljs.highlightAll();
  };
  document.getElementById('header').scrollIntoView();
  // load_markdown_file('/Description/description.md', "personal_descriptor");
}

// Ensure the DOM is fully loaded before executing the script
document.addEventListener("DOMContentLoaded", (event) => {
  // Call the function with the path to your markdown file
  // load_defaults();
  markdownFiles.forEach((element) => {
    const html = parse_markdown_file(element.file_path);
    html.then((response) =>{
      document.getElementById(element.title).innerHTML = response;
    });
    hljs.highlightAll();
  });
});
