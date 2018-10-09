var MyPlugin = {

  EnableCopyToClipboard: function (textValue) {
  	var string = Pointer_stringify(textValue);
  	// window.alert(string);
	document.onclick = function() { 
		var copyText = document.getElementById("textID"); 
		if(copyText==null){
			var text = document.createElement("INPUT");
			text.value = string;
			text.style = "width:0; border:0;";
			text.id = "textID";
			document.body.appendChild(text);
			copyText = document.getElementById("textID");
		}
	 
		copyText.select(); 
		document.execCommand("copy"); 
	};
  },

};


mergeInto(LibraryManager.library, MyPlugin);