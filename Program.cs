using System.Linq;
using LibGit2Sharp;
using Serilog;

namespace GitTool
{
    internal class Program
    {
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo
                .File($"log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
            using var repo = new Repository($"C:\\_Projects\\FrostViper.Web\\.git");
            foreach (var c in repo.Commits.ToList())
            {
                Log.Information(c.Author + "," + c.Message);
            }
        }
    }
}
