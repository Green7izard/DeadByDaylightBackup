# DeadByDaylightBackup
Simple Backup tool for Dead By Daylight Save Files


## Synopsis

A very simple low tech solution for backing up your DeadByDaylight Save files

## Installation

Download the Latest release from https://github.com/Green7izard/DeadByDaylightBackup/tree/master/Releases
Extract it anywhere!
Execute it!

## How To Add your Saves

In the upper left textbox you can add the path where your save file exists. Most of the times it will be:
C:\Steam\userdata\*YOUR PROFILE NUMBER*\381210\remote\ProfileSaves

If you dont know the exact location: Push the "Search For Steam Paths" button and it will find them for you!

## The Save File locations

On the left the program will show a list of found save files (Not the exact location).
The delete button will remove the backup function for those files

## Choosing a Backup location

In the "DeadByDaylightBackup.exe.config" file you will find a line that sais:
<add key="BackupLocation" value="E:\\DeadByDaylight"/>
change this to any path where you like to store your backups. (Do mind the double \ )

## Backing up your saves

The top right button will backup all found saves.
The backups are stored with the Steam Profile number and the last mutation date of the save file.

All backups are shown in the lower left.

To delete a older backup: Push the delete button. The backupfile will be deleted.
To Restore a backup: Push the restore button. !!ATTENTION!! Steam Cloud Save might try to overwrite your file again. Disable it temporary to prevent problems!

## Error Handling

The application Will show exceptions in a popup
It will also log them to the log folder.
For logging it uses NLog, which is brutal overkill for a application of this size.
You can change the NLog config to your own liking!

## License

MIT License
Copyright (c) 2017 Bas van Summeren
