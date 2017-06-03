using System.Text;
using CommandLine;
using CommandLine.Text;
using LarchConsole;


namespace IIS.Models {
    public class Options {
        private Filter _value;
        private Filter _id;
        private Filter _name;
        private Filter _binding;
        private Filter _state;
        private Filter _ip;

        [ValueOption(0)]
        public string Value { get; set; }

        public Filter ValueFilter => _value ?? (_value = NewFilter(Value));

        // single filter
        [Option('i', "id", HelpText = "where id matchs VALUE\n")]
        public int? Id { get; set; }

        public Filter IdFilter => _id ?? (_id = NewFilter(Id?.ToString()));

        // multi filter
        [Option('n', "name", HelpText = "where site name matchs VALUE")]
        public string Name { get; set; }

        public Filter NameFilter => _name ?? (_name = NewFilter(Name));

        [Option('b', "binding", HelpText = "where binding matchs VALUE")]
        public string Binding { get; set; }

        public Filter BindingFilter => _binding ?? (_binding = NewFilter(Binding));

        [Option('s', "state", HelpText = "where state matchs VALUE")]
        public string State { get; set; }

        public Filter StateFilter => _state ?? (_state = NewFilter(State));

        [Option("ip", HelpText = "where IP matchs VALUE\n")]
        public string Ip { get; set; }

        public Filter IpFilter => _ip ?? (_ip = NewFilter(Ip));

        // search flags
        [Option("https", HelpText = "where sites using https://")]
        public bool Https { get; set; }

        [Option("sni", HelpText = "where https sites using Sni")]
        public bool Sni { get; set; }

        [Option("central", HelpText = "where https sites using CentralCertStore")]
        public bool CentralCertStore { get; set; }

        [Option("https-none", HelpText = "show all https sites without special ssl flags\n")]
        public bool HttpsNone { get; set; }

        // match and matchsity
        [Option('A', "and", HelpText = "use AND contition")]
        public bool UseAndCondition { get; set; }

        [Option('R', "regex", HelpText = "use regex for filter\n")]
        public bool Regex { get; set; }

        // views
        [Option("bindings", HelpText = "use bindings view for print")]
        public bool UseBindingsView { get; set; }

        [Option("sites", HelpText = "use sites view for print")]
        public bool UseSitesView { get; set; }

        // extra
        [Option("debug", HelpText = "enables debuging")]
        public bool Debug { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage() {
            var sb = new StringBuilder();
            sb.AppendLine(" Usage: iis [FILTER] [FLAGS]");
            //CommandLine.Text.
            var helpText = new HelpText() {
                Heading = sb.ToString(),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true,
                Copyright = "Copyright 2017 René Larch",
                MaximumDisplayWidth = int.MaxValue
            };

            helpText.AddOptions(this);
            helpText.AddPostOptionsLine(new StringBuilder()
                .AppendLine()
                .AppendLine("  list all")
                .AppendLine("   iis *")
                .AppendLine()
                .AppendLine("  find https sites with top level domain .com")
                .AppendLine("   iis --https -b *.com --and")
                .AppendLine()
                .AppendLine("  find all running sites where domain contains example")
                .AppendLine("   iis -s started -b *example* --and")
                .AppendLine()
                .AppendLine("  find all subdomains")
                .AppendLine("   iis -R -b ^(.*)\\.(.*\\..*)$ --bindings")
                .AppendLine()
                .AppendLine("  find all www domains and highlight them")
                .AppendLine("   iis -R -b ^(www)\\.(.*)")
                .AppendLine()
                .AppendLine("  change regex highlight color by double capture groups")
                .AppendLine("   TRY:  iis -R -b (www)  THEN TRY: iis -R -b ((www))")
                .ToString()
                );

            return helpText;
        }

        private Filter NewFilter(string value) {
            return new Filter(value,
                Regex
                    ? CampareType.Regex
                    : CampareType.WildCard,
                CompareMode.CaseIgnore
                ) {
                    OnEmptyMatchAll = true
                };
        }
    }
}