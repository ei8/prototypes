using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using Microsoft.Maui.Storage;

namespace IPCameraViewer.Services
{
    public class AppSettings
    {
        public List<CameraSettings> Cameras { get; set; } = new();
    }

    public class CameraSettings
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public double MotionThresholdPercent { get; set; } = 1.5;
        public double SoundVolume { get; set; } = 1.0;
        public bool SoundEnabled { get; set; } = true;
        public string? SoundFilePath { get; set; }
        public bool RecordingEnabled { get; set; } = false;
        public bool RecordingGif { get; set; } = false;
        public bool RecordingPng { get; set; } = false;
        public bool RecordingMp4 { get; set; } = true;
        public string? RecordingOutputPath { get; set; }
        public int RecordingBeforeSeconds { get; set; } = 5;
        public int RecordingAfterSeconds { get; set; } = 10;
    }

    public class SettingsService
    {
        private const string SettingsFileName = "settings.json";
        private readonly string _filePath;

        public AppSettings Settings { get; private set; }

        public SettingsService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, SettingsFileName);
            System.Diagnostics.Debug.WriteLine($"[SettingsService] Settings file path: {_filePath}");
            Settings = new AppSettings();
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    var loadedSettings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (loadedSettings != null)
                    {
                        Settings = loadedSettings;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                // Fallback to default settings if load fails
                Settings = new AppSettings();
            }
        }

        public void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Settings, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
    }
}
