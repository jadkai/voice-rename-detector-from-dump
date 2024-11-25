# VoiceRenameDetectorFromDump

The input format for this program is a text file consisting of groups of text lines that provide details about the INFO records in an ESM or ESP file from Gamebryo or the Creation Engine. These records describe lines of dialogue.

An INFO consists of one or more "responses", which are individual lines of dialogue that correspond to a voice file for that audio. Each INFO belongs to a DIAL (topic) and to a QUST (quest). And this information is used when generating the file name for the audio file corresponding to each line of dialogue.

Each group of text lines has the following format:
```text
<1 INFO header line>
<1 or more response lines>
```

The INFO header line is formatted like this:
```text
<INFO editor ID> [INFO <form ID>] in <DIAL editor ID> [DIAL <form ID>] in <QUST editor ID> [QUST <form ID>]
```
where:

- `<INFO editor ID>` is an optional quoted value specifying the editor ID of the INFO record
- `[INFO <form ID>]` is required and is literal except for `<form ID>`, which is replaced with the form ID of the INFO record
- `<DIAL editor ID>` is an optional quoted value specifying the editor ID of the DIAL record that the INFO record belongs to
- `[DIAL <form ID>]` is required and is literal except for `<form ID>`, which is replaced with the form ID of the DIAL record
- `<QUST editor ID>` is an optional quoted value specifying the editor ID of the QUST record that the INFO record belongs to

Valid lines INFO header lines include, but are not limited to:
```text
[INFO 00123456] in [DIAL 00ABFC32] in [QUST 00543212]
"InfoEditorID" [INFO 00123456] in "DialEditorID" [DIAL 00ABFC32] in "QuestEditorID" [QUST 00543212]
[INFO 00123456] in "DialEditorID" [DIAL 00ABFC32] in "QuestEditorID" [QUST 00543212]
[INFO 00123456] in [DIAL 00ABFC32] in "QuestEditorID" [QUST 00543212]
```
After the INFO header are 1 or more lines of response text. These are formatted as:
```text
    <response number>: <response text>
```
where:
- `<response number>` is the response number for the dialogue line, but note that this is distinct from the response _index_, which is the order that the dialogue lines are said - response numbers can be out of order
- `<response text>` is the quoted text of the line of dialogue

## Valid example file
```text
[INFO 00123456] in [DIAL 00ABFC32] in [QUST 00543212]
    0: "Hi"
    1: "What are you doing?"

[INFO 00123457] in "DialEditorID" [DIAL 00ABFC32] in "QuestEditorID" [QUST 00543212]
    5: "I saw a mudcrab the other day."
    3: "Disgusting creatures"
    2: "Well, goodbye."
```