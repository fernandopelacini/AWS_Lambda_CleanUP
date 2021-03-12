using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AWS_Clean_up_Lambda.Entities
{
    public static class Constants
    {
        public const string LambdaLastVersion = "$LATEST";
        public const string LambdaAll = "All";
        private static IConfiguration _config;
        public static IConfiguration config
        {
            set
            {
                _config = value;
            }
        }

        public static string AWSConfigFilePath
        {
            get
            {
                return _config.GetSection("Paths:EnvironmentConfigFile").Value.ToString();
            }
            //get { return ConfigurationManager.Appsettings[""] }
        }



    }
}
