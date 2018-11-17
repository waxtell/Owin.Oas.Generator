using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLine;
using Microsoft.Owin.Testing;

namespace Owin.Oas.Generator
{
    public class Program
    {
        public class Options
        {
            [Option('a', "assembly", Required = true, HelpText = "Assembly that contains Startup class.")]
            public string Assembly { get; set; }

            [Option('s', "startup", Required = false, HelpText = "Startup class name.")]
            public string Startup { get; set; }

            [Option('o', "output", Required = false, HelpText = "File name for generated OAS.",
                Default = "swagger.json")]
            public string Output { get; set; }

            [Option('r', "route", Required = false, HelpText = "Route for OAS.", Default = "docs/v1")]
            public string Route { get; set; }
        }

        static void Main(string[] args)
        {
            Parser
                .Default
                .ParseArguments<Options>(args)
                .WithParsed(DoWork)
                .WithNotParsed(ShowErrors);
        }

        private static void ShowErrors(IEnumerable<Error> errs)
        {
            foreach (var error in errs)
            {
                Console.WriteLine(error);
            }
        }

        private static void DoWork(Options opts)
        {
            var startupType = GetStartupType(opts.Assembly, opts.Startup);
            var testServer = BuildTestServer(startupType);

            var wasSuccessful = GenerateOas(testServer, opts.Route, out var content);

            if (wasSuccessful)
            {
                File.WriteAllText(opts.Output, content);
            }
            else
            {
                Console.WriteLine(content);
            }
        }

        private static Type GetStartupType(string assembly, string startupClass)
        {
            var startupAssembly = Assembly.LoadFrom(assembly);
            var startupType = startupAssembly.GetType(startupClass);

            return startupType;
        }

        private static TestServer BuildTestServer(Type startupType)
        {
            var createMethod = typeof(TestServer).GetMethod("Create");
            var genericCreateMethod = createMethod?.MakeGenericMethod(startupType);

            return (TestServer) genericCreateMethod?.Invoke(null, null);
        }

        private static bool GenerateOas(TestServer server, string route, out string content)
        {
            var response = server.HttpClient.GetAsync(route).Result;

            content = response.Content.ReadAsStringAsync().Result;

            return response.IsSuccessStatusCode;
        }
    }
}
