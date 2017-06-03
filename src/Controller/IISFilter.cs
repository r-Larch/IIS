using System;
using System.Collections.Generic;
using System.Linq;
using LarchConsole;
using Microsoft.Web.Administration;


// ReSharper disable once InconsistentNaming

namespace IIS.Controller {
    public class IISFilter {
        private readonly bool _useRegex;
        private readonly List<FilterSite> _all;
        private IEnumerable<FilterSite> _result;

        public IISFilter(ServerManager iisManager, bool useRegex = false) {
            _useRegex = useRegex;

            _all = iisManager.Sites.Select(x => new FilterSite() {
                Id = x.Id,
                Name = x.Name,
                State = x.State,
                Bindings = x.Bindings.ToList(),
            }).ToList();
        }

        public FilterCondition Condition { get; set; }
        public IEnumerable<FilterSite> Result => _result;
        public bool HasResult { get; private set; }
        public bool FilterBindings { get; set; }

        private Filter NewFilter(string value) {
            return new Filter(value,
                _useRegex
                    ? CampareType.Regex
                    : CampareType.WildCard,
                CompareMode.CaseIgnore
                ) {
                    //OnEmptyMatchAll = true
                };
        }

        private void Merge(Func<IEnumerable<FilterSite>, IEnumerable<FilterSite>> func) {
            if (Condition == FilterCondition.And) {
                if (_result == null) {
                    _result = _all;
                }
                _result = func(_result).ToList();
            } else {
                _result = Combine(_result, func(_all)).Distinct().ToList();
            }
            HasResult = true;
        }

        private IEnumerable<FilterSite> Combine(IEnumerable<FilterSite> list1, IEnumerable<FilterSite> list2) {
            if (list1 != null)
                foreach (var site in list1)
                    yield return site;

            if (list2 != null)
                foreach (var site in list2)
                    yield return site;
        }

        public void WhereBinding(Filter filter) {
            if (FilterBindings) {
                Merge(sites => {
                    return sites.Select(site => {
                        site.Bindings = site.Bindings.Where(x => filter.IsMatch(x.Host)).ToList();
                        return site;
                    });
                });
            } else {
                Merge(d => d.Where(x => x.Bindings.Any(_ => filter.IsMatch(_.Host))));
            }
        }

        public void WhereName(Filter filter) {
            Merge(d => d.Where(x => filter.IsMatch(x.Name)));
        }

        public void WhereId(Filter filter) {
            Merge(d => d.Where(x => filter.IsMatch(x.Id.ToString())));
        }

        public void WhereState(Filter filter) {
            Merge(d => d.Where(x => filter.IsMatch(x.State.ToString())));
        }

        public void WhereIp(Filter filter) {
            Merge(d => d.Where(x => x.Bindings.Any(_ => filter.IsMatch(_.EndPoint?.Address.ToString()))));
        }

        public void WhereHttps() {
            Merge(d => d.Where(x => x.Bindings.Any(_ => _.Protocol == "https")));
        }

        public void WhereSni() {
            Merge(d => d.Where(x => x.Bindings.Any(_ => (_.SslFlags() & SslFlags.Sni) == SslFlags.Sni)));
        }

        public void WhereCentralCertStore() {
            Merge(d => d.Where(x => x.Bindings.Any(_ => (_.SslFlags() & SslFlags.CentralCertStore) == SslFlags.CentralCertStore)));
        }

        public void WhereHttpsNone() {
            Merge(d => d.Where(x => x.Bindings.Any(_ => _.Protocol == "https" && _.SslFlags() == SslFlags.None)));
        }
    }


    public class FilterSite {
        public long Id { get; set; }
        public string Name { get; set; }
        public ObjectState State { get; set; }
        public List<Binding> Bindings { get; set; }
    }


    public enum FilterCondition {
        Or = 0,
        And = 1
    }
}