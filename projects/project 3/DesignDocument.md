# APPLICATION NAME
Currently I only accept this to be used in conjunction with this class as I have left the dialog window for the upload to cloud button, this is meant to be more of a proof off concept for using one the google cloud apis. For now this app just tells you what the top tag is for the image you submitted which I guess is cool?

## System Design 
Tested on an Android 7 device, though should worn on system 5 through 8.

## Usage
First thing that you do is pick A photo you want to upload to the cloud, you can use your gallery or camera
After selection of the picture, you may upload it to the cloud or pick a new picture
	Upon attempting to upload the picture you will be hit with an alert window noting that you have limited free uses of google cloud which you can accet or deny
If you accept the dialog window there will be a text field telling you that you submitted a great what ever the top tag was
	If you hit Thanks! then the buttons will close and you will be left with the picture, text and the option to restart with the restart button
	If you instead hit That's not what it is then a textfield will open so you can type what tag it was meant to be
		it will try to find your tag in the tags from the cloud but besides that you are left with the option to restart
