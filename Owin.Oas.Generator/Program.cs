using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Microsoft.Owin.Testing;
using Owin.Oas.Generator.Context;
using Owin.Oas.Generator.Exceptions;

namespace Owin.Oas.Generator
{
    public partial class Program
    {
        public static void Main(string[] args)
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

        private static Assembly Resolver(ResolveEventArgs args, IEnumerable<string> referencePaths)
        {
            if (args.Name.Contains(".resources"))
            {
                return null;
            }

            var assembly = AppDomain
                                .CurrentDomain
                                .GetAssemblies()
                                .FirstOrDefault(a => a.FullName == args.Name);

            if (assembly != null)
            {
                return assembly;
            }

            foreach (var path in referencePaths)
            {
                var filename = args.Name.Split(',')[0] + ".dll".ToLower();
                var asmFile = Path.Combine(path, filename);

                try
                {
#pragma warning disable S3885 // "Assembly.Load" should be used
                    return Assembly.LoadFrom(asmFile);
#pragma warning restore S3885 // "Assembly.Load" should be used
                }
                catch (Exception)
                {
                    // Not found, keep searching
                }
            }

            throw new UnableToLoadDependencyException(args.Name);
        }

        private static void DoWork(Options opts)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Resolver(args, opts.ReferencePaths.Split(','));

            var startupType = GetStartupType(opts.Assembly, opts.Startup);

            TestServer testServer = null;
            new UnitOfWork(AppDomain.CurrentDomain.SetupInformation.ApplicationBase)
                .DoWorkInContext
                (
                    opts.BaseDirectory,
                    () => testServer = BuildTestServer(startupType)
                );

            var headers = (!string.IsNullOrEmpty(opts.Headers) && opts.Headers.Contains(":"))
                                ? opts
                                    .Headers
                                    .Split('|')
                                    .ToDictionary(k => k.Split(':')[0], v => v.Split(':')[1])
                                : new Dictionary<string, string>();

            var wasSuccessful = GenerateOas(testServer, opts.Route, headers, out var content);
            if (wasSuccessful)
            {
                File.WriteAllText(opts.Output, content);
                Console.WriteLine("OAS written to {0}", opts.Output);
            }
            else
            {
                Console.WriteLine(content);
            }
        }

        private static Type GetStartupType(string assembly, string startupTypeName)
        {
            Assembly startupAssembly;
            Type startupType;

            try
            {
#pragma warning disable S3885 // "Assembly.Load" should be used
                startupAssembly = Assembly.LoadFrom(assembly);
#pragma warning restore S3885 // "Assembly.Load" should be used
            }
            catch (Exception ex)
            {
                throw new UnableToLoadStartupAssemblyException(assembly, ex);
            }

            try
            {
                startupType = startupAssembly.GetType(startupTypeName);
            }
            catch (Exception e)
            {
                throw new UnableToLoadStartupTypeException(startupTypeName, e);
            }

            return startupType;
        }

        private static TestServer BuildTestServer(Type startupType)
        {
            return (TestServer) typeof(TestServer)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Single(x => x.Name == "Create" && x.IsGenericMethod)
                        .MakeGenericMethod(startupType)
                        .Invoke(null, null);
        }

        private static bool GenerateOas(TestServer server, string route, IDictionary<string,string> headers, out string content)
        {
            var request = server.CreateRequest(route);

            foreach (var entry in headers)
            {
                request.AddHeader(entry.Key, entry.Value);
            }

            var response = request
                            .GetAsync()
                            .Result;

            content = response.Content.ReadAsStringAsync().Result;

            return response.IsSuccessStatusCode;
        }
    }
}
