using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Html;
using System.Text.RegularExpressions;

namespace ClearScore.Blobs
{
    public class BlobAgent
    {
        public static string connectionString = "DefaultEndpointsProtocol=https;AccountName=cpwebappservicestorage;AccountKey=BwRiSpB1qLhsq8NGngM+13i6X6dnpnBMdRI+2fwIIDgxA+CptA6UBh/q2jxCqkgSxTrka+FgJtx5+AStBA+QUA==;EndpointSuffix=core.windows.net";
        public static string key = "BwRiSpB1qLhsq8NGngM+13i6X6dnpnBMdRI+2fwIIDgxA+CptA6UBh/q2jxCqkgSxTrka+FgJtx5+AStBA+QUA==";

        public static BlobServiceClient _blobServiceClient = new BlobServiceClient(connectionString);

        public static BlobContainerClient GetBlobContainerClient(string container)
        {
            var client = _blobServiceClient.GetBlobContainerClient(container);
            return client;
        }

        public static async Task<HtmlString> GetImageAsync(string name = "chevron_right_20_regular.svg", string fill = "", int? width = null, int? height = null)
        {
            var container = GetBlobContainerClient("icons");
            string folder = name.Substring(0, 1);
            var client = container.GetBlobClient($"{folder}/{name}");

            if (await client.ExistsAsync())
            {
                BlobDownloadResult result = await client.DownloadContentAsync();

                string contents = result.Content.ToString();

                if (!string.IsNullOrEmpty(fill))
                {
                    contents = contents.Insert(5, $"fill=\"{fill}\" ");
                }

                HtmlString htmlString = HtmlString.Empty;

                if (width != null)
                {
                    string widthPattern = @"width=""\d+""";
                    string newWidth = $"width=\"{width}\"";
                    contents = Regex.Replace(contents, widthPattern, newWidth);
                }

                if (height != null)
                {
                    string heightPattern = @"height=""\d+""";
                    string newHeight = $"height=\"{height}\"";
                    contents = Regex.Replace(contents, heightPattern, newHeight);
                }

                var output = new HtmlString(contents.ToString());

                return output;
            }

            return new HtmlString(null);
        }


        public static async Task<HtmlString> GetDefaultImageAsync(string name, string? type = "regular")
        {
            var container = GetBlobContainerClient("icons");
            string folder = name.Substring(0, 1);
            var client = container.GetBlobClient($"{folder}/{name}_24_{type}.svg");

            if (await client.ExistsAsync())
            {
                BlobDownloadResult result = await client.DownloadContentAsync();

                string contents = result.Content.ToString();

                HtmlString htmlString = HtmlString.Empty;

                // height
                string widthPattern = @"width=""\d+""";
                string newWidth = $"width=\"20\"";
                contents = Regex.Replace(contents, widthPattern, newWidth);

                // height
                string heightPattern = @"height=""\d+""";
                string newHeight = $"height=\"20\"";
                contents = Regex.Replace(contents, heightPattern, newHeight);

                var output = new HtmlString(contents.ToString());

                return output;
            }

            return new HtmlString(null);
        }


        public static async Task<Stream?> DownloadAsync(string path)
        {
            var container = GetBlobContainerClient("files");
            var client = container.GetBlobClient(path);

            if (await client.ExistsAsync())
            {
                var file = await client.DownloadContentAsync();
                return file.Value.Content.ToStream();
            }
            else
            {
                return null;
            }
        }

        private static async Task<BlobCopyResult> _CopyBlobAsync(string source, string destination)
        {
            var container = GetBlobContainerClient("files");

            BlobClient sourceBlob = container.GetBlobClient(source);
            BlobClient destinationBlob = container.GetBlobClient(destination);

            if (!await sourceBlob.ExistsAsync())
            {
                return new BlobCopyResult
                {
                    Success = false,
                    Status = "Source blob does not exist"
                };
            }

            var operation = await destinationBlob.StartCopyFromUriAsync(sourceBlob.Uri);
            var properties = await destinationBlob.GetPropertiesAsync();

            return new BlobCopyResult
            {
                Success = properties.Value.CopyStatus == CopyStatus.Success,
                Status = properties.Value.CopyStatus.ToString(),
                CopyId = properties.Value.CopyId,
                DestinationUri = destinationBlob.Uri
            };
        }

        public static async Task<BlobCopyResult> CopyBlobAsync(string sourceFileName, string destinationFileName)
        {
            var container = GetBlobContainerClient("files");
            var result = await _CopyBlobAsync(sourceFileName, destinationFileName);
            return result;
        }
    }
}
