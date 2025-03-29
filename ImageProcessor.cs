using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats; // Required for IImageFormat, IImageEncoder
using SixLabors.ImageSharp.Formats.Png; // Required for PngEncoder
using SixLabors.ImageSharp.Metadata; // Required for ImageMetadata

namespace ImageSharpProcessorLib
{
    /// <summary>
    /// Provides functionality to process images, specifically changing PPI and format.
    /// </summary>
    public class ImageProcessor
    {
        /// <summary>
        /// Processes an image file by setting its PPI and optionally converting it to PNG format.
        /// The output file is saved in the same directory as the input file.
        /// </summary>
        /// <param name="imagePath">The full path to the input image file.</param>
        /// <param name="ppi">The desired Pixels Per Inch (PPI) for the output image. Defaults to 144.</param>
        /// <param name="convertToPng">If true, the image will be converted to PNG format. If false, the original format will be retained.</param>
        /// <exception cref="ArgumentNullException">Thrown if imagePath is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the file specified by imagePath does not exist.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if ppi is less than or equal to 0.</exception>
        /// <exception cref="UnknownImageFormatException">Thrown if the input image format cannot be determined or is unsupported.</exception>
        /// <exception cref="IOException">Thrown if there is an error during file reading or writing.</exception>
        public static void ProcessImage(string imagePath, int ppi = 144, bool convertToPng = false)
        {
            // --- Input Validation ---
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                throw new ArgumentNullException(nameof(imagePath), "Image path cannot be null or empty.");
            }
        
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"The specified image file was not found: {imagePath}", imagePath);
            }
        
            if (ppi <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ppi), "PPI must be a positive value.");
            }

            string tempOutputPath = string.Empty;
            try
            {
                // --- Load Image and Detect Format ---
                IImageFormat originalFormat;
                using (var stream = File.OpenRead(imagePath))
                {
                    // Detect format
                    originalFormat = Image.DetectFormat(stream) 
                        ?? throw new UnknownImageFormatException($"Could not detect image format for: {imagePath}");
                    
                    // Reset stream and load image
                    stream.Position = 0;
                    using var image = Image.Load(stream);

                    // --- Determine Output Format and Encoder ---
                    IImageEncoder outputEncoder;
                    string outputExtension;

                    if (convertToPng)
                    {
                        outputEncoder = new PngEncoder();
                        outputExtension = ".png";
                    }
                    else
                    {
                        outputEncoder = image.Configuration.ImageFormatsManager.GetEncoder(originalFormat)
                            ?? throw new NotSupportedException($"Encoding is not supported for the format: {originalFormat.Name}");
                        outputExtension = Path.GetExtension(imagePath)?.ToLowerInvariant() ?? string.Empty;
                    }

                    // --- Determine Output Path ---
                    string outputDirectory = Path.GetDirectoryName(imagePath) ?? Environment.CurrentDirectory;
                    string outputFileNameBase = Path.GetFileNameWithoutExtension(imagePath);
                    string finalOutputPath = Path.Combine(outputDirectory, outputFileNameBase + outputExtension);
                    tempOutputPath = Path.Combine(outputDirectory, outputFileNameBase + "_temp" + outputExtension);

                    // --- Apply Changes ---
                    // Set PPI Metadata
                    image.Metadata.ResolutionUnits = PixelResolutionUnit.PixelsPerInch;
                    image.Metadata.HorizontalResolution = ppi;
                    image.Metadata.VerticalResolution = ppi;

                    // Save to temporary file first
                    image.Save(tempOutputPath, outputEncoder);
                }

                // Replace original file with new file
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
                File.Move(tempOutputPath, imagePath);
            }
            catch (UnknownImageFormatException ex)
            {
                throw new Exception(
                    $"Could not process image. Format may be unsupported or file corrupted: {imagePath}. Inner exception: {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new IOException($"An error occurred during file I/O for {imagePath}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error processing {imagePath}: {ex}");
                throw;
            }
            finally
            {
                // Clean up temporary file if it exists
                if (!string.IsNullOrEmpty(tempOutputPath) && File.Exists(tempOutputPath))
                {
                    try
                    {
                        File.Delete(tempOutputPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Failed to delete temporary file: {ex.Message}");
                    }
                }
            }
        }
    }
}