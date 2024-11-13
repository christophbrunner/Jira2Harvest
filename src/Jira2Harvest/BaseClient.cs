using System.Net;
using System.Text.Json;
using BSW.Core.Extension;
using Microsoft.Extensions.Configuration;

namespace Jira2Harvest;

public abstract class BaseClient
{
    protected BaseClient(IConfiguration configuration)
    {
        string proxyAddress = configuration.GetConfiguration<string>("ProxyAddress", string.Empty);

        if (!string.IsNullOrEmpty(proxyAddress))

        {
            WebProxy proxyObject = new(proxyAddress, true)
            {
                UseDefaultCredentials = true
            };

            var handler = new HttpClientHandler { Proxy = proxyObject };

            HttpClient = new HttpClient(handler);
        }
        else
        {
            HttpClient = new HttpClient();
        }

        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    protected readonly HttpClient HttpClient;
    protected readonly JsonSerializerOptions JsonSerializerOptions;

    protected async Task<T> Get<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);

        if (data == null)
        {
            throw new Exception($"Failed to deserialize response from GET {url}");
        }

        return data;
    }

    protected async Task<T> Patch<T>(string url)
    {
        var response = await HttpClient.PatchAsync(url, new StringContent(string.Empty));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);

        if (data == null)
        {
            throw new Exception($"Failed to deserialize response from PATCH {url}");
        }

        return data;
    }

    protected async Task<T> Post<T>(string url)
    {
        var response = await HttpClient.PostAsync(url, new StringContent(string.Empty));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);

        if (data == null)
        {
            throw new Exception($"Failed to deserialize response from DELETE {url}");
        }

        return data;
    }

    protected async Task<T> Delete<T>(string url)
    {
        var response = await HttpClient.DeleteAsync(url);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);

        if (data == null)
        {
            throw new Exception($"Failed to deserialize response from DELETE {url}");
        }

        return data;
    }
}