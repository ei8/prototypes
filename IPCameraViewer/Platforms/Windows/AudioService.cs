#if WINDOWS
using IPCameraViewer.Services;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace IPCameraViewer.Platforms.Windows
{
    public class AudioService : IAudioService
    {
        private const string WinmmDll = "winmm.dll";
        private const string PlaySoundWEntryPoint = "PlaySoundW";
        private const string DebugFilePathNull = "AudioService: filePath is null or empty";
        private const string DebugFileNotExists = "AudioService: File does not exist: {0}";
        private const string DebugCouldNotGetFullPath = "AudioService: Could not get full path";
        private const string DebugAttemptingToPlay = "AudioService: Attempting to play sound: {0}";
        private const string DebugPlaySoundReturned = "AudioService: PlaySoundWin32 returned: {0} for path: {1}";
        private const string DebugPlaySoundFailed = "AudioService: PlaySoundWin32 failed with error code: {0}";
        private const string DebugDllNotFoundException = "AudioService: DllNotFoundException - {0}";
        private const string DebugEntryPointNotFoundException = "AudioService: EntryPointNotFoundException - {0}";
        private const string DebugBadImageFormatException = "AudioService: BadImageFormatException - {0}";
        private const string DebugAccessViolationException = "AudioService: AccessViolationException - {0}";
        private const string DebugExceptionFormat = "AudioService: Exception - {0}: {1}";

        [DllImport(AudioService.WinmmDll, EntryPoint = AudioService.PlaySoundWEntryPoint, CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PlaySoundWin32([MarshalAs(UnmanagedType.LPWStr)] string? pszSound, IntPtr hmod, uint fdwSound);

        private const uint SND_FILENAME = 0x00020000;
        private const uint SND_ASYNC = 0x0001;
        private const uint SND_NODEFAULT = 0x0002;

        public void PlaySound(string? filePath, double volume = 1.0)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        // Convert to full path to avoid issues
                        string fullPath = Path.GetFullPath(filePath);
                        if (!string.IsNullOrEmpty(fullPath))
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format(AudioService.DebugAttemptingToPlay, fullPath));

                            // Clamp volume between 0.0 and 1.0
                            double clampedVolume = Math.Max(0.0, Math.Min(1.0, volume));

                            // Use MediaPlayer for volume control support
                            // Fire and forget async operation
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    var mediaPlayer = new MediaPlayer();
                                    mediaPlayer.Volume = clampedVolume;
                                    
                                    // Open the file and play asynchronously
                                    var file = await StorageFile.GetFileFromPathAsync(fullPath);
                                    mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                                    mediaPlayer.Play();
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format(AudioService.DebugExceptionFormat, ex.GetType().Name, ex.Message));
                                }
                            });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(AudioService.DebugCouldNotGetFullPath);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format(AudioService.DebugFileNotExists, filePath));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format(AudioService.DebugExceptionFormat, ex.GetType().Name, ex.Message));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(AudioService.DebugFilePathNull);
            }
        }
    }
}
#endif

