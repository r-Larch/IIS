using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using IIS.Models;
using LarchConsole;
using Microsoft.Web.Administration;


namespace IIS.Controller {
    public class IisController {
        private readonly ServerManager _iisManager;

        public IisController(ServerManager iisManager) {
            _iisManager = iisManager;
        }

        public void List(List<FilterSite> filterd, Options opt) {
            using (new Watch("print")) {
                var protocolLan = filterd.Any() ? filterd.Max(x => x.Bindings.Max(b => b.Protocol.Length)) : 0;

                ConsoleEx.PrintWithPaging(
                    list: filterd,
                    countAll: _iisManager.Sites.Count,
                    header: ConsoleWriter.CreateLine("   ID │"),
                    line: (x, i) => new ConsoleWriter()
                        .FormatLine("{id,5} │ {state,-9} {name}", p => p
                            .Add("id", opt.IdFilter.GetMatch(x.Id), opt.Id.HasValue)
                            .Add("state", opt.StateFilter.GetMatch(x.State), opt.State.HasValue())
                            .Add("name", opt.NameFilter.GetMatch(x.Name), opt.Name.HasValue())
                        )
                        .FormatLines("      │   {protocol,-" + protocolLan + "} {ssl} {ip}:{port,-4} {host}", x.Bindings, (b, parms) => parms
                            //.Add("schema", b.Schema.Name)
                            .Add("ip", opt.IpFilter.GetMatch(GetAddress(b.EndPoint)), opt.Ip.HasValue())
                            .Add("port", GetPort(b.EndPoint))
                            .Add("host", opt.BindingFilter.GetMatch(b.Host), opt.Binding.HasValue())
                            .Add("protocol", b.Protocol)
                            .Add("ssl", GetSslString(b.SslFlags()))
                        )
                        .WriteLine()
                    );
            }
        }

        public void Bindings(List<FilterSite> filterd, Options opt) {
            using (new Watch("print")) {
                var datas = filterd.SelectMany(x => x.Bindings.Select(b => new {
                    x.Id,
                    x.Name,
                    x.State,
                    b.Host,
                    b.EndPoint,
                    b.Protocol,
                    SslFlags = b.SslFlags()
                })).ToList();
                var nameLan = filterd.Any() ? filterd.Max(x => x.Name.Length) : 0;
                var protocolLan = datas.Any() ? datas.Max(x => x.Protocol.Length) : 0;

                ConsoleEx.PrintWithPaging(
                    list: datas,
                    countAll: _iisManager.Sites.Count,
                    header: ConsoleWriter.CreateLine("   ID │"),
                    line: (x, i) => new ConsoleWriter()
                        .FormatLine("{id,5} │ {name,-" + nameLan + "} {protocol,-" + protocolLan + "} {ssl} {ip}:{port,-4} {host}", p => p
                            .Add("id", opt.IdFilter.GetMatch(x.Id), opt.Id.HasValue)
                            .Add("name", opt.NameFilter.GetMatch(x.Name), opt.Name.HasValue())
                            .Add("ip", opt.IpFilter.GetMatch(GetAddress(x.EndPoint)), opt.Ip.HasValue())
                            .Add("port", GetPort(x.EndPoint))
                            .Add("host", opt.BindingFilter.GetMatch(x.Host), opt.Binding.HasValue())
                            .Add("protocol", x.Protocol)
                            .Add("ssl", GetSslString(x.SslFlags))
                        )
                    );
            }
        }

        public void Sites(List<FilterSite> filterd, Options opt) {
            using (new Watch("print")) {
                var nameLan = filterd.Any() ? filterd.Max(x => x.Name.Length) : 0;

                ConsoleEx.PrintWithPaging(
                    list: filterd,
                    countAll: _iisManager.Sites.Count,
                    header: ConsoleWriter.CreateLine("   ID │"),
                    line: (x, i) => new ConsoleWriter()
                        .FormatLine("{id,5} │ {state,-9} {name,-" + nameLan + "} bindings: {bindins-count}", p => p
                            .Add("id", opt.IdFilter.GetMatch(x.Id), opt.Id.HasValue)
                            .Add("state", opt.StateFilter.GetMatch(x.State), opt.State.HasValue())
                            .Add("name", opt.NameFilter.GetMatch(x.Name), opt.Name.HasValue())
                            .Add("bindins-count", x.Bindings.Count)
                        )
                    );
            }
        }


        #region private

        private string GetAddress(IPEndPoint endPoint) {
            if (endPoint == null) return "<null>";

            return Equals(endPoint.Address, IPAddress.Any) ? "*" : endPoint.Address.ToString();
        }

        private string GetPort(IPEndPoint endPoint) {
            return endPoint?.Port.ToString() ?? "<null>";
        }

        private string GetSslString(SslFlags sslFlags) {
            if (sslFlags == SslFlags.None) {
                return string.Empty;
            }

            var sb = new StringBuilder();
            if ((sslFlags & SslFlags.CentralCertStore) == SslFlags.CentralCertStore) {
                sb.Append(SslFlags.CentralCertStore);
            }
            if ((sslFlags & SslFlags.Sni) == SslFlags.Sni) {
                if (sb.Length != 0) {
                    sb.Append(" and ");
                }
                sb.Append(SslFlags.Sni);
            }

            return sb.ToString();
        }

        #endregion
    }
}