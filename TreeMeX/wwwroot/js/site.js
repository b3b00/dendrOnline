// // // Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// // // for details on configuring this project to bundle and minify static web assets.
// //
// // // Write your JavaScript code.
window.createNote = function(parent) {
    let name = window.prompt("nouvelle note",parent);
    if (name !== null && name !== undefined) {
        window.location = "/Index?handler=newNote&parent=" + parent + "&new=" + name;
    }
}

window.showTab = function (tabName) {

    var tabs = ["hierarchy","preview","editor"];
    for (let i = 0; i < tabs.length; i++) {
        let tab = tabs[i];
        var t = document.getElementById(tab);
        if (tab == tabName) {
            t.style.display = "block";
        }
        else {
            t.style.display = "none";
        }

    }
}