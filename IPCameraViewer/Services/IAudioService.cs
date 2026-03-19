namespace IPCameraViewer.Services
{
    public interface IAudioService
    {
        void PlaySound(string? filePath, double volume = 1.0);
    }
}

