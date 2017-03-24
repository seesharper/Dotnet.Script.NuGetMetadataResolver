namespace Dotnet.Script.NuGetMetadataResolver
{
    using System;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A class that is capable of running a command.
    /// </summary>
    public class CommandRunner : ICommandRunner
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRunner"/> class.
        /// </summary>
        /// <param name="loggerFactory"></param>
        public CommandRunner(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<CommandRunner>();
        }

        public void Execute(string commandPath, string arguments)
        {
            logger.LogInformation($"Executing {commandPath} {arguments}");
            var startInformation = CreateProcessStartInfo(commandPath, arguments);
            var process = CreateProcess(startInformation);            
            RunAndWait(process);

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"The command {commandPath} {arguments} failed to execute");
            }
        }

        private static ProcessStartInfo CreateProcessStartInfo(string commandPath, string arguments)
        {
            var startInformation = new ProcessStartInfo(commandPath);
            startInformation.CreateNoWindow = true;
            startInformation.Arguments = arguments;
            startInformation.RedirectStandardOutput = true;
            startInformation.RedirectStandardError = true;
            startInformation.UseShellExecute = false;
            return startInformation;
        }

        private Process CreateProcess(ProcessStartInfo startInformation)
        {
            var process = new Process();
            process.StartInfo = startInformation;
            process.ErrorDataReceived += (s, a) => logger.LogError(a.Data);
            process.OutputDataReceived += (s, a) => logger.LogInformation(a.Data);
            return process;
        }

        private static void RunAndWait(Process process)
        {
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
    }
}