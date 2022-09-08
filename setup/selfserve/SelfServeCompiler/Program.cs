using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SelfServeCompiler
{
    class Program
    {
        private const string DATA_FILE = "lab-content-definition.json";

        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('s', "source", Required = true, HelpText = "Enter the source path of the MarkDown files")]
            public string SourcePath { get; set; }

            [Option('d', "destination", Required = true, HelpText = "Enter the destination path for the web pages")]
            public string DestinationPath { get; set; }


            [Option('i', "ImagesUrl", Default = "https://serverlessoh.azureedge.net/public/", Required = false, 
                HelpText = "Use this argument to set the url of the images mentioned at the Markdown files. Url will be replaced with a local path at the destination html file")]
            public string ImagesUrl { get; set; }

        }

        static void Main(string[] args)
        {
            Options options = null;
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       options = o;
                   })
                   .WithNotParsed(o =>
                   {
                       Console.WriteLine("Unable to parse command line arguments!");
                   });

            if (IsValidOptions(options))
            {
                //Read Content Definition File
                Console.WriteLine("Reading Content Definition File...");
                ContentDefinition contentDefinition = JsonSerializer.Deserialize<ContentDefinition>(
                    File.ReadAllText(Path.Combine(options.SourcePath, DATA_FILE)));
                StringBuilder htmlContent = new StringBuilder(ReadResource("SelfServeCompiler.index.html"));

                var md = new MarkdownSharp.Markdown();

                //Read Overview Content
                Console.WriteLine("Transforming Overview content...");
                var overviewText = md.Transform(
                    File.ReadAllText(
                        RenameHtml2Md(options.SourcePath, contentDefinition.OverviewUrl)));
                htmlContent.Replace("{{OverviewContent}}", overviewText);

                //Build Tab Pages
                StringBuilder tabPageContent = new StringBuilder();
                StringBuilder tabContent = new StringBuilder();
                foreach (var item in contentDefinition.ContentPackage.ContentItems.OrderBy(i => i.Order))
                {
                    Console.WriteLine($"Building page {item.Title}...");
                    string tabId = item.Url.Replace(".html", "");
                    //Create Tab page
                    tabPageContent.Append(
                        $"<li class=\"nav-item\"><a class=\"nav-link\" href=\"#{tabId}\"><strong>Challenge {item.Order}</strong><br />" +
                        $"{item.Title[(item.Title.IndexOf(':') + 2)..]}</a></li>");

                    //Create Tab Content
                    tabContent.Append(
                        $"<div id=\"{tabId}\" class=\"tab-pane\" role=\"tabpanel\" aria-labelledby=\"{tabId}\">" +
                        $"{md.Transform(File.ReadAllText(RenameHtml2Md(options.SourcePath, item.Url)).Replace(options.ImagesUrl, "images/"))}</div>");
                }

                //Constructing HTML page
                Console.WriteLine("Constructing the html page");
                htmlContent.Replace("{{TabPages}}", tabPageContent.ToString());
                htmlContent.Replace("{{TabContent}}", tabContent.ToString());
                File.WriteAllText(Path.Combine(options.DestinationPath, "index.html"), htmlContent.ToString());
                File.WriteAllText(Path.Combine(options.DestinationPath, "demo.css"), ReadResource("SelfServeCompiler.demo.css"));

                //Cpying Image files
                Console.WriteLine("Copying Images...");
                string imgFolderPath = Path.Combine(options.DestinationPath, "images");
                Directory.CreateDirectory(imgFolderPath);
                var images = Directory.GetFiles(Path.Combine(options.SourcePath, "images"));

                foreach (var img in images)
                {
                    Console.WriteLine($"Copying { Path.GetFileName(img) }...");
                    File.Copy(img, Path.Combine(imgFolderPath, Path.GetFileName(img)), true);
                }
            }
            else
            {
                Console.WriteLine("Invalid Options were provided");
            }
        }

        private static string RenameHtml2Md(String dir, string fileName)
        {
            return Path.Combine(dir, fileName.Replace(".html", ".md"));
        }

        static bool IsValidOptions(Options opt)
        {
            if (opt == null ||
                string.IsNullOrEmpty(opt.SourcePath) ||
                string.IsNullOrEmpty(opt.DestinationPath) ||
                !Directory.Exists(opt.SourcePath)) return false;

            if (!Directory.Exists(opt.DestinationPath))
                Directory.CreateDirectory(opt.DestinationPath);

            return true;
        }

        static string ReadResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(name));

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
