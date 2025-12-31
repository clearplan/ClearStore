namespace ClearStore.Models
{
    public class ImageDetail
    {
        public int? ImageId { get; set; }

        public byte[]? ImageData { get; set; }

        public string ImageName { get; set; } = string.Empty;
    }
}
