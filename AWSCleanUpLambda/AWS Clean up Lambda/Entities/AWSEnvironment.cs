using System;
using System.Collections.Generic;
using System.Text;

namespace AWS_Clean_up_Lambda.Entities
{
    public class AWSEnvironment
    {
        public int AIR { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string LogGroupNamePrefix { get; set; }
    }
}
