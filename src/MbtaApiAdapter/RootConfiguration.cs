using System;
using System.Collections.Generic;
using System.Text;

namespace MbtaApiAdapter
{
    public class RootConfiguration
    {
        public string ApiUrl { get; set; }

        public string PublisherAddress { get; set; }

        public string ApiKey { get; set; }

        public string Schedule { get; set; }

        public string Trip { get; set; }

        public int PageSize { get; set; }
    }
}
