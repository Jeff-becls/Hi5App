using Google.Protobuf.Collections;
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
            var response = new ExampleResponse { TotalCount = request.PageIndex * request.PageSize, Message = $"--From Server-- Current index from service is {request.PageIndex}" };

            return Task.FromResult(response);
        }

        public override async Task<ExampleResponse> StreamingFromClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            string totalIndex = string.Empty;
            await foreach (var message in requestStream.ReadAllAsync())
            {
                totalIndex += (message.PageIndex.ToString() + "|");

                if (message.PageIndex == 0)
                {
                    totalIndex += "Meet 0 to exiting...";
                    break;
                }
            }

            
            var response = new ExampleResponse { TotalCount = 0, Message = $"--From Server-- Current indexes from server is {totalIndex}" };

            return response;
        }

        public override async Task StreamingFromServer(ExampleRequest request, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            int startIndex = 1;
            startIndex = request.PageIndex;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var response = new ExampleResponse { TotalCount = startIndex, Message = $"--From Server-- Current index is {startIndex}" };
                startIndex++;

                await responseStream.WriteAsync(response);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return;
        }

        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                var response = new ExampleResponse { TotalCount = message.PageIndex, Message = $"--From Server-- Current index from response is {message.PageIndex}" };

                await responseStream.WriteAsync(response);
                await Task.Delay(TimeSpan.FromSeconds(1),context.CancellationToken);
            }
        }

        public override async Task SendingBigDataPackage(IAsyncStreamReader<BigDataRequest> requestStream, IServerStreamWriter<CommonResponse> responseStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var count = message.Content.Count;

                var response = new CommonResponse { Code = 0, Message = $"--From Server-- Current index from response is {message.DataType}, count: {count}" };

                await responseStream.WriteAsync(response);
                // await Task.Delay(10, context.CancellationToken);
            }

            return;
        }

    }
}
