using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace IBISA.Helper
{
    public static class AppSettingsHelper
    {
        public static string BlockchainUri = ConfigurationManager.AppSettings["BlockchainUri"] ?? "http://localhost:4545";
        public static int BlockchainUriTimeoutInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["BlockchainUriTimeoutInSeconds"] ?? "60");
    }

    public static class EnumHelper
    {
        public enum ImportParameters
        {
            AgreementNumber,
            Zone,
            Location,
            PlotID,
            Crop,
            MaxPayout,
            TargetContribution,
            CurrentContribution,
            DateOfAgreement,
            UserDisplayName
        }

        public enum TransactionType
        {
            Contribution = 1,
            Indemnification = 2
        }
    }

    public static class ExceptionHelper
    {
        public static void Log(string message, Exception ex = null)
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Log";
                Directory.CreateDirectory(filePath);
                File.AppendAllText(filePath + "\\Error.log", ex != null ? string.Join("\r\n", DateTime.Now, message, ex.Message, ex.InnerException, ex.StackTrace, Environment.NewLine) : string.Join("\r\n", DateTime.Now, message, Environment.NewLine));
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}