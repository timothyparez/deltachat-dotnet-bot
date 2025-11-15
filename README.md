# Proof-of-Concept Delta.Chat bot for .NET

This code is nothing but a proof-of-concept, it's not production ready!  
Currently all the classes are written manually because I can't get the  
JSON-RPC generator to work for any language other than `rust` and `typescript`

# How to use

  - Create a new Delta.Chat account
  - Create a backup for that account, this will give you a `.tar` file
  - Copy the `.tar` file to your device
  - Call `client.ImportBackup(<accountid>, <path-to-your-tar-file>)`
  - Implement `IMessageProcessor` to process messages

## CommandProcessor Example

![screenshot of Delta.chat interacting with the bot](screenshot.png)
 


