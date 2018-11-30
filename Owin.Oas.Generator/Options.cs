using CommandLine;

namespace Owin.Oas.Generator
{
    public partial class Program
    {
        public class Options
        {
            [Option('a', "assembly", Required = true, HelpText = "Assembly that contains Startup class.")]
            public string Assembly { get; set; }

            [Option('s', "startup", Required = true, HelpText = "The Startup class type in the form \"fulltypename\".")]
            public string Startup { get; set; }

            [Option('o', "output", Required = false, HelpText = "File name for generated OAS.",Default = "swagger.json")]
            public string Output { get; set; }

            [Option('r', "route", Required = false, HelpText = "Route for OAS.", Default = "swagger/docs/v1")]
            public string Route { get; set; }

            [Option('b', "base", Required = false, HelpText = "Physical root of web api (helps to locate non-assembly dependencies).")]
            public string BaseDirectory { get; set; }

            [Option('p', "referencepaths", Required = false, HelpText = "Comma separated paths which will be searched when loading dependencies.")]
            public string ReferencePaths { get; set; }

            [Option('h', "headers", Required = false, HelpText = "HTTP headers in the form key:value|key=value")]
            public string Headers { get; set; }
        }
    }
}