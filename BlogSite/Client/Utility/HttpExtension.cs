using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace BlogSite.Client.Utility
{
    public static class HttpExtension
    {
        public static async Task SetAuthorization(this HttpClient client, AuthenticationStateProvider authProvider)
        {
            var customAuthProvider = (CustomAuthStateProvider)authProvider;
            var token = await customAuthProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
