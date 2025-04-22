using System.Net.Http.Json;

namespace MiniGram.Client.Utils;

public static class ResponseExtensions
{
    public static async Task<Response<T>> ToClientItemResponse<T>(this HttpResponseMessage httpResponse,
        CancellationToken cancellationToken = default)
    {
        if (httpResponse.IsSuccessStatusCode)
        {
            var data = await httpResponse.Content.ReadFromJsonAsync<T>(cancellationToken);
            return new ApiSuccessResponse<T>(data!, (int)httpResponse.StatusCode);
        }

        var error = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        return new ApiErrorResponse<T>(error, (int)httpResponse.StatusCode);
    }

    public static async Task<Response> ToClientItemResponse(this HttpResponseMessage httpResponse,
        CancellationToken cancellationToken = default)
    {
        if (httpResponse.IsSuccessStatusCode)
        {
            var data = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            return new ApiSuccessResponse(data, (int)httpResponse.StatusCode);
        }

        var error = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        return new ApiErrorResponse(error, (int)httpResponse.StatusCode);
    }
}

public class ApiSuccessResponse<T>(T item, int httpStatus) : Response<T>(item, httpStatus);
public class ApiSuccessResponse(string data, int statusCode) : Response(data, statusCode);
public class ApiErrorResponse(string error, int statusCode) : Response(error, statusCode);
public class ApiErrorResponse<T> : Response<T>
{
    public ApiErrorResponse(string error, int httpStatus) : base(error, httpStatus) { }
    public ApiErrorResponse(T errorDetails, int httpStatus) : base(errorDetails, httpStatus) { }
}
