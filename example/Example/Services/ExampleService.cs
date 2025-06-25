using Grpc.Core;

namespace Example.Services;

public class ExampleService : Example.ExampleBase
{
    private readonly ILogger<ExampleService> _logger;
    public ExampleService(ILogger<ExampleService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloResponse()
        {
            Message = "Hello " + request.Name
        });
    }

    public override Task<GetStatsResponse> GetStats(GetStatsRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetStatsResponse() { });
    }
}
