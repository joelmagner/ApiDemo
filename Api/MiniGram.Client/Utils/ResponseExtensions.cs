using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace MiniGram.Client.Utils
{
    public static class ResponseExtensions
    {
        // for API endpoint use
        public static IResult ToClientItemResponse<T>(this Response<T> response)
        {
            return response.IsSuccess
                ? Results.Json(response.Item, statusCode: response.HttpStatus)
                : Results.Problem(detail: response.Error, statusCode: response.HttpStatus);
        }

        // for client use
        public static async Task<Response<T>> ToClientItemResponse<T>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                var data = await httpResponse.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
                return new ApiSuccessResponse<T>(data!, (int)httpResponse.StatusCode);
            }

            var error = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            return new ApiErrorResponse<T>(error, (int)httpResponse.StatusCode);
        }
    }

    public class ApiSuccessResponse<T>(T item, int httpStatus) : Response<T>(item, httpStatus);
    public class ApiErrorResponse<T>(string error, int httpStatus) : Response<T>(error, httpStatus);
}
