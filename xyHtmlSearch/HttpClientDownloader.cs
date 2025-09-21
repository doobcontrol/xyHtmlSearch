using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace xyHtmlSearch
{
    public class HttpClientDownloader
    {
        private readonly HttpClient _httpClient;

        //For unit testing
        public HttpClientDownloader(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36"
                );
        }

        public async Task DownloadFileAsync(
            string uri,
            string outputPath)
        {
            Uri uriResult;

            if (!Uri.TryCreate(uri, UriKind.Absolute, out uriResult))
                throw new InvalidOperationException("URI Is Invalid");

            byte[] fileBytes = await _httpClient.GetByteArrayAsync(uri);
            File.WriteAllBytes(outputPath, fileBytes);
        }

        public async Task<string> GetHtmlStringAsync(
            string url,
            string encoding)
        {
            var response = await _httpClient.GetByteArrayAsync(url);
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var responseString = await Task.Run(
                () =>
                Encoding.GetEncoding(encoding).
                    GetString(response, 0, response.Length)
            );
            return responseString;
        }
    }
}
