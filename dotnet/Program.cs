using dotnet.ResourceGroups;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create a new app registration, service principal, serviceprincialtokensigningcert, and a default set of claims (some of which don't work because I haven't figured out transforms)
    new AzureSamlApplication("ClaimsXray", "ClaimsXray", new string[] { "urn:microsoft:adfs:claimsxray" }, new string[] { "https://adfshelp.microsoft.com/ClaimsXray/TokenResponse" });
    // Create all the same as the above, but don't manage the claims
    new AzureSamlApplication("SampleApp", "SampleApp", new string[] { "http://www.thesneakyfox.com" }, new string[] { "https://www.thesneakyfox.com/login/acs" }, false);
});