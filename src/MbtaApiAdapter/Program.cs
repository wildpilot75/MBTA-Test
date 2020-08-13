namespace MbtaApiAdapter
{
    using MbtaCommon;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    class Program
    {
        

        static void Main()
        {
            var bootstrap = new Bootstrap();
            bootstrap.Run();
        }
    }
}
