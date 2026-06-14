using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HospitalManagement.Client.Session;

namespace HospitalManagement.Client.Api;

/// <summary>
/// Central HTTP client wrapper for all API calls to the hospital server.
/// Automatically attaches the JWT bearer token from ClientSession.
/// </summary>
public static class ApiClient
{
    private static readonly HttpClient _http;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Server base URL — can be moved to app config later
    public const string BaseUrl = "https://localhost:7001";

    static ApiClient()
    {
        // Accept all SSL certificates in development (self-signed)
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _http = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };
    }

    /// <summary>
    /// Sends a POST request with a JSON body.
    /// Returns the deserialized response or default on failure.
    /// </summary>
    public static async Task<T?> PostAsync<T>(string path, object body)
    {
        AttachToken();
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(path, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"[{(int)response.StatusCode}] {responseBody}");

        return JsonSerializer.Deserialize<T>(responseBody, _jsonOptions);
    }

    /// <summary>
    /// Sends a GET request and returns the deserialized response.
    /// </summary>
    public static async Task<T?> GetAsync<T>(string path)
    {
        AttachToken();
        var response = await _http.GetAsync(path);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"[{(int)response.StatusCode}] {responseBody}");

        return JsonSerializer.Deserialize<T>(responseBody, _jsonOptions);
    }

    /// <summary>
    /// Sends a PUT request with a JSON body.
    /// </summary>
    public static async Task PutAsync(string path, object body)
    {
        AttachToken();
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PutAsync(path, content);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"[{(int)response.StatusCode}] {responseBody}");
        }
    }

    /// <summary>
    /// Sends a DELETE request.
    /// </summary>
    public static async Task DeleteAsync(string path)
    {
        AttachToken();
        var response = await _http.DeleteAsync(path);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"[{(int)response.StatusCode}] {responseBody}");
        }
    }

    /// <summary>
    /// Attaches the JWT bearer token to every outgoing request if the user is authenticated.
    /// </summary>
    private static void AttachToken()
    {
        if (ClientSession.IsAuthenticated)
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", ClientSession.Token);
        else
            _http.DefaultRequestHeaders.Authorization = null;
    }
}
