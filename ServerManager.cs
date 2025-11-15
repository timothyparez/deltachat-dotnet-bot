public class ServerManager
{
    public Process ServerProcess { get; private set; }
    public async Task StartAsync()
    {
        var startInfo = new ProcessStartInfo("deltachat-rpc-server")
        {
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };

        ServerProcess = new Process() { StartInfo = startInfo };        
        ServerProcess.Start();
        
        await Task.Delay(2000);
    }
}