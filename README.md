# ManageYourPhotosRepo

## Project back ground:

If you like take photos, you may backup your photo in different places/devices. Then how to make them consistent everywhere, may borther you just like me:

I like to manage my photo in my laptop, and process them with a desktop, and also make a backup in usb disk... when i pick up and delete photo in one place.  I hanve to keep updating in all place ,otherwise i have to handle the difference during the restore and merge.


## Solution:
The tool allow you make a selected photo list file by auto scan your working folder (can detecte the subfolder as well), then can clean up your other place based on the selected photo list file.

It is a command line app, can work in windows and mac
use (windows) "foto_list.exe --help" to get cmd help:
currently support models
1. "by default without any parameter, create baseline list mode, create photo list based on baseline folder (sub folder include) to your user desktop";
2. "Use parameter --path, switch to clean mode, clean the target folder based on foto list, should be: --path \"foto list full path\"";
3. "Use parameter --clean, switch to clean mode, clean the target folder based on foto list, should be: --clean \"foto list full path\"";
4. "Use parameter --compare, switch to compare mode, generate compare report for both baseline and target folder, should be: --compare \"foto list full path\"";


### All selected photo will be kept, the removed photo will be moved to a 'remove' sub folder. you can manully check them later.

## Supports and required
- Windows and MacOS system are supported
- .Net core 3.1x runtime needed
