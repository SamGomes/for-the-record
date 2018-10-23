

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


  EnableFileLoad : function () {
		function loadFileAsText(fileToLoad){
		  var fileReader = new FileReader();
		  fileReader.onload = function(fileLoadedEvent){
		  	  alert("-------->");
		      this.textFromFileLoaded = fileLoadedEvent.target.result;
		  };
		  fileReader.readAsText(fileToLoad, "UTF-8");
		}

		var button = document.createElement("BUTTON");
		button.text = "Load file";
		document.body.appendChild(button);

		var fileInput = document.getElementById("textID");
		if(fileInput==null){
			fileInput = document.createElement("INPUT");
			//text.style = "width:0; border:0;";
			fileInput.id = "textID";
			fileInput.type = "file";
			document.body.appendChild(fileInput);
		}

		fileInput.addEventListener("input", function(){
			alert(loadFileAsText(fileInput.files[0]));
		});
  },
  GetLoadedFileText : function(){
  	return this.textFromFileLoaded;
  }


};


mergeInto(LibraryManager.library, MyPlugin);


