﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace WebTestApp.Services
{
    /// <summary>
    /// <see cref="HttpClient"/> typed version of the service to access http://labs.bible.org/api_web_service.
    /// </summary>
    public class TorahService
    {
        private readonly TorahSettings _options;

        public HttpClient Client { get; }

        public TorahService(HttpClient httpClient, IOptions<TorahSettings> options )
        {
            _options = options.Value;
                        
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", nameof(TorahService));

            Client = httpClient;
        }

        /// <summary>
        /// Returns verses from the quotation.
        ///  Utilizes QqueryHelpers: https://rehansaeed.com/asp-net-core-hidden-gem-queryhelpers/
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<IList<TorahVerses>> GetVerses(string exp, CancellationToken token)
        {
            // create query parameters
            var args = new Dictionary<string, string>
            {
                { "type", "json" },
                { "passage", Uri.EscapeDataString(exp)}
            };

            var url = QueryHelpers.AddQueryString(_options.ApiUrl, args);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await Client.SendAsync(request, token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TorahVerses>>(result);
        }
    }
}
