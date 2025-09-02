using APIGateway.Zitadel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;
using System.Security.Claims;

namespace APIGateway.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static AuthenticationBuilder AddZitadelIntrospection(
        this AuthenticationBuilder builder,
        Action<ZitadelIntrospectionOptions>? configureOptions = default)
        => builder.AddZitadelIntrospection(ZitadelDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddZitadelIntrospection(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<ZitadelIntrospectionOptions>? configureOptions = default) =>
            builder
                .AddOAuth2Introspection(
                    authenticationScheme,
                    options =>
                    {
                        var zitadelOptions = new ZitadelIntrospectionOptions
                        {
                            ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                            AuthorizationHeaderStyle = BasicAuthenticationHeaderStyle.Rfc6749,
                            RoleClaimType = ZitadelClaimTypes.Role,
                        };
                        configureOptions?.Invoke(zitadelOptions);

                        // copy all properties from zitadel-options to options
                        options.Authority = zitadelOptions.Authority;
                        options.Events = zitadelOptions.Events;
                        options.AuthenticationType = zitadelOptions.AuthenticationType;
                        options.CacheDuration = zitadelOptions.CacheDuration;
                        options.ClientId = zitadelOptions.ClientId;
                        options.ClientSecret = zitadelOptions.ClientSecret;
                        options.DiscoveryPolicy = zitadelOptions.DiscoveryPolicy;
                        options.EnableCaching = zitadelOptions.EnableCaching;
                        options.IntrospectionEndpoint = zitadelOptions.IntrospectionEndpoint;
                        options.SaveToken = zitadelOptions.SaveToken;
                        options.TokenRetriever = zitadelOptions.TokenRetriever;
                        options.AuthorizationHeaderStyle = zitadelOptions.AuthorizationHeaderStyle;
                        options.CacheKeyGenerator = zitadelOptions.CacheKeyGenerator;
                        options.CacheKeyPrefix = zitadelOptions.CacheKeyPrefix;
                        options.ClientCredentialStyle = zitadelOptions.ClientCredentialStyle;
                        options.NameClaimType = zitadelOptions.NameClaimType;
                        options.RoleClaimType = zitadelOptions.RoleClaimType;
                        options.TokenTypeHint = zitadelOptions.TokenTypeHint;
                        options.SkipTokensWithDots = zitadelOptions.SkipTokensWithDots;
                        options.ClaimsIssuer = zitadelOptions.ClaimsIssuer;
                        options.EventsType = zitadelOptions.EventsType;
                        options.ForwardAuthenticate = zitadelOptions.ForwardAuthenticate;
                        options.ForwardChallenge = zitadelOptions.ForwardChallenge;
                        options.ForwardDefault = zitadelOptions.ForwardDefault;
                        options.ForwardForbid = zitadelOptions.ForwardForbid;
                        options.ForwardDefaultSelector = zitadelOptions.ForwardDefaultSelector;
                        options.ForwardSignIn = zitadelOptions.ForwardSignIn;
                        options.ForwardSignOut = zitadelOptions.ForwardSignOut;

                        options.Events.OnTokenValidated += context =>
                        {
                            var roleClaims = context.Principal?.Claims.Where(c => c.Type == context.Options.RoleClaimType);
                            if (roleClaims is null)
                            {
                                return Task.CompletedTask;
                            }

                            var roleIdentity = new ClaimsIdentity(
                                roleClaims
                                    .Select(
                                        c => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(
                                            c.Value))
                                    .OfType<Dictionary<string, Dictionary<string, string>>>()
                                    .SelectMany(
                                        dict => dict.SelectMany(
                                            role => role.Value
                                                .Select(
                                                    org => new Claim(
                                                        ZitadelClaimTypes.OrganizationRole(org.Key),
                                                        role.Key,
                                                        ClaimValueTypes.String,
                                                        context.Options.ClaimsIssuer))
                                                .Append(
                                                    new(
                                                        ClaimTypes.Role,
                                                        role.Key,
                                                        ClaimValueTypes.String,
                                                        context.Options.ClaimsIssuer)))));

                            context.Principal?.AddIdentity(roleIdentity);

                            return Task.CompletedTask;
                        };

                        if (zitadelOptions.JwtProfile == null)
                        {
                            return;
                        }

                        options.ClientId = null;
                        options.ClientSecret = null;
                        options.ClientCredentialStyle = ClientCredentialStyle.PostBody;
                        options.Events.OnUpdateClientAssertion += async context =>
                        {
                            var jwt = await zitadelOptions.JwtProfile.GetSignedJwtAsync(options.Authority);
                            context.ClientAssertion = new()
                            {
                                Type = ZitadelIntrospectionOptions.JwtBearerClientAssertionType,
                                Value = jwt,
                            };
                            context.ClientAssertionExpirationTime = DateTime.UtcNow.AddMinutes(55);
                        };
                    });
    }
}
