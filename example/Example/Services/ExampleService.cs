using Grpc.Core;

namespace Example.Services;

public class ExampleService : Example.ExampleBase
{
    private readonly ILogger<ExampleService> _logger;
    public ExampleService(ILogger<ExampleService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
