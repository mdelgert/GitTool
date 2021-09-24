using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using OfficeOpenXml;
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

            CreateReportPr(repo);
            CreateReportCommit(repo);
            CreateReportBugs(repo);
        }


        private static void CreateReportBugs(IRepository repo)
        {
            var cnt = 1;
            var file = new FileInfo(@"ReportBugs.xlsx");
            using var package = new ExcelPackage(file);
            var sheet = package.Workbook.Worksheets.Add("ReportBugs");

            foreach (var c in repo.Commits.ToList())
            {
                if (c.Message.IndexOf("bug", StringComparison.OrdinalIgnoreCase) < 0) continue;
                sheet.Cells["A" + cnt].Value = c.Committer.When;
                sheet.Cells["B" + cnt].Value = c.Author;
                sheet.Cells["C" + cnt].Value = c.MessageShort;
                cnt += 1;
            }

            // Save to file
            package.Save();
        }

        private static void CreateReportCommit(IRepository repo)
        {
            var cnt = 1;
            var file = new FileInfo(@"ReportCommits.xlsx");
            using var package = new ExcelPackage(file);
            var sheet = package.Workbook.Worksheets.Add("ReportCommits");

            foreach (var c in repo.Commits.ToList())
            {
                //if (c.Message.IndexOf("bug", StringComparison.OrdinalIgnoreCase) < 0) continue;
                sheet.Cells["A" + cnt].Value = c.Committer.When;
                sheet.Cells["B" + cnt].Value = c.Author;
                sheet.Cells["C" + cnt].Value = c.MessageShort;
                cnt += 1;
            }

            // Save to file
            package.Save();
        }

        private static void CreateReportPr(IRepository repo)
        {
            var cnt = 1;
            var file = new FileInfo(@"ReportPR.xlsx");
            using var package = new ExcelPackage(file);
            var sheet = package.Workbook.Worksheets.Add("ReportPR");

            // find the master branch in the repo
            var masterBranch = repo.Branches.Single(branch => branch.FriendlyName == "master");

            // Filter the branch's commits to ones that are merges
            var mergeList = masterBranch.Commits.Where(p => p.Parents.Count() >= 1);

            // Display the merge commits (pull requests) 
            foreach (var c in mergeList)
            {
                Console.WriteLine("{0}\t{1}", c.Author.Name, c.MessageShort);
                sheet.Cells["A" + cnt].Value = c.Committer.When;
                sheet.Cells["B" + cnt].Value = c.Author;
                sheet.Cells["C" + cnt].Value = c.MessageShort;
                cnt += 1;
            }

            // Save to file
            package.Save();
        }
    }

}
