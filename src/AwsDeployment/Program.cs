
using Amazon.CDK;
using Lambda = Amazon.CDK.AWS.Lambda;

var app = new App();

var stack = new BinPackingStack(app, "BinPacking", new StackProps
{
    StackName = "BinPacking",
    Env = new Amazon.CDK.Environment
    {
        Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
        Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
    }
});

app.Synth();

public class BinPackingStack : Stack
{
    public BinPackingStack(Constructs.Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
    {
        var fn = new Lambda.Function(this, "BinPackingApp", new Lambda.FunctionProps
        {
            Runtime = Lambda.Runtime.DOTNET_6,
            FunctionName = "BinPacking",
            MemorySize = 1024,
            // dotnet publish -c Release src/CromulentBisgetti.DemoApp -o ./build
            Code = Lambda.Code.FromAsset("./build"),
            Handler = "CromulentBisgetti.DemoApp"
        });

        var fnUrl = fn.AddFunctionUrl(new Lambda.FunctionUrlProps
        {
            AuthType = Lambda.FunctionUrlAuthType.NONE,
        });

        new CfnOutput(this, "LambdaURL", new CfnOutputProps { ExportName = "LambdaURL", Value = fnUrl.Url });
    }
}