namespace IPCameraViewer.Services
{
    public class NoOpAudioService : IAudioService
    {
        public void PlaySound(string? filePath, double volume = 1.0)
        {
            // No-op implementation for non-Windows platforms
        }
    }
}

