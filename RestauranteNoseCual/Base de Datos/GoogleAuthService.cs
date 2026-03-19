using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace RestauranteNoseCual.Base_de_Datos
{
    public class AutentificacionGoogle
    {
        private const string ClientId = "134372964689-avv8eggc7ejfpqddhmvb7046upopi255.apps.googleusercontent.com";
        private const string ClientSecret = "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";
        private const string RedirectUri = "com.googleusercontent.apps.134372964689-avv8eggc7ejfpqddhmvb7046upopi255:/oauth2redirect";
        private const string Scope = "openid email profile";
        private const string AuthUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenUrl = "https://oauth2.googleapis.com/token";

        public class GoogleUser
        {
            public string? Id { get; set; }
            public string? Email { get; set; }
            public string? Name { get; set; }
            public string? Picture { get; set; }
            public string? IdToken { get; set; }
            public string? AccessToken { get; set; }
        }

        public async Task<GoogleUser?> SignInAsync()
        {
            try
            {
                var codeVerifier = GenerateCodeVerifier();
                var codeChallenge = GenerateCodeChallenge(codeVerifier);
                var state = Guid.NewGuid().ToString("N");

                var authUri = new Uri(
                    $"{AuthUrl}" +
                    $"?client_id={Uri.EscapeDataString(ClientId)}" +
                    $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                    $"&response_type=code" +
                    $"&scope={Uri.EscapeDataString(Scope)}" +
                    $"&state={state}" +
                    $"&code_challenge={codeChallenge}" +
                    $"&code_challenge_method=S256"
                );

                var result = await WebAuthenticator.Default.AuthenticateAsync(
                    new WebAuthenticatorOptions
                    {
                        Url = new Uri(authUri.ToString()),
                        CallbackUrl = new Uri(RedirectUri),
                        PrefersEphemeralWebBrowserSession = true
                    }
                );

                if (!result.Properties.TryGetValue("code", out var code))
                    throw new Exception("No se recibió el código de autorización.");

                var tokens = await ExchangeCodeForTokensAsync(code, codeVerifier);
                if (tokens is null) return null;

                var user = ParseIdToken(tokens.Value.idToken);
                if (user is not null)
                {
                    user.IdToken = tokens.Value.idToken;
                    user.AccessToken = tokens.Value.accessToken;
                }
                return user;
            }
            catch (TaskCanceledException) { return null; }
            catch (Exception ex)
            {
                Console.WriteLine($"[GoogleAuth] Error: {ex.Message}");
                throw;
            }
        }

        private static async Task<(string idToken, string accessToken)?> ExchangeCodeForTokensAsync(
            string code, string codeVerifier)
        {
            using var http = new HttpClient();
            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = ClientId,
                ["client_secret"] = ClientSecret,
                ["redirect_uri"] = RedirectUri,
                ["grant_type"] = "authorization_code",
                ["code_verifier"] = codeVerifier
            });

            var response = await http.PostAsync(TokenUrl, body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[GoogleAuth] Token error: {json}");
                return null;
            }

            using var doc = JsonDocument.Parse(json);
            var idToken = doc.RootElement.GetProperty("id_token").GetString() ?? "";
            var accessToken = doc.RootElement.GetProperty("access_token").GetString() ?? "";
            return (idToken, accessToken);
        }

        private static GoogleUser? ParseIdToken(string idToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(idToken);
                return new GoogleUser
                {
                    Id = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                    Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                    Name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                    Picture = jwt.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
                };
            }
            catch { return null; }
        }

        private static string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(string verifier)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.ASCII.GetBytes(verifier));
            return Base64UrlEncode(bytes);
        }

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
