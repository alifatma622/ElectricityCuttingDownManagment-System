using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ElectricityCuttingDownManagmentSystem.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        // store client stats  for each client IP
        private static readonly ConcurrentDictionary<string, ClientStats> _clients = new();

        // 10 requests per second 
        private const int MaxRequestsPerSecond = 10;

        // max 1 MB bandwidth per second
        private const long MaxBandwidthPerSecond = 1024 * 1024; 

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            // cleanup  to remove old entries
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    CleanupOldData();
                }
            });
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = GetClientIp(context);
            var stats = _clients.GetOrAdd(clientIp, _ => new ClientStats());

            //validate requests limit
            if (!stats.AllowRequest())
            {
                _logger.LogWarning($"Rate limit exceeded for IP: {clientIp}");
                context.Response.StatusCode = 429; // Too many  Requests so send 429
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            //validate bandwidth limit
            var contentLength = context.Request.ContentLength ?? 0;
            if (!stats.AllowBandwidth(contentLength))
            {
                _logger.LogWarning($"Bandwidth limit exceeded for IP: {clientIp}");
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Bandwidth limit exceeded. Please try again later.");
                return;
            }

            await _next(context);
        }

        private static string GetClientIp(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private static void CleanupOldData()
        {
            var now = DateTime.UtcNow;
            foreach (var kvp in _clients)
            {
                if ((now - kvp.Value.LastRequest).TotalMinutes > 5)
                {
                    _clients.TryRemove(kvp.Key, out _);
                }
            }
        }

        private class ClientStats
        {
            private readonly SemaphoreSlim _semaphore = new(1, 1);
            private int _requestCount;
            private long _bandwidthUsed;
            private DateTime _windowStart = DateTime.UtcNow;
            public DateTime LastRequest { get; private set; } = DateTime.UtcNow;

            public bool AllowRequest()
            {
                _semaphore.Wait();
                try
                {
                    var now = DateTime.UtcNow;
                    LastRequest = now;

                    //if  one second has passed, reset counters
                    if ((now - _windowStart).TotalSeconds >= 1)
                    {
                        _windowStart = now;
                        _requestCount = 0;
                        _bandwidthUsed = 0;
                    }

                    if (_requestCount >= MaxRequestsPerSecond)
                    {
                        return false;
                    }

                    _requestCount++;
                    return true;
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public bool AllowBandwidth(long bytes)
            {
                _semaphore.Wait();
                try
                {
                    if (_bandwidthUsed + bytes > MaxBandwidthPerSecond)
                    {
                        return false;
                    }

                    _bandwidthUsed += bytes;
                    return true;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }
    }
}
