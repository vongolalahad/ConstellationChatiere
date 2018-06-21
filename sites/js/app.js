/*
    When we click on the button, we call the function who
        check if the button is checked or not
        if checked:
            add class checked on on the button

*/

var switchers = document.getElementsByClassName("switch")
for (var i = 0; i < switchers.length; i++) {
    var switcher = switchers[i]
    var toggleClassChecked = function(e){
        e.stopPropagation()
        this.parentNode.classList.toggle("checked")
    }
    switcher.addEventListener("click",toggleClassChecked)
}