using System;
using System.Collections.Generic;
using System.Linq;
using IIS.Controller;
using IIS.Models;
using LarchConsole;
using Microsoft.Web.Administration;


namespace IIS {
    public class Program {
        private IisController _iis;

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
            _iis = new IisController(iisManager);

            var iif = new IISFilter(iisManager, options.Regex) {
                FilterBindings = options.UseBindingsView
            };
            var filtered = GetFiltered(options, iif)?.ToList();

            if (iif.HasResult) {
                Display(filtered, options);
                return;
            }

            // handle shordhand for "iis VALUE" = "iis -b VALUE"
            if (!string.IsNullOrEmpty(options.Value)) {
                options.Binding = options.Value;
                iif.WhereBinding(options.ValueFilter);
                Display(iif.Result.ToList(), options);
            }

            // handle empty value
            if (string.IsNullOrEmpty(options.Value)) {
                Console.WriteLine(options.GetUsage());
                return;
            }
        }

        private void Display(List<FilterSite> filtered, Options options) {
            if (options.UseBindingsView) {
                _iis.Bindings(filtered, options);
            }
            else if (options.UseSitesView) {
                _iis.Sites(filtered, options);
            }
            else {
                _iis.List(filtered, options);
            }
        }

        private IEnumerable<FilterSite> GetFiltered(Options options, IISFilter iif) {
            using (new Watch("Filter")) {
                iif.Condition = options.UseAndCondition ? FilterCondition.And : FilterCondition.Or;

                // single filter
                if (options.Id != null) {
                    iif.WhereId(options.IdFilter);
                }

                // multi filter
                if (options.Name.HasValue()) {
                    iif.WhereName(options.NameFilter);
                }
                if (options.Binding.HasValue()) {
                    iif.WhereBinding(options.BindingFilter);
                }
                if (options.State.HasValue()) {
                    iif.WhereState(options.StateFilter);
                }
                if (options.Ip.HasValue()) {
                    iif.WhereIp(options.IpFilter);
                }

                // search flags
                if (options.Https) {
                    iif.WhereHttps();
                } else if (options.Sni) {
                    iif.WhereSni();
                } else if (options.CentralCertStore) {
                    iif.WhereCentralCertStore();
                } else if (options.HttpsNone) {
                    iif.WhereHttpsNone();
                }

                return iif.Result;
            }
        }
    }
}