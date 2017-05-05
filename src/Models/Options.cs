using System.Text;
using CommandLine;
using CommandLine.Text;


namespace IIS.Models {
    internal class Options {
        [ValueOption(0)]
        public string Value { get; set; }

        [Option('l', "list", HelpText = "List using wildcards or regex")]
        public bool List { get; set; }

        [Option('s', "site", HelpText = "Filter by site")]
        public bool Site { get; set; }

        [Option('i', "id", HelpText = "Filter by id")]
        public bool Id { get; set; }

        [Option('t', "state", HelpText = "Filter by state")]
        public bool State { get; set; }

        [Option("ip", HelpText = "Show all sites using this IP")]
        public bool Ip { get; set; }

        [Option("https", HelpText = "Show all sites using https://")]
        public bool Https { get; set; }

        [Option("sni", HelpText = "Show all https sites using Sni")]
        public bool Sni { get; set; }

        [Option("central", HelpText = "Show all https sites using CentralCertStore")]
        public bool CentralCertStore { get; set; }

        [Option("https-none", HelpText = "Show all https sites without special ssl flags")]
        public bool HttpsNone { get; set; }

        [Option('R', "regex", HelpText = "Use regex for filter")]
        public bool Regex { get; set; }

        [Option('d', "debug", HelpText = "Enables debuging")]
        public bool Debug { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage() {
            var sb = new StringBuilder();
            sb.AppendLine(" Usage: iis -l [FILTER] [FLAGS] PATTERN");

            var helpText = new HelpText() {
                Heading = sb.ToString(),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true,
                Copyright = "Copyright 2017 René Larch"
            };

            helpText.AddOptions(this);

            return helpText;
        }
    }
}