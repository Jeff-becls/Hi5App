using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hi5App
{
    public class GrpcConnectionFactory
    {
        public static readonly string SocketPath = Path.Combine(Path.GetTempPath(), "socket.tmp");

        public static GrpcChannel CreateUdsChannel()
        {
            var udsEndPoint = new UnixDomainSocketEndPoint(SocketPath);
            var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
            var socketsHttpHandler = new SocketsHttpHandler
            {
                ConnectCallback = connectionFactory.ConnectAsync
            };

            return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                HttpHandler = socketsHttpHandler
            });
        }

        public static GrpcChannel CreateUdsChannel(MethodConfig methodConfig)
        {
            var udsEndPoint = new UnixDomainSocketEndPoint(SocketPath);
            var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
            var socketsHttpHandler = new SocketsHttpHandler
            {
                ConnectCallback = connectionFactory.ConnectAsync
            };

            var options = new GrpcChannelOptions
            {
                HttpHandler = socketsHttpHandler
            };
            options.ServiceConfig = new ServiceConfig();
            options.ServiceConfig.MethodConfigs.Add(methodConfig);

            return GrpcChannel.ForAddress("http://localhost", options);
        }
    }
}
