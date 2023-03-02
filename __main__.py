"""An Azure RM Python Pulumi program"""

from pulumi_azure_native import storage
from pulumi_azure_native import resources
import pulumi_azuread as azuread

import os

notification_emails = os.environ.get("NOTIFICATION_EMAIL").split(",")

claims_x_ray = azuread.Application("ClaimsXRay",
    display_name="ClaimsXray",
    feature_tags=[azuread.ApplicationFeatureTagArgs()],
    identifier_uris=["urn:microsoft:adfs:claimsxray"],
    web=azuread.ApplicationWebArgs(
        redirect_uris=["https://adfshelp.microsoft.com/ClaimsXray/TokenResponse"],
    ))

claims_x_ray_sp = azuread.ServicePrincipal("ClaimsXRaySP",
    application_id=claims_x_ray.application_id,
    feature_tags=[azuread.ServicePrincipalFeatureTagArgs(
        enterprise=True,
        gallery=False,
        custom_single_sign_on=True
    )], 
    preferred_single_sign_on_mode="saml",
    notification_email_addresses=notification_emails)

claims_x_ray_signing_certificate = azuread.ServicePrincipalTokenSigningCertificate("ClaimsXRayCertificate",
    service_principal_id=claims_x_ray_sp.id)