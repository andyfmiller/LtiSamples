using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using LtiLibrary.AspNet.Extensions;
using LtiLibrary.Core.Extensions;
using LtiLibrary.Core.OAuth;

namespace SimpleLti.Controllers
{
    public class OAuthHeaderAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        // Reference: http://www.asp.net/web-api/overview/security/authentication-filters

        public bool AllowMultiple => true;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var authorizationHeader = context.Request.Headers.Authorization;
            if (authorizationHeader == null)
            {
                return;
            }

            if (!authorizationHeader.Scheme.Equals(OAuthConstants.AuthScheme))
            {
                return;
            }

            if (string.IsNullOrEmpty(authorizationHeader.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", context.Request);
                return;
            }

            // Parse Authorization header
            var parameters = authorizationHeader.ParseOAuthAuthorizationHeader();
            var consumerKey = parameters[OAuthConstants.ConsumerKeyParameter];
            var nonce = parameters[OAuthConstants.NonceParameter];
            var signatureIn = parameters[OAuthConstants.SignatureParameter];
            var timestamp = parameters[OAuthConstants.TimestampParameter];

            if (string.IsNullOrEmpty(consumerKey))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing consumerKey", context.Request);
                return;
            }

            if (string.IsNullOrEmpty(signatureIn))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing signature", context.Request);
                return;
            }

            // Recalculate the signature
            var body = await context.Request.Content.ReadAsByteArrayAsync();
            var signature = GetSignature(context.Request, body, nonce, timestamp, consumerKey, "secret");

            if (!signatureIn.Equals(signature))
            {
                context.ErrorResult = new AuthenticationFailureResult("Signatures do not match", context.Request);
            }
            else
            {
                IPrincipal principal = new GenericPrincipal(new GenericIdentity(consumerKey), new string[] { });
                context.Principal = principal;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            // No challenge required
            return Task.FromResult(0);
        }


        private static string GetSignature(HttpRequestMessage request, byte[] body, string nonce, string timestamp, string consumerKey, string consumerSecret)
        {
            var parameters = new NameValueCollection();
            parameters.AddParameter(OAuthConstants.ConsumerKeyParameter, consumerKey);
            parameters.AddParameter(OAuthConstants.NonceParameter, nonce);
            parameters.AddParameter(OAuthConstants.SignatureMethodParameter, OAuthConstants.SignatureMethodHmacSha1);
            parameters.AddParameter(OAuthConstants.VersionParameter, OAuthConstants.Version10);
            parameters.AddParameter(OAuthConstants.TimestampParameter, timestamp);

            // Calculate the body hash
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var hash = sha1.ComputeHash(body);
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(request.Method.ToString(), request.RequestUri, parameters,
                consumerSecret);

            return signature;
        }
    }
}

public class AuthenticationFailureResult : IHttpActionResult
{
    public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
    {
        ReasonPhrase = reasonPhrase;
        Request = request;
    }

    public string ReasonPhrase { get; }

    public HttpRequestMessage Request { get; }

    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute());
    }

    private HttpResponseMessage Execute()
    {
        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        response.RequestMessage = Request;
        response.ReasonPhrase = ReasonPhrase;
        return response;
    }
}
