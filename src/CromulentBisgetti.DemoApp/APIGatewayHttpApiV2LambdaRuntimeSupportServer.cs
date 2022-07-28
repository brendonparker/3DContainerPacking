using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Hosting.Internal;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

public class CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer : LambdaRuntimeSupportServer
{
    /// <summary>
    /// Create instances
    /// </summary>
    /// <param name="serviceProvider">The IServiceProvider created for the ASP.NET Core application</param>
    public CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    /// <summary>
    /// Creates HandlerWrapper for processing events from API Gateway HTTP API
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    protected override HandlerWrapper CreateHandlerWrapper(IServiceProvider serviceProvider)
    {
        var handler = new APIGatewayHttpApiV2MinimalApi(serviceProvider).FunctionHandlerAsync;
        return HandlerWrapper.GetHandlerWrapper(handler, new DefaultLambdaJsonSerializer());
    }

    /// <summary>
    /// Create the APIGatewayHttpApiV2ProxyFunction passing in the ASP.NET Core application's IServiceProvider
    /// </summary>
    public class APIGatewayHttpApiV2MinimalApi : APIGatewayHttpApiV2ProxyFunction
    {
        /// <summary>
        /// Create instances
        /// </summary>
        /// <param name="serviceProvider">The IServiceProvider created for the ASP.NET Core application</param>
        public APIGatewayHttpApiV2MinimalApi(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            // This is the line I want/need to add
            RegisterResponseContentEncodingForContentType("font/woff2", ResponseContentEncoding.Base64);
        }
    }
}

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the ability to run the ASP.NET Core Lambda function in AWS Lambda. If the project is not running in Lambda 
    /// this method will do nothing allowing the normal Kestrel webserver to host the application.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="eventSource"></param>
    /// <returns></returns>
    public static IServiceCollection AddAWSLambdaHostingCustomized(this IServiceCollection services)
    {
        // Not running in Lambda so exit and let Kestrel be the web server
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
            return services;

        Utilities.EnsureLambdaServerRegistered(services, typeof(CustomAPIGatewayHttpApiV2LambdaRuntimeSupportServer));
        return services;
    }
}