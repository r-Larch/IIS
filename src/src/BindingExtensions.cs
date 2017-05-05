using System;
using System.Runtime.InteropServices;
using Microsoft.Web.Administration;


namespace IIS {
    public static class BindingExtensions {
        public static SslFlags SslFlags(this Binding binding) {
            try {
                return (SslFlags) (long)binding.GetAttributeValue("sslFlags");
            } catch (COMException ex) {
                if (ex.ErrorCode == -2147023483)
                    return IIS.SslFlags.None;
                throw;
            }
        }
    }

    [Flags]
    public enum SslFlags {
        None = 0,
        Sni = 1,
        CentralCertStore = 2,
    }
}