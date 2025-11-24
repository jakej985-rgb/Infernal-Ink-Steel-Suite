using System.IO;
using System;

namespace InfernalInkSteelSuite.Services
{
    public class ImageValidationService
    {
        public static (bool IsValid, string Message) ValidateImage(string path, int? maxWidthPx = null, int? maxHeightPx = null, long? maxSizeBytes = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return (false, "No image path provided");
            }

            if (!File.Exists(path))
            {
                return (false, "Image file does not exist");
            }

            try
            {
                var fileInfo = new FileInfo(path);

                // Check file size
                if (maxSizeBytes.HasValue && fileInfo.Length > maxSizeBytes.Value)
                {
                    var maxMB = maxSizeBytes.Value / (1024.0 * 1024.0);
                    return (false, $"Image file size exceeds {maxMB:0.##} MB");
                }

                // Check dimensions if specified
                if (maxWidthPx.HasValue || maxHeightPx.HasValue)
                {
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(
                        stream,
                        System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreColorProfile,
                        System.Windows.Media.Imaging.BitmapCacheOption.Default);

                    if (decoder.Frames.Count > 0)
                    {
                        var frame = decoder.Frames[0];

                        if (maxWidthPx.HasValue && frame.PixelWidth > maxWidthPx.Value)
                        {
                            return (false, $"Image width ({frame.PixelWidth}px) exceeds maximum ({maxWidthPx.Value}px)");
                        }

                        if (maxHeightPx.HasValue && frame.PixelHeight > maxHeightPx.Value)
                        {
                            return (false, $"Image height ({frame.PixelHeight}px) exceeds maximum ({maxHeightPx.Value}px)");
                        }
                    }
                }

                return (true, "Image is valid");
            }
            catch (Exception ex)
            {
                return (false, $"Error validating image: {ex.Message}");
            }
        }

        public static (int Width, int Height, long SizeBytes) GetImageMetadata(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return (0, 0, 0);
            }

            try
            {
                var fileInfo = new FileInfo(path);
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(
                    stream,
                    System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreColorProfile,
                    System.Windows.Media.Imaging.BitmapCacheOption.Default);

                if (decoder.Frames.Count > 0)
                {
                    var frame = decoder.Frames[0];
                    return (frame.PixelWidth, frame.PixelHeight, fileInfo.Length);
                }

                return (0, 0, fileInfo.Length);
            }
            catch
            {
                return (0, 0, 0);
            }
        }
    }
}
