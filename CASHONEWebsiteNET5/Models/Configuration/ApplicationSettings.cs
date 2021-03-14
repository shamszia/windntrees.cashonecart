using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models.Configuration
{
    public class ApplicationSettings
    {
        public string ApplicationID { get; set; }
        public string CurrencySymbol { get; set; }
        public string theme { get; set; }
        public int SessionTimeOut { get; set; }

        public string CheckOutID { get; set; }
        public string CheckOutPublicKey { get; set; }
        public string CheckOutPrivateKey { get; set; }
        public string CheckOutMode { get; set; }
        public string CurrencyCode { get; set; }

        public int CommandTimeout { get; set; }

        public bool EnableEmail { get; set; }
        public string EmailHost { get; set; }
        public int EmailHostPort { get; set; }
        public string EmailUser { get; set; }
        public string EmailUserPassword { get; set; }

        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string UnSubEmail { get; set; }
        public string Company { get; set; }
        public string CompanyTitle { get; set; }
        public string Website { get; set; }
        public string BreifDescription { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactCell { get; set; }
        public string BusinessHourLine1 { get; set; }
        public string BusinessHourLine2 { get; set; }
        public string MaxFileSizeInKB { get; set; }
        public string[] AllowedExtensions { get; set; }

        public string DefaultLocale { get; set; }
        public string[] SupportedLocales { get; set; }
        public string[] LocalePaths { get; set; }
        public string[] LocaleLibraries { get; set; }
        public string[] ViewLibraries { get; set; }
    }
}
