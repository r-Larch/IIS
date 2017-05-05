using System;
using IIS.Controller;
using IIS.Models;
using LarchConsole;
using Microsoft.Web.Administration;


namespace IIS {
    public class Program {
        public static void Main(string[] args) {
            try {
                var p = new Program();
                var options = new Options();

                var parser = new CommandLine.Parser(settings => settings.CaseSensitive = true);
                if (parser.ParseArguments(args, options)) {
                    p.Run(options);
                } else {
                    // print help
                    Console.WriteLine(options.GetUsage());
                }

                if (options.Debug) {
                    Watch.PrintTasks();
                }
            } catch (Exception e) {
                ConsoleEx.PrintException(e.Message, e);
                Environment.ExitCode = e.HResult == 0 ? -1 : e.HResult;
            }
        }


        private void Run(Options options) {
            var iisManager = new ServerManager();
            var iis = new IisController(iisManager);

            var filter = new Filter(options.Value,
                options.Regex
                    ? CampareType.Regex
                    : CampareType.WildCard,
                CompareMode.CaseIgnore
                );

            var filterProp = GetFilterProp(options);

            // list
            if (
                options.List ||
                options.Site ||
                options.Id ||
                options.State ||
                options.Ip ||
                options.Https ||
                options.Sni ||
                options.CentralCertStore ||
                options.HttpsNone
                ) {
                filter.OnEmptyMatchAll = true;
                iis.List(filter, filterProp);
                return;
            }

            // handle empty value
            if (string.IsNullOrEmpty(options.Value)) {
                Console.WriteLine(options.GetUsage());
                return;
            }
        }

        private FilterProp GetFilterProp(Options options) {
            if (options.Site) {
                return FilterProp.Name;
            }

            if (options.Id) {
                return FilterProp.Id;
            }

            if (options.State) {
                return FilterProp.State;
            }

            if (options.Ip) {
                return FilterProp.Ip;
            }

            if (options.Https) {
                return FilterProp.Https;
            }

            if (options.Sni) {
                return FilterProp.Sni;
            }

            if (options.CentralCertStore) {
                return FilterProp.CentralCertStore;
            }

            if (options.HttpsNone) {
                return FilterProp.HttpsNone;
            }

            return FilterProp.Binding;
        }
    }
}