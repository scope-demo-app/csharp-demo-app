using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace csharp_demo_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            var gitInfo = ScopeAgent.Coverage.Utils.GitInfo.GetCurrent();
            Console.WriteLine();
            Console.WriteLine("From Git Folder:");
            Console.WriteLine("  Repository: {0}", gitInfo.Repository);
            Console.WriteLine("  Branch: {0}", gitInfo.Branch);
            Console.WriteLine("  Commit: {0}", gitInfo.Commit);
            Console.WriteLine("  SourceRoot: {0}", gitInfo.SourceRoot);
            Console.WriteLine();
            Console.WriteLine("From Agent Settings:");
            Console.WriteLine("  Repository: {0}", ScopeAgent.Agent.Settings.Repository);
            Console.WriteLine("  Branch: {0}", ScopeAgent.Agent.Settings.Branch);
            Console.WriteLine("  Commit: {0}", ScopeAgent.Agent.Settings.Commit);
            Console.WriteLine("  SourceRoot: {0}", ScopeAgent.Agent.Settings.SourceRoot);
            Console.WriteLine();
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}