namespace PixelService.Images
{
    public class GifImageGenerator : IGifImageGenerator
    {
        public byte[] Generate()
        {
            // Create a 1x1 pixel GIF image
            byte[] gifBytes =
            {
                0x47, 0x49, 0x46, 0x38, 0x39, 0x61, // GIF89a header
                0x01, 0x00, 0x01, 0x00,             // Image dimensions (1x1)
                0xF7, 0x00,                         // Global color table (not used for 1x1 image)
                0xFF, 0x00, 0x00,                   // Color table for pixel (red)
                0x2C, 0x00, 0x00, 0x00, 0x00,       // Image separator and position
                0x01, 0x00, 0x01, 0x00,             // Image dimensions (1x1)
                0x00,                               // No local color table
                0x02,                               // LZW minimum code size
                0x02, 0x44, 0x01, 0x00,             // Image data
                0x3B                                // GIF trailer
            };

            return gifBytes;
        }
    }
}