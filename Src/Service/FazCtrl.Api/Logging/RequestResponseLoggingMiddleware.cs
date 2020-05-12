using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IO;


namespace FazCtrl.Api.Logging
{

    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly Action<RequestProfilerModel> _requestResponseHandler;
        private const int ReadChunkBufferLength = 4096;

        public RequestResponseLoggingMiddleware(RequestDelegate next, Action<RequestProfilerModel> requestResponseHandler)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _requestResponseHandler = requestResponseHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            var model = new RequestProfilerModel
            {
                RequestTime = new DateTimeOffset(),
                Context = context,
                Request = await FormatRequest(context)
            };

            var stopWatch = Stopwatch.StartNew();
            Stream originalBody = context.Response.Body;

            using (MemoryStream newResponseBody = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = newResponseBody;

                await _next(context);

                stopWatch.Stop();

                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalBody);

                newResponseBody.Seek(0, SeekOrigin.Begin);
                model.Response = FormatResponse(context, newResponseBody, stopWatch);
                model.ResponseTime = new DateTimeOffset();
                _requestResponseHandler(model);
            }
        }

        private string FormatResponse(HttpContext context, MemoryStream newResponseBody, Stopwatch stopWatch)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            return $"Http Response Information: {Environment.NewLine}" +
                    $"Schema:{request.Scheme} {Environment.NewLine}" +
                    $"Host: {request.Host} {Environment.NewLine}" +
                    $"Path: {request.Path} {Environment.NewLine}" +
                    $"QueryString: {request.QueryString} {Environment.NewLine}" +
                    $"StatusCode: {response.StatusCode} {Environment.NewLine}" +
                    $"Time: {stopWatch.ElapsedMilliseconds} {Environment.NewLine}" +
                    $"Response Body: {ReadStreamInChunks(newResponseBody)}";
        }

        private async Task<string> FormatRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            return $"Http Request Information: {Environment.NewLine}" +
                        $"Schema:{request.Scheme} {Environment.NewLine}" +
                        $"Host: {request.Host} {Environment.NewLine}" +
                        $"Path: {request.Path} {Environment.NewLine}" +
                        $"QueryString: {request.QueryString} {Environment.NewLine}" +
                        $"Request Body: {await GetRequestBody(request)}";
        }

        public async Task<string> GetRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            //request.EnableRewind();

            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await request.Body.CopyToAsync(requestStream);
                request.Body.Seek(0, SeekOrigin.Begin);
                return ReadStreamInChunks(requestStream);
            }
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;
            using (var textWriter = new StringWriter())
            using (var reader = new StreamReader(stream))
            {
                var readChunk = new char[ReadChunkBufferLength];
                int readChunkLength;
                //do while: is useful for the last iteration in case readChunkLength < chunkLength
                do
                {
                    readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
                    textWriter.Write(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                result = textWriter.ToString();
            }

            return result;
        }

        public class RequestProfilerModel
        {
            public DateTimeOffset RequestTime { get; set; }
            public HttpContext Context { get; set; }
            public string Request { get; set; }
            public string Response { get; set; }
            public DateTimeOffset ResponseTime { get; set; }
        }
    }

    //public class ApiLoggingMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private ApiLogService _apiLogService;

    //    public ApiLoggingMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task Invoke(HttpContext httpContext, ApiLogService apiLogService)
    //    {
    //        try
    //        {
    //            _apiLogService = apiLogService;

    //            var request = httpContext.Request;
    //            if (request.Path.StartsWithSegments(new PathString("/api")))
    //            {
    //                var stopWatch = Stopwatch.StartNew();
    //                var requestTime = DateTime.UtcNow;
    //                var requestBodyContent = await ReadRequestBody(request);
    //                var originalBodyStream = httpContext.Response.Body;
    //                using (var responseBody = new MemoryStream())
    //                {
    //                    var response = httpContext.Response;
    //                    response.Body = responseBody;
    //                    await _next(httpContext);
    //                    stopWatch.Stop();

    //                    string responseBodyContent = null;
    //                    responseBodyContent = await ReadResponseBody(response);
    //                    await responseBody.CopyToAsync(originalBodyStream);

    //                    await SafeLog(requestTime,
    //                        stopWatch.ElapsedMilliseconds,
    //                        response.StatusCode,
    //                        request.Method,
    //                        request.Path,
    //                        request.QueryString.ToString(),
    //                        requestBodyContent,
    //                        responseBodyContent);
    //                }
    //            }
    //            else
    //            {
    //                await _next(httpContext);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            await _next(httpContext);
    //        }
    //    }

    //    private async Task<string> ReadRequestBody(HttpRequest request)
    //    {
    //        request.EnableRewind();

    //        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
    //        await request.Body.ReadAsync(buffer, 0, buffer.Length);
    //        var bodyAsText = Encoding.UTF8.GetString(buffer);
    //        request.Body.Seek(0, SeekOrigin.Begin);

    //        return bodyAsText;
    //    }

    //    private async Task<string> ReadResponseBody(HttpResponse response)
    //    {
    //        response.Body.Seek(0, SeekOrigin.Begin);
    //        var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
    //        response.Body.Seek(0, SeekOrigin.Begin);

    //        return bodyAsText;
    //    }

    //    private async Task SafeLog(DateTime requestTime,
    //                        long responseMillis,
    //                        int statusCode,
    //                        string method,
    //                        string path,
    //                        string queryString,
    //                        string requestBody,
    //                        string responseBody)
    //    {
    //        if (path.ToLower().StartsWith("/api/login"))
    //        {
    //            requestBody = "(Request logging disabled for /api/login)";
    //            responseBody = "(Response logging disabled for /api/login)";
    //        }

    //        if (requestBody.Length > 100)
    //        {
    //            requestBody = $"(Truncated to 100 chars) {requestBody.Substring(0, 100)}";
    //        }

    //        if (responseBody.Length > 100)
    //        {
    //            responseBody = $"(Truncated to 100 chars) {responseBody.Substring(0, 100)}";
    //        }

    //        if (queryString.Length > 100)
    //        {
    //            queryString = $"(Truncated to 100 chars) {queryString.Substring(0, 100)}";
    //        }

    //        await _apiLogService.Log(new ApiLogItem
    //        {
    //            RequestTime = requestTime,
    //            ResponseMillis = responseMillis,
    //            StatusCode = statusCode,
    //            Method = method,
    //            Path = path,
    //            QueryString = queryString,
    //            RequestBody = requestBody,
    //            ResponseBody = responseBody
    //        });
    //    }

    //    private class ApiLogItem
    //    {
    //        public DateTime RequestTime { get; set; }
    //        public long ResponseMillis { get; set; }
    //        public int StatusCode { get; set; }
    //        public string Method { get; set; }
    //        public string Path { get; set; }
    //        public string QueryString { get; set; }
    //        public string RequestBody { get; set; }
    //        public string ResponseBody { get; set; }
    //    }
    //}
}