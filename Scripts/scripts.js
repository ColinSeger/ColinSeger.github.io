
class Content {
  constructor(file_path, target, context, poster) {
    this.file_path = file_path;
    this.title = target;
    this.context = context;
    this.poster = poster;
  }
}

let markdownFiles = [];
let htmlFiles = [];

// Function to load a markdown file
function load_markdown_file(file_path, target) {
  markdownFiles.push(new Content(file_path, target, ""));
}

async function load_file(file_path) {
  const response = await fetch(file_path);
  if (!response.ok) {
    throw new Error("Failed to load file");
    // return "Network response was not ok";
  }
  return response.text();
}

async function parse_markdown_file(file_path) {
  const file_contents = await load_file(file_path);
  const html = marked.parse(file_contents);
  if (typeof marked === "undefined") {
    throw new Error("marked library is not loaded");
    return file_contents;
  }
  return html;
}

async function parse_and_apply_id(file_path, target) {
  const html = parse_markdown_file(file_path);
  html.then((response) => {
    document.getElementById(target).innerHTML = response;
    sessionStorage.setItem(target, response);
    Prism.highlightAll();
  });
}

async function parse_html_and_apply_id(file_path, target) {
  const file = await fetch(file_path);

  if (!file.ok) {
    document.getElementById(target).innerHTML = "Failed to load text";
    return;
  }

  const html = await file.text();

  document.getElementById(target).innerHTML = html;
  sessionStorage.setItem(target, html);
  Prism.highlightAll();
}

async function load_defaults() {
  const content = [];
  content.push(
    new Content(
      "ProjectSummary/project_liminal.html",
      "Project Liminal",
      "Game Project 4",
      "Images/OptimizedPosters/ProjectLiminal.webp"
    ),
  );
  content.push(
    new Content(
      "ProjectSummary/in_bloom.html",
      "In Bloom",
      "Game Project 3",
      "Images/OptimizedPosters/in_bloom.webp"
    ),
  );
  content.push(
    new Content(
      "ProjectSummary/dark_descent.html",
      "Dark Descent",
      "Game Project 2",
      "Images/OptimizedPosters/dark_descent_poster.webp"
    ),
  );
  content.push(
    new Content(
      "ProjectSummary/grow_bot.html",
      "Grow Bot",
      "Game Project 1",
      "Images/OptimizedPosters/Growbot_Poster.webp"
    ),
  );

  const right_container = document.getElementById("right_side");
  const left_container = document.getElementById("left_side");
  right_container.innerHTML = "<h2>Highlighted Projects</h2>";

  const left = await load_file("Description/description.html");
  left_container.innerHTML = left;

  const template = document.getElementById("highlighted_project_template");

  for (const element of content) {
    const html = parse_markdown_file(element.file_path);
    const cloned = template.content.cloneNode(true);

    const title = cloned.querySelector(".title");
    const context = cloned.querySelector(".context");
    const text = cloned.querySelector(".text");
    const poster = cloned.querySelector(".project_poster");


    title.innerHTML = element.title;
    context.innerHTML = element.context;
    poster.src = element.poster;
    text.innerHTML = await html;

    right_container.appendChild(cloned);
  }
  document.getElementById("header").scrollIntoView();

  sessionStorage.setItem("left_side", '0');
  sessionStorage.setItem("right_side", '0');

}

// Ensure the DOM is fully loaded before executing the script
document.addEventListener("DOMContentLoaded", (event) => {
  // Call the function with the path to your markdown file
  if (sessionStorage.getItem("left_side") === '0' || !sessionStorage.getItem("left_side")) {
    load_defaults();

  } else {
    document.getElementById("left_side").innerHTML = sessionStorage.getItem("left_side");
    document.getElementById("right_side").innerHTML = sessionStorage.getItem("right_side");
    Prism.highlightAll();
  }

  markdownFiles.forEach((element) => {
    const html = parse_markdown_file(element.file_path);
    html.then((response) =>{
      document.getElementById(element.title).innerHTML = response;
    });
  });
});
// Push a dummy state when page loads
window.history.pushState({ page: 1 }, "", "");

// Listen for back button
window.addEventListener("popstate", function (event) {
    // Call your function
    load_defaults();
});




























//EXTERNAL MARKDOWN PARSER
//EXTERNAL MARKDOWN PARSER
//EXTERNAL MARKDOWN PARSER
//EXTERNAL MARKDOWN PARSER
//EXTERNAL MARKDOWN PARSER
//EXTERNAL MARKDOWN PARSER
