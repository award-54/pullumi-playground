using System.Collections.Generic;
using System.Text.Json;
using Pulumi.AzureAD.Inputs;
using Pulumi.AzureAD;

namespace dotnet.Dtos
{
    internal class AzureSamlApplication
    {

        public Application ApplicationResource;
        public ServicePrincipal ServicePrincipalResource;
        public ServicePrincipalTokenSigningCertificate ServicePrincipalSigningCert;
        public ClaimsMappingPolicy ClaimsMappingPolicyResource;
        public ServicePrincipalClaimsMappingPolicyAssignment ClaimsPolicyMappingResource;

        public AzureSamlApplication(string appName, string displayName, string[] identifiers, string[] redirectUris)
        {
            ApplicationResource = new Application(appName, new()
            {
                DisplayName = displayName,
                IdentifierUris = identifiers,
                Web = new ApplicationWebArgs()
                {
                    RedirectUris = redirectUris,
                }
            });

            ServicePrincipalResource = new ServicePrincipal(appName, new()
            {
                ApplicationId = ApplicationResource.ApplicationId,
                FeatureTags = new[]
                {
                    new ServicePrincipalFeatureTagArgs
                    {
                        CustomSingleSignOn = true,
                        Enterprise = true,
                    },
                },
                PreferredSingleSignOnMode = "saml"
            });

            ServicePrincipalSigningCert = new ServicePrincipalTokenSigningCertificate(appName, new()
            {
                ServicePrincipalId = ServicePrincipalResource.Id,
            });

            ClaimsMappingPolicyResource = new ClaimsMappingPolicy(appName, new()
            {
                DisplayName = displayName,
                Definitions = new[]
                {
                    JsonSerializer.Serialize(new Dictionary<string, object?>
                    {
                        ["ClaimsMappingPolicy"] = new Dictionary<string, object?>
                        {
                            ["ClaimsSchema"] = new[]
                            {
                                new Dictionary<string, object?>
                                {
                                    ["ID"] = "employeeid",
                                    ["JwtClaimType"] = "name",
                                    ["SamlClaimType"] = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                                    ["Source"] = "user",
                                },
                                new Dictionary<string, object?>
                                {
                                    ["ID"] = "tenantcountry",
                                    ["JwtClaimType"] = "country",
                                    ["SamlClaimType"] = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country",
                                    ["Source"] = "company",
                                },
                                new Dictionary<string, object?>
                                {
                                    ["ID"] = "TOS",
                                    ["DisplayName"] = "",

                                }
                            },
                            ["ClaimsTransformation"] = new[]
                            {
                                new Dictionary<string, object?>
                                {
                                    ["ID"] = "customClaim",
                                    ["TransformationMethod"] = "CreateStringClaim",
                                    ["InputParameters"] = new []
                                    {
                                        new Dictionary<string, object?>
                                        {
                                            ["ID"] = "value",
                                            ["DataType"] = "string",
                                            ["Value"] = "test"
                                        }
                                    },
                                    ["OutputClaims"] = new []
                                    {
                                        new Dictionary<string, object?>
                                        {
                                            ["ClaimTypeReferenceId"] = "TOS",
                                            ["TransformationClaimType"] = "createdClaim"
                                        }
                                    }
                                }
                            },
                            ["IncludeBasicClaimSet"] = "false",
                            ["Version"] = 1,
                        },
                    }),
                },
            });
            ClaimsPolicyMappingResource = new ServicePrincipalClaimsMappingPolicyAssignment(appName, new()
            {
                ClaimsMappingPolicyId = ClaimsMappingPolicyResource.Id,
                ServicePrincipalId = ServicePrincipalResource.Id,
            });
        }
    }
}
