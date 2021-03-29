using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcShopService
{
    public class StreamShopService :StreamShoper.StreamShoperBase
    {
        private readonly ILogger<StreamShopService> _logger;
        public StreamShopService(ILogger<StreamShopService> logger)
        {
            _logger = logger;
        }

        public override Task<ExampleResponse> UnaryCall(ExampleRequest request, ServerCallContext context)
        {
            return base.UnaryCall(request, context);
        }

        public override Task<ExampleResponse> StreamingFromClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            return base.StreamingFromClient(requestStream, context);
        }

        public override async Task StreamingFromServer(ExampleRequest request, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            int startIndex = 1;
            startIndex = request.PageIndex;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var response = new ExampleResponse { TotalCount = startIndex, Message = $"Current index is {startIndex}" };
                startIndex++;

                await responseStream.WriteAsync(response);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

        }

        public override Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            return base.StreamingBothWays(requestStream, responseStream, context);
        }
    }
}
