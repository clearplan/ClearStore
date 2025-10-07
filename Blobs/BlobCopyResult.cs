#nullable disable
namespace ClearScore.Blobs
{
    public class BlobCopyResult
    {
        public bool Success { get; set; }
        public string Status { get; set; }
        public string CopyId { get; set; }
        public Uri DestinationUri { get; set; }
    }
}
