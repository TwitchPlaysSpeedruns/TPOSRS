using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TRBot.Permissions;
using TRBot.Utilities;
using Microsoft.EntityFrameworkCore;

public class KbMouseCommand : BaseCommand
{
    private const string EXECUTABLE_PATH = "./Data/kbmousecontrol.py";
    
    public KbMouseCommand()
    {
    
    }
    
    public override void ExecuteCommand(EvtChatCommandArgs args)
    {
        string argStr = args.Command.ArgumentsAsString;
        
        //Run it in another task
        Task.Run(() => StartMouseProcess(argStr));
    }
    
    private async Task StartMouseProcess(string args)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            
            startInfo.FileName = (TRBotOSPlatform.CurrentOS == TRBotOSPlatform.OS.Windows) ? "python" : "python3";
            startInfo.Arguments = $"{EXECUTABLE_PATH} {args}";
            
            //Redirect standard error so the script outputs error messages
            startInfo.RedirectStandardError = true;
            
            using (Process p = Process.Start(startInfo))
            {
                await p.WaitForExitAsync();
                
                //QueueMessage($"EXIT CODE = {p.ExitCode}"); 
                
                //Read standard error
                string stdErr = await p.StandardError.ReadToEndAsync();
                
                //Show script output
                if (string.IsNullOrEmpty(stdErr) == false)
                {
                    QueueMessage(stdErr);
                }
            }
        }
        catch (Exception e)
        {
            QueueMessage($"Error controlling the mouse - {e.Message}");
            return;
        }
    }
}

return new KbMouseCommand();
