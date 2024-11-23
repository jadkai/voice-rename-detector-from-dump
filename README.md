# VoiceRenameDetectorFromDump

## This info is outdated

The input file is now a custom file format containing only the relevant info, because xEdit dumps aren't a guaranteed stable format.

**Everything from here on is old**

This tool will parse a pair of xEdit dump files and attempt to match the corresponding INFO records from the two files.

This is to serve as the basis for a tool that will be able to take a set of original voice file paths from Oblivion and determine the correct voice file path for the corresponding voice file for the same dialogue in Skyrim, for the purposes of Skyblivion.

## Creating the dump files

Download [TES4Edit](https://www.nexusmods.com/oblivion/mods/11536) and put it into your Oblivion directory. Navigate to that directory in a terminal window and run the following command:

```text
.\Optional\TES4Dump64.exe -dg:DIAL .\Data\Oblivion.esm > OblivionDIALs.txt
```

Download [SSEEdit](https://www.nexusmods.com/skyrimspecialedition/mods/164) and put it into your Skyrim Special Edition directory. Navigate to that directory in a terminal window and run the following command:

```text
.\Optional\SSEDump64.exe -dg:DIAL .\Data\Skyblivion.esm > SkyblivionDIALs.txt
```

Obviously, this is assuming that you are matching Oblivion DIALs to Skyblivion DIALs.

