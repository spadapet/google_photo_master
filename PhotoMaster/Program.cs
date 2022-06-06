using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CasCap.Models;
using CasCap.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PhotoMaster
{
    internal static class Program
    {
        private const int ResultSuccess = 0;
        private const int ResultError = 1;

        internal async static Task<int> Main(string[] args)
        {
            int result = Program.ResultSuccess;
            const string userEmail = "lilapeterspada@gmail.com";

            IOptions<GooglePhotosOptions> options = Options.Create(new GooglePhotosOptions()
            {
                User = userEmail,
                ClientId = "012345678901-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.apps.googleusercontent.com",
                ClientSecret = "abcabcabcabcabcabcabcabc",
                Scopes = new[] { GooglePhotosScope.Access },
            });

            using (HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            })
            using (HttpClient client = new HttpClient(handler))
            using (LoggerFactory loggerFactory = new LoggerFactory())
            {
                ILogger<GooglePhotosService> logger = loggerFactory.CreateLogger<GooglePhotosService>();
                GooglePhotosService photos = new GooglePhotosService(logger, options, client);

                try
                {
                    if (!await photos.LoginAsync())
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(ex.Message))
                    {
                        logger.LogError(ex);
                    }

                    result = Program.ResultError;
                }
            }

            return result;
        }
    }
}
