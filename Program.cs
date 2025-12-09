

using M = Spectre.Console.Markup;

Write(new FigletText("NG-Bot v1.0").LeftJustified().Color(Color.Purple));
MarkupLineInterpolated($"[grey]{new string('-', System.Console.WindowWidth)}[/]");

var serverManager = new ServerManager();
var messageProcessor = new CommandProcessor();
await serverManager.StartAsync();

var client = new DeltaRpcClient(serverManager.ServerProcess.StandardInput, serverManager.ServerProcess.StandardOutput);
var systemInfo = client.GetSystemInfo();

MarkupLineInterpolated($"[yellow]{M.Escape(systemInfo.DeltaChatCoreVersion)}[/] / x{M.Escape(systemInfo.Architecture ?? "")} / {M.Escape(systemInfo.CpuCount.ToString())} Threads");

int currentAccountId;
var accountIds = client.GetAllAccountIds();

MarkupLine("[red]Ignoreing all accounts but the first one for now[/]");
if (accountIds.Length == 0)
{
    MarkupLine("[red]No accounts have been configured yet, adding now[/]");
    currentAccountId = client.AddAccount();

    //The idea here is that you use the official client to create an account
    //and then backup that account to a .tar file. Next you call ImportBackup
    //with the path to that tar file in order to import the account
    //You only have to do this once.    

    //client.ImportBackup(currentAccountId, "enter path to your backup here", "");    
}
else
{
    currentAccountId = accountIds[0];
    MarkupLineInterpolated($"[yellow]Using existing account: {currentAccountId}[/]");
}

client.ConfigureAccountSetting(currentAccountId, AccountSetting.BOT, "1");
var isAccountConfigured = client.IsAccountConfigured(currentAccountId);


//If you did not import from a backup the account may not yet be configured
//It's possible to configure the e-mail address and password, but that won't
//actually work since this method does not take into account the public/private keys
//It's added here more as an example
// if (!isAccountConfigured)
// {

//     MarkupLine("[red]The current account has not yet been configured[/]");
//     client.ConfigureAccountSetting(currentAccountId, AccountSetting.MAIL_ADDRESS, "");
//     client.ConfigureAccountSetting(currentAccountId, AccountSetting.MAIL_PASSWORD, "");

//     MarkupLineInterpolated($"[yellow]Mail Adddress and Password set[/]");
//     var result = client.ConfigureAccount(currentAccountId);
    

//     isAccountConfigured = client.IsAccountConfigured(currentAccountId);
//     if (isAccountConfigured)
//     {
//         MarkupLine("[yellow]The account is now configured[/]");
//     }
//     else
//     {
//         //If your account still isn't configured at this point
//         //There's not much else we can do, so we just exit.
//         MarkupLine("[red]Failed to configure the acccount.[/], Press any key to exit.");        
//         System.Console.ReadKey();
//         Environment.Exit(1);
//     }
// }

client.StartIOForAccount(currentAccountId);

while (true)
{
    try
    {
        var deltaEvent = client.GetNextEvent();

        switch (deltaEvent.@event.kind.ToString())
        {
            case EventType.INCOMING_MSG:
                await ProcessMessage(deltaEvent.@event.msgId);
                break;
            case EventType.INFO:
                MarkupLineInterpolated($"{M.Escape(DateTime.Now.ToShortTimeString())}: {M.Escape(deltaEvent.@event.msg.ToString())}");
                break;
            case EventType.WARNING:
                MarkupLineInterpolated($"[red3_1]{M.Escape(DateTime.Now.ToShortTimeString())}[/]: {M.Escape(deltaEvent.@event.msg.ToString())}");
                break;
            case EventType.ERROR:
                MarkupLineInterpolated($"[red1]{M.Escape(DateTime.Now.ToShortTimeString())} [/]: {M.Escape(deltaEvent.@event.msg.ToString())}");
                break;
            case EventType.CONNECTIVITY_CHANGED:
                MarkupLineInterpolated($"[darkgoldenrod]Connectivity changed[/]");
                break;
            case EventType.IMAP_INBOX_IDLE:
                MarkupLineInterpolated($"[lightsteelblue1]IMAP indbox idle[/]");
                break;
            case EventType.INCOMING_MSG_BUNCH:
                MarkupLine("Incoming message bunch");
                break;
            default:
                MarkupLineInterpolated($"[lightgoldenrod1]Unknown event kind[/]: {M.Escape(deltaEvent.@event.kind.ToString())}");
                break;
        }
    }
    catch (Exception ex)
    {
        MarkupLineInterpolated($"Unhandled exception while handling event: {ex.ToString()}");
    }
}

async Task ProcessMessage(int messageId)
{
    var chatMessage = client.GetMessage(currentAccountId, messageId);
    MessageData? chatMessageResponseData = null;

    if (chatMessage.FromId != SpecialContactId.Self && !chatMessage.IsBot && !chatMessage.IsInfo)
    {
        chatMessageResponseData = await messageProcessor.ProcessChatMessageAsync(chatMessage);
    }
    else
    {
        MarkupLineInterpolated($"[fuchsia]{M.Escape(chatMessage.Text)}[/]");        
    }

    if (chatMessageResponseData != null)
    {
        client.SendMessage(currentAccountId, chatMessage.ChatId, chatMessageResponseData);
    }
    else
    {
        MarkupLine("[red]Did not get a valid [yellow]MessageData[/] instance from the message processor[/]");
    }
}







