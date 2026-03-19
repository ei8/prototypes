using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace IPCameraViewer.Services
{
    public class DetectionRecorder
    {
        public async Task SaveRecordingAsync(
            List<FrameData> frames,
            string cameraName,
            string outputDirectory,
            int beforeSeconds,
            int afterSeconds,
            bool saveGif,
            bool savePng,
            bool saveMp4,
            DateTime detectionTime)
        {
            if (frames == null || frames.Count == 0)
            {
                return;
            }

            // Ensure output directory exists
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Create filename with timestamp
            string timestamp = detectionTime.ToString("yyyyMMdd_HHmmss");
            string safeCameraName = SanitizeFileName(cameraName);
            string baseFileName = $"{safeCameraName}_{timestamp}";

            var tasks = new List<Task>();

            if (saveGif)
            {
                tasks.Add(SaveAsGifAsync(frames, outputDirectory, baseFileName));
            }

            if (savePng)
            {
                tasks.Add(SaveAsPngSequenceAsync(frames, outputDirectory, baseFileName));
            }

            if (saveMp4)
            {
                tasks.Add(SaveAsMp4Async(frames, outputDirectory, baseFileName, beforeSeconds + afterSeconds));
            }

            await Task.WhenAll(tasks);
        }

        private async Task SaveAsGifAsync(List<FrameData> frames, string outputDirectory, string baseFileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (frames.Count == 0)
                    {
                        return;
                    }

                    string gifPath = Path.Combine(outputDirectory, $"{baseFileName}.gif");

                    var images = new List<Image<Rgba32>>();
                    try
                    {
                        // Load all frames
                        foreach (var frame in frames)
                        {
                            var image = SixLabors.ImageSharp.Image.Load<Rgba32>(frame.JpegBytes);
                            images.Add(image);
                        }

                        if (images.Count > 0)
                        {
                            // Create animated GIF using the first image as base
                            using var animatedGif = images[0].CloneAs<Rgba32>();
                            var gifMetadata = animatedGif.Metadata.GetGifMetadata();
                            gifMetadata.RepeatCount = 0; // Loop forever

                            // Add remaining frames with delay (100ms = 10 FPS)
                            for (int i = 1; i < images.Count; i++)
                            {
                                // Get the root frame from the image (images are kept alive in the list)
                                var frame = images[i].Frames.RootFrame;
                                var frameMetadata = frame.Metadata.GetGifMetadata();
                                frameMetadata.FrameDelay = 10; // 10 * 10ms = 100ms per frame
                                animatedGif.Frames.AddFrame(frame);
                            }

                            var gifEncoder = new GifEncoder
                            {
                                ColorTableMode = GifColorTableMode.Global
                            };

                            animatedGif.Save(gifPath, gifEncoder);
                        }
                    }
                    finally
                    {
                        foreach (var img in images)
                        {
                            img?.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving GIF: {ex.Message}");
                }
            });
        }

        private async Task SaveAsPngSequenceAsync(List<FrameData> frames, string outputDirectory, string baseFileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    string pngDir = Path.Combine(outputDirectory, $"{baseFileName}_png");
                    Directory.CreateDirectory(pngDir);

                    var pngEncoder = new PngEncoder();

                    for (int i = 0; i < frames.Count; i++)
                    {
                        using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(frames[i].JpegBytes);
                        string pngPath = Path.Combine(pngDir, $"frame_{i:D4}.png");
                        image.Save(pngPath, pngEncoder);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving PNG sequence: {ex.Message}");
                }
            });
        }

        private async Task SaveAsMp4Async(List<FrameData> frames, string outputDirectory, string baseFileName, int totalDurationSeconds)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (frames.Count == 0)
                    {
                        return;
                    }

                    string mp4Path = Path.Combine(outputDirectory, $"{baseFileName}.mp4");
                    
                    // First, save frames as temporary PNG files for FFmpeg
                    string tempPngDir = Path.Combine(outputDirectory, $"{baseFileName}_temp_png");
                    Directory.CreateDirectory(tempPngDir);

                    var pngEncoder = new PngEncoder();
                    for (int i = 0; i < frames.Count; i++)
                    {
                        using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(frames[i].JpegBytes);
                        string pngPath = Path.Combine(tempPngDir, $"frame_{i:D6}.png");
                        image.Save(pngPath, pngEncoder);
                    }

                    // Try to use FFmpeg to create MP4
                    bool ffmpegSuccess = await TryCreateMp4WithFfmpegAsync(tempPngDir, mp4Path, frames.Count, totalDurationSeconds);

                    // Clean up temporary PNG files
                    try
                    {
                        Directory.Delete(tempPngDir, true);
                    }
                    catch { }

                    if (!ffmpegSuccess)
                    {
                        // If FFmpeg failed, create a note file with detailed instructions
                        string notePath = Path.Combine(outputDirectory, $"{baseFileName}_mp4_note.txt");
                        
                        string instructions = 
                            "MP4 ENCODING FAILED - FFmpeg Not Found\n" +
                            "==========================================\n\n" +
                            "FFmpeg is required to create MP4 video files from the recorded frames.\n\n" +
                            "TO INSTALL FFmpeg:\n" +
                            "1. Download FFmpeg from: https://www.gyan.dev/ffmpeg/builds/\n" +
                            "   (or https://ffmpeg.org/download.html)\n" +
                            "2. Extract the ZIP file\n" +
                            "3. Add the 'bin' folder to your system PATH:\n" +
                            "   - Open System Properties > Environment Variables\n" +
                            "   - Edit the 'Path' variable\n" +
                            "   - Add the path to FFmpeg bin folder (e.g., C:\\ffmpeg\\bin)\n" +
                            "   - Restart the application\n\n" +
                            "OR install FFmpeg to one of these locations:\n" +
                            $"  - {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ffmpeg", "bin")}\n" +
                            $"  - {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ffmpeg", "bin")}\n\n" +
                            "TO CREATE MP4 MANUALLY:\n" +
                            $"Open Command Prompt in: {outputDirectory}\n" +
                            $"Run: ffmpeg -framerate 15 -i \"{baseFileName}_temp_png/frame_%06d.png\" -c:v libx264 -pix_fmt yuv420p -y \"{baseFileName}.mp4\"\n\n" +
                            $"Note: Temporary PNG files were cleaned up. If you need them, disable MP4 recording and use PNG format instead.";
                        
                        File.WriteAllText(notePath, instructions);
                        
                        System.Diagnostics.Debug.WriteLine("MP4 encoding failed: FFmpeg not available or conversion failed.");
                        System.Diagnostics.Debug.WriteLine($"Note file created at: {notePath}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving MP4: {ex.Message}");
                }
            });
        }

        private async Task<bool> TryCreateMp4WithFfmpegAsync(string pngDirectory, string outputMp4Path, int frameCount, int totalDurationSeconds)
        {
            try
            {
                // Calculate framerate based on the actual total duration
                double framerate = frameCount / (double)totalDurationSeconds;
                
                // Clamp to reasonable values (minimum 0.1 FPS for very slow cameras)
                framerate = Math.Max(0.1, Math.Min(30.0, framerate));
                
                System.Diagnostics.Debug.WriteLine($"[MP4] Frame count: {frameCount}, Duration: {totalDurationSeconds}s, Framerate: {framerate:F2} FPS");

                string? ffmpegExe = FindFfmpegExecutable();
                
                if (string.IsNullOrEmpty(ffmpegExe))
                {
                    System.Diagnostics.Debug.WriteLine("FFmpeg not found in system PATH or common locations");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Using FFmpeg at: {ffmpegExe}");

                string inputPattern = Path.Combine(pngDirectory, "frame_%06d.png");
                
                // Verify input pattern files exist
                string firstFrame = Path.Combine(pngDirectory, "frame_000000.png");
                if (!File.Exists(firstFrame))
                {
                    System.Diagnostics.Debug.WriteLine($"First frame not found: {firstFrame}");
                    return false;
                }

                string arguments = $"-framerate {framerate:F2} -i \"{inputPattern}\" -c:v libx264 -pix_fmt yuv420p -preset medium -crf 23 -y \"{outputMp4Path}\"";

                System.Diagnostics.Debug.WriteLine($"FFmpeg command: {ffmpegExe} {arguments}");

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegExe,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(processStartInfo);
                if (process == null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to start FFmpeg process");
                    return false;
                }

                // Read output and error streams
                string? errorOutput = null;
                _ = Task.Run(async () =>
                {
                    errorOutput = await process.StandardError.ReadToEndAsync();
                });

                // Wait for completion with timeout (30 seconds per second of video, max 5 minutes)
                int timeoutMs = Math.Min(300000, (int)(frameCount / framerate * 30000));
                bool completed = await Task.Run(() => process.WaitForExit(timeoutMs));

                if (!completed)
                {
                    System.Diagnostics.Debug.WriteLine("FFmpeg process timed out");
                    try { process.Kill(); } catch { }
                    return false;
                }

                if (process.ExitCode != 0)
                {
                    System.Diagnostics.Debug.WriteLine($"FFmpeg exited with code {process.ExitCode}");
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        System.Diagnostics.Debug.WriteLine($"FFmpeg error: {errorOutput}");
                    }
                    return false;
                }

                bool fileExists = File.Exists(outputMp4Path);
                if (fileExists)
                {
                    System.Diagnostics.Debug.WriteLine($"MP4 file created successfully: {outputMp4Path}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"MP4 file not found after FFmpeg completion: {outputMp4Path}");
                }

                return fileExists;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FFmpeg execution error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private static string? FindFfmpegExecutable()
        {
            // On Windows, try common installation locations first (faster than PATH check)
#if WINDOWS
            string[] commonPaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ffmpeg", "bin", "ffmpeg.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ffmpeg", "bin", "ffmpeg.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ffmpeg", "bin", "ffmpeg.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ImageMagick-7.1.0-Q16-HDRI", "ffmpeg.exe"), // Sometimes bundled with ImageMagick
            };

            foreach (var path in commonPaths)
            {
                if (File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"Found FFmpeg at: {path}");
                    return path;
                }
            }
#endif

            // Try to find ffmpeg in PATH by checking if it can be executed
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(processStartInfo);
                if (process != null)
                {
                    bool exited = process.WaitForExit(3000);
                    if (exited && process.ExitCode == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Found FFmpeg in system PATH");
                        return "ffmpeg";
                    }
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // FFmpeg not found in PATH - this is expected if not installed
                System.Diagnostics.Debug.WriteLine("FFmpeg not found in system PATH");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking for FFmpeg in PATH: {ex.Message}");
            }

            return null;
        }

        private static string SanitizeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
                .TrimEnd('.');
        }
    }
}

