using Pulumi;
using AzureAD = Pulumi.AzureAD;
using dotnet.Dtos;

return await Pulumi.Deployment.RunAsync(() =>
{
    new AzureSamlApplication("ClaimsXray", "ClaimsXray", new string[] { "urn:microsoft:adfs:claimsxray" }, new string[] { "https://adfshelp.microsoft.com/ClaimsXray/TokenResponse" });
});