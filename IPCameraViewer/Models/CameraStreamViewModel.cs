using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IPCameraViewer.Services;
using Microsoft.Maui.Controls;

namespace IPCameraViewer.Models
{
    public class CameraStreamViewModel : INotifyPropertyChanged
    {
        private const string DefaultMetricsText = "Metrics: -";
        private const string DefaultMotionIdleText = "Motion: idle";
        private const double DefaultMotionThresholdPercent = 1.5;
        private const double DefaultSoundVolume = 1.0;
        private const bool DefaultSoundEnabled = true;
        private const bool DefaultRecordingEnabled = false;
        private const bool DefaultRecordingGif = false;
        private const bool DefaultRecordingPng = false;
        private const bool DefaultRecordingMp4 = true;
        private const int DefaultRecordingBeforeSeconds = 5;
        private const int DefaultRecordingAfterSeconds = 10;

        private int id;
        private string cameraName = string.Empty;
        private string url = string.Empty;
        private bool isRunning;
        private ImageSource? currentFrame;
        private string metrics = CameraStreamViewModel.DefaultMetricsText;
        private string motionStatus = CameraStreamViewModel.DefaultMotionIdleText;
        private Color motionColor = Colors.Gray;
        private float lastRatio;
        private double motionThresholdPercent = CameraStreamViewModel.DefaultMotionThresholdPercent;
        private double soundVolume = CameraStreamViewModel.DefaultSoundVolume;
        private bool soundEnabled = CameraStreamViewModel.DefaultSoundEnabled;
        private string? soundFilePath;
        private bool recordingEnabled = CameraStreamViewModel.DefaultRecordingEnabled;
        private bool recordingGif = CameraStreamViewModel.DefaultRecordingGif;
        private bool recordingPng = CameraStreamViewModel.DefaultRecordingPng;
        private bool recordingMp4 = CameraStreamViewModel.DefaultRecordingMp4;
        private string? recordingOutputPath;
        private int recordingBeforeSeconds = CameraStreamViewModel.DefaultRecordingBeforeSeconds;
        private int recordingAfterSeconds = CameraStreamViewModel.DefaultRecordingAfterSeconds;
        private bool isMotionHighlighted;

        // Callback to notify when settings change
        public Action? OnSettingsChanged { get; set; }

        public int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public string CameraName
        {
            get => this.cameraName;
            set
            {
                if (this.SetProperty(ref this.cameraName, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public string Url
        {
            get => this.url;
            set
            {
                if (this.SetProperty(ref this.url, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool IsRunning
        {
            get => this.isRunning;
            set => this.SetProperty(ref this.isRunning, value);
        }

        public ImageSource? CurrentFrame
        {
            get => this.currentFrame;
            set => this.SetProperty(ref this.currentFrame, value);
        }

        public string Metrics
        {
            get => this.metrics;
            set => this.SetProperty(ref this.metrics, value);
        }

        public string MotionStatus
        {
            get => this.motionStatus;
            set => this.SetProperty(ref this.motionStatus, value);
        }

        public Color MotionColor
        {
            get => this.motionColor;
            set => this.SetProperty(ref this.motionColor, value);
        }

        public float LastRatio
        {
            get => this.lastRatio;
            set => this.SetProperty(ref this.lastRatio, value);
        }

        public double MotionThresholdPercent
        {
            get => this.motionThresholdPercent;
            set
            {
                if (this.SetProperty(ref this.motionThresholdPercent, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public double SoundVolume
        {
            get => this.soundVolume;
            set
            {
                if (this.SetProperty(ref this.soundVolume, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool SoundEnabled
        {
            get => this.soundEnabled;
            set
            {
                if (this.SetProperty(ref this.soundEnabled, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public string? SoundFilePath
        {
            get => this.soundFilePath;
            set
            {
                if (this.SetProperty(ref this.soundFilePath, value))
                {
                    this.OnPropertyChanged(nameof(this.SoundFileName));
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public string SoundFileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.soundFilePath))
                {
                    return "No file selected";
                }
                return System.IO.Path.GetFileName(this.soundFilePath);
            }
        }

        public bool RecordingEnabled
        {
            get => this.recordingEnabled;
            set
            {
                if (this.SetProperty(ref this.recordingEnabled, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public int RecordingBeforeSeconds
        {
            get => this.recordingBeforeSeconds;
            set
            {
                if (this.SetProperty(ref this.recordingBeforeSeconds, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public int RecordingAfterSeconds
        {
            get => this.recordingAfterSeconds;
            set
            {
                if (this.SetProperty(ref this.recordingAfterSeconds, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool RecordingGif
        {
            get => this.recordingGif;
            set
            {
                if (this.SetProperty(ref this.recordingGif, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool RecordingPng
        {
            get => this.recordingPng;
            set
            {
                if (this.SetProperty(ref this.recordingPng, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool RecordingMp4
        {
            get => this.recordingMp4;
            set
            {
                if (this.SetProperty(ref this.recordingMp4, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public string? RecordingOutputPath
        {
            get => this.recordingOutputPath;
            set
            {
                if (this.SetProperty(ref this.recordingOutputPath, value))
                {
                    this.OnSettingsChanged?.Invoke();
                }
            }
        }

        public ObservableCollection<string> DetectionLogs { get; } = new();

        public bool IsMotionHighlighted
        {
            get => this.isMotionHighlighted;
            set => this.SetProperty(ref this.isMotionHighlighted, value);
        }


        public MjpegStreamer? Streamer { get; set; }
        
        // Frame buffer for recording
        public IPCameraViewer.Services.FrameBuffer? FrameBuffer { get; set; }
        
        // Recording state
        public bool IsRecording { get; set; }
        public DateTime? RecordingStartTime { get; set; }
        public List<IPCameraViewer.Services.FrameData> RecordingFrames { get; set; } = new();
        
        // Lock object for thread-safe access to RecordingFrames
        public object RecordingFramesLock { get; } = new object();
        
        // Current frame bytes for recording
        public byte[]? CurrentFrameBytes { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CameraStreamViewModel()
        {
        }

        public CameraStreamViewModel(CameraSettings settings)
        {
            this.id = settings.Id;
            this.cameraName = settings.Name;
            this.url = settings.Url;
            this.motionThresholdPercent = settings.MotionThresholdPercent;
            this.soundVolume = settings.SoundVolume;
            this.soundEnabled = settings.SoundEnabled;
            this.soundFilePath = settings.SoundFilePath;
            this.recordingEnabled = settings.RecordingEnabled;
            this.recordingGif = settings.RecordingGif;
            this.recordingPng = settings.RecordingPng;
            this.recordingMp4 = settings.RecordingMp4;
            this.recordingOutputPath = settings.RecordingOutputPath;
            this.recordingBeforeSeconds = settings.RecordingBeforeSeconds;
            this.recordingAfterSeconds = settings.RecordingAfterSeconds;
        }

        public CameraSettings ToSettings()
        {
            return new CameraSettings
            {
                Id = this.Id,
                Name = this.CameraName,
                Url = this.Url,
                MotionThresholdPercent = this.MotionThresholdPercent,
                SoundVolume = this.SoundVolume,
                SoundEnabled = this.SoundEnabled,
                SoundFilePath = this.SoundFilePath,
                RecordingEnabled = this.RecordingEnabled,
                RecordingGif = this.RecordingGif,
                RecordingPng = this.RecordingPng,
                RecordingMp4 = this.RecordingMp4,
                RecordingOutputPath = this.RecordingOutputPath,
                RecordingBeforeSeconds = this.RecordingBeforeSeconds,
                RecordingAfterSeconds = this.RecordingAfterSeconds
            };
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            bool result = true;
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                result = false;
            }
            else
            {
                field = value;
                this.OnPropertyChanged(propertyName);
            }
            return result;
        }
    }
}