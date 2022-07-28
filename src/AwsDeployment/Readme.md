# How to deploy

## Pre-Reqs

1. Install the CDK: `npm install -g aws-cdk@2.33.0`
2. Configure the AWS credentials. I suggest using a profile, and then set this environment variable: `AWS_PROFILE`

## And then...

From the root of the repository...

1. Build/Publish the dotnet application

```
dotnet publish -c Release src/CromulentBisgetti.DemoApp -o ./build
```

2. Run the CDK deployment

```
cdk deploy
```


3. Observe the outputs; should contain the function URL