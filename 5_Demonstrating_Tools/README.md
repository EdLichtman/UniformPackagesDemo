Tools:
https://github.com/aws/aws-extensions-for-dotnet-cli
* Amazon.Lambda.Tools adds commands to the dotnet cli to deploy AWS Lambda functions.
https://www.nuget.org/packages/Microsoft.Playwright.CLI
* Playwrite enables reliable end to end testing for modern web apps
https://www.nuget.org/packages/dotnet-outdated-tool
* A dotnet tool for updating project references
https://www.nuget.org/packages/Nuke.GlobalTool
* The AKEless Build System for C#/.NET


Here's the website
https://petstore.swagger.io/

dotnet tool restore;
dotnet refitter https://petstore.swagger.io/v2/swagger.json --namespace "Demo.Tools" -o PetStoreApi.cs


