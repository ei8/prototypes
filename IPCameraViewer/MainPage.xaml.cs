using Microsoft.Maui.Controls;
using IPCameraViewer.Services;
using IPCameraViewer.Models;
using System.Net.Http;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace IPCameraViewer
{
    public partial class MainPage : ContentPage
    {
        private const string MetricsResetText = "Metrics: -";
        private const string MotionIdleText = "Motion: idle";
        private const string MotionDetectedText = "Motion: detected";
        private const string ErrorTitle = "Error";
        private const string OkButtonText = "OK";
        private const string InvalidCameraUrlMessage = "Please enter a valid camera URL.";
        private const string CameraUrlAlreadyAddedMessage = "This camera URL is already added.";
        private const string CameraNameFormat = "Camera {0}";
        private const string AddedStreamFormat = "Added stream: {0}";
        private const string RemovedStreamFormat = "Removed stream: {0}";
        private const string StoppedStreamFormat = "Stopped stream: {0}";
        private const string StartedStreamFormat = "Started stream: {0}";
        private const string AllStreamsStoppedText = "All streams stopped";
        private const string AllLogsClearedText = "All logs cleared";
        private const string StreamErrorTitleFormat = "Stream Error - {0}";
        private const string MetricsFormat = "Metrics: ratio={0:0.000}, changed={1}/{2}";
        private const string MotionDetectedLogFormat = "[{0}] Motion detected (ratio={1:0.000})";
        private const string StatusFormat = "{0} | Active streams: {1}/{2}";
        private const string ReadyStatusText = "Ready";
        private const string EmptyString = "";
        private const int MaxDetectionLogs = 1000;

        private readonly ObservableCollection<CameraStreamViewModel> streams = new();
        private int streamIdCounter = 0;
        private IAudioService? audioService;
        private readonly DetectionRecorder detectionRecorder = new();
        private readonly SettingsService settingsService = new();

        private const string DebugPlayMotionSoundCalled = "PlayMotionSound: Called";
        private const string DebugAudioServiceNull = "PlayMotionSound: audioService is null, attempting to resolve";
        private const string DebugServiceResolved = "PlayMotionSound: Service resolved: {0}";
        private const string DebugApplicationNull = "PlayMotionSound: Application.Current or Handler is null";
        private const string DebugAudioServiceStillNull = "PlayMotionSound: audioService is still null, cannot play sound";
        private const string DebugSoundEnabled = "PlayMotionSound: Sound enabled: {0}";
        private const string DebugSoundFilePath = "PlayMotionSound: Sound file path: {0}";
        private const string DebugNoSoundFilePath = "PlayMotionSound: No sound file path configured";
        private const string DebugFileNotExists = "PlayMotionSound: File does not exist: {0}";
        private const string DebugCallingPlaySound = "PlayMotionSound: Calling audioService.PlaySound({0})";
        private const string DebugExceptionFormat = "PlayMotionSound: Exception - {0}: {1}";
        private const string DebugStackTrace = "PlayMotionSound: StackTrace: {0}";

        public MainPage()
        {
            InitializeComponent();
            this.StreamsListCollection.ItemsSource = this.streams;

            // Load settings
            this.LoadSettings();

            // Try to get services after initialization
            try
            {
                var app = Application.Current;
                if (app?.Handler?.MauiContext?.Services != null)
                {
                    this.audioService = app.Handler.MauiContext.Services.GetService<IAudioService>();
                }
            }
            catch
            {
                // Services will remain null if they can't be resolved
            }
        }

        private void LoadSettings()
        {
            this.settingsService.LoadSettings();
            foreach (var cameraSettings in this.settingsService.Settings.Cameras)
            {
                var vm = new CameraStreamViewModel(cameraSettings)
                {
                    OnSettingsChanged = this.SaveSettings
                };
                this.streams.Add(vm);
                this.StartStream(vm);
            }

            if (this.streams.Any())
            {
                this.streamIdCounter = this.streams.Max(s => s.Id) + 1;
            }
        }

        private void SaveSettings()
        {
            this.settingsService.Settings.Cameras = this.streams.Select(s => s.ToSettings()).ToList();
            this.settingsService.SaveSettings();
        }

        private void OnAddStreamClicked(object sender, EventArgs e)
        {
            var cameraUrl = this.CameraUrlEntry.Text?.Trim();
            var cameraName = this.CameraNameEntry.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(cameraUrl))
            {
                if (string.IsNullOrWhiteSpace(cameraName))
                {
                    cameraName = string.Format(MainPage.CameraNameFormat, this.streamIdCounter + 1);
                }

                // Check if URL already exists
                if (!this.streams.Any(s => s.Url == cameraUrl))
                {
                    var streamViewModel = new CameraStreamViewModel
                    {
                        Id = this.streamIdCounter++,
                        CameraName = cameraName,
                        Url = cameraUrl,
                        OnSettingsChanged = this.SaveSettings
                    };

                    this.streams.Add(streamViewModel);
                    this.SaveSettings(); // Save immediately after adding
                    this.StartStream(streamViewModel);

                    // Clear inputs
                    this.CameraUrlEntry.Text = MainPage.EmptyString;
                    this.CameraNameEntry.Text = MainPage.EmptyString;

                    this.UpdateStatus(string.Format(MainPage.AddedStreamFormat, cameraName));
                }
                else
                {
                    this.DisplayAlert(MainPage.ErrorTitle, MainPage.CameraUrlAlreadyAddedMessage, MainPage.OkButtonText);
                }
            }
            else
            {
                this.DisplayAlert(MainPage.ErrorTitle, MainPage.InvalidCameraUrlMessage, MainPage.OkButtonText);
            }
        }

        private void OnRemoveStreamClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int id)
            {
                var stream = this.streams.FirstOrDefault(s => s.Id == id);
                if (stream != null)
                {
                    this.StopStream(stream);
                    this.streams.Remove(stream);
                    this.SaveSettings(); // Save immediately after removing
                    this.UpdateStatus(string.Format(MainPage.RemovedStreamFormat, stream.CameraName));
                }
            }
        }

        private void OnToggleStreamClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int id)
            {
                var stream = this.streams.FirstOrDefault(s => s.Id == id);
                if (stream != null)
                {
                    if (stream.IsRunning)
                    {
                        this.StopStream(stream);
                        this.UpdateStatus(string.Format(MainPage.StoppedStreamFormat, stream.CameraName));
                    }
                    else
                    {
                        this.StartStream(stream);
                        this.UpdateStatus(string.Format(MainPage.StartedStreamFormat, stream.CameraName));
                    }
                }
            }
        }

        private void OnStopAllClicked(object sender, EventArgs e)
        {
            foreach (var stream in this.streams)
            {
                this.StopStream(stream);
            }
            
            this.UpdateStatus(MainPage.AllStreamsStoppedText);
        }

        private void OnClearStreamLogsClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int id)
            {
                var stream = this.streams.FirstOrDefault(s => s.Id == id);
                if (stream != null)
                {
                    stream.DetectionLogs.Clear();
                }
            }
        }

        private void OnClearAllLogsClicked(object sender, EventArgs e)
        {
            foreach (var stream in this.streams)
            {
                stream.DetectionLogs.Clear();
            }
            this.UpdateStatus(MainPage.AllLogsClearedText);
        }

        private bool isTileView = false;
        private int gridColumnsPerRow = 3;
        private readonly Dictionary<int, CollectionView> gridCollections = new();
        private CollectionView? currentGridCollection;

        private CollectionView GetOrCreateGridCollection(int columns)
        {
            if (!this.gridCollections.TryGetValue(columns, out var collectionView))
            {
                // Create new CollectionView dynamically for this column count
                collectionView = new CollectionView
                {
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = (DataTemplate)this.Resources["TileTemplate"],
                    ItemsSource = this.streams,
                    IsVisible = false,
                    ItemsLayout = new GridItemsLayout(columns, ItemsLayoutOrientation.Vertical)
                    {
                        HorizontalItemSpacing = 10,
                        VerticalItemSpacing = 10
                    }
                };
                this.gridCollections[columns] = collectionView;
                this.GridContainer.Children.Add(collectionView);
            }
            return collectionView;
        }

        private void ShowGridForColumns(int columns)
        {
            // Hide current grid collection if any
            if (this.currentGridCollection != null)
                this.currentGridCollection.IsVisible = false;

            // Get or create CollectionView for this column count and show it
            this.currentGridCollection = this.GetOrCreateGridCollection(columns);
            this.currentGridCollection.IsVisible = true;
        }

        private void OnGridColumnsEntryTextChanged(object? sender, TextChangedEventArgs e)
        {
            // Optional: validate as user types, but don't apply until they press Apply or Enter
        }

        private void OnGridColumnsEntryCompleted(object? sender, EventArgs e)
        {
            this.ApplyGridColumns();
        }

        private void OnGridColumnsApplyClicked(object? sender, EventArgs e)
        {
            this.ApplyGridColumns();
        }

        private void ApplyGridColumns()
        {
            if (string.IsNullOrWhiteSpace(this.GridColumnsEntry.Text))
                return;

            if (int.TryParse(this.GridColumnsEntry.Text, out int columns) && columns >= 1)
            {
                this.gridColumnsPerRow = columns;
                if (this.isTileView)
                    this.ShowGridForColumns(this.gridColumnsPerRow);
            }
            else
            {
                // Invalid input - reset to current value
                this.GridColumnsEntry.Text = this.gridColumnsPerRow.ToString();
                this.DisplayAlert("Invalid Input", "Please enter a valid number greater than or equal to 1.", "OK");
            }
        }

        private void OnListViewClicked(object sender, EventArgs e)
        {
            if (this.isTileView)
            {
                this.isTileView = false;
                this.StreamsListCollection.IsVisible = true;
                this.GridContainer.IsVisible = false;
                this.GridColumnsPanel.IsVisible = false;
                this.ListViewButton.IsEnabled = false;
                this.GridViewButton.IsEnabled = true;
            }
        }

        private void OnGridViewClicked(object sender, EventArgs e)
        {
            if (!this.isTileView)
            {
                this.isTileView = true;
                this.StreamsListCollection.IsVisible = false;
                this.GridContainer.IsVisible = true;
                this.GridColumnsPanel.IsVisible = true;
                this.GridColumnsEntry.Text = this.gridColumnsPerRow.ToString();
                this.ListViewButton.IsEnabled = true;
                this.GridViewButton.IsEnabled = false;
                this.ShowGridForColumns(this.gridColumnsPerRow);
            }
        }

        private void OnTileSizeChanged(object? sender, EventArgs e)
        {
            if (sender is Border border && border.Width > 0)
            {
                // Maintain 4:3 aspect ratio: height = width * 3/4
                border.HeightRequest = border.Width * 0.75;
            }
        }

        private void StartStream(CameraStreamViewModel streamViewModel)
        {
            if (streamViewModel.Streamer != null)
            {
                this.StopStream(streamViewModel);
            }

            // Convert percentage to ratio (1.5% = 0.015)
            float thresholdRatio = (float)(streamViewModel.MotionThresholdPercent / 100.0);
            var streamer = new MjpegStreamer(new HttpClient(), differenceThresholdRatio: thresholdRatio);
            streamer.FrameReceived += (jpegBytes) => this.OnFrameReceived(streamViewModel, jpegBytes);
            streamer.Metrics += (ratio, changed, total) => this.OnMetrics(streamViewModel, ratio, changed, total);
            streamer.MotionDetected += () => this.OnMotion(streamViewModel);
            streamer.Error += (message) => this.OnError(streamViewModel, message);

            // Initialize frame buffer if recording is enabled (uses camera-specific duration)
            if (streamViewModel.RecordingEnabled)
            {
                streamViewModel.FrameBuffer = new FrameBuffer(streamViewModel.RecordingBeforeSeconds, estimatedFps: 15);
            }

            streamViewModel.Streamer = streamer;
            streamViewModel.IsRunning = true;
            streamer.Start(streamViewModel.Url);
        }

        private async void StopStream(CameraStreamViewModel streamViewModel)
        {
            if (streamViewModel.Streamer != null)
            {
                var streamer = streamViewModel.Streamer;
                streamViewModel.Streamer = null;
                streamViewModel.IsRunning = false;

                try
                {
                    await streamer.DisposeAsync();
                }
                catch { }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    streamViewModel.CurrentFrame = null;
                    streamViewModel.Metrics = MainPage.MetricsResetText;
                    streamViewModel.MotionStatus = MainPage.MotionIdleText;
                    streamViewModel.MotionColor = Colors.Gray;
                });
            }
        }

        private void OnFrameReceived(CameraStreamViewModel streamViewModel, byte[] jpegBytes)
        {
            long timestampMs = Environment.TickCount64;

            // Store current frame bytes for OCR processing
            streamViewModel.CurrentFrameBytes = jpegBytes;

            // Add to frame buffer if recording is enabled
            if (streamViewModel.RecordingEnabled && streamViewModel.FrameBuffer != null)
            {
                streamViewModel.FrameBuffer.AddFrame(jpegBytes, timestampMs);
            }

            // Add to recording frames if currently recording
            if (streamViewModel.IsRecording)
            {
                streamViewModel.RecordingFrames.Add(new FrameData
                {
                    JpegBytes = jpegBytes,
                    TimestampMs = timestampMs
                });

                // Log every frame during recording to debug the issue
                System.Diagnostics.Debug.WriteLine($"[RECORDING] Frame added! Total: {streamViewModel.RecordingFrames.Count}, IsRecording: {streamViewModel.IsRecording}");
            }
            else
            {
                // Debug: Log when frames are NOT being added
                if (streamViewModel.RecordingEnabled)
                {
                    System.Diagnostics.Debug.WriteLine($"[RECORDING] Frame NOT added - IsRecording is FALSE");
                }
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                streamViewModel.CurrentFrame = ImageSource.FromStream(() => new MemoryStream(jpegBytes));
            });
        }

        private void OnMetrics(CameraStreamViewModel streamViewModel, float ratio, int changed, int total)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                streamViewModel.Metrics = string.Format(MainPage.MetricsFormat, ratio, changed, total);
                streamViewModel.LastRatio = ratio;
            });
        }

        private async void OnMotion(CameraStreamViewModel streamViewModel)
        {
            var detectionTime = DateTime.Now;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                streamViewModel.MotionStatus = MainPage.MotionDetectedText;
                streamViewModel.MotionColor = Colors.OrangeRed;

                // 🔥 Highlight the camera border
                streamViewModel.IsMotionHighlighted = true;
                System.Diagnostics.Debug.WriteLine($"[HIGHLIGHT] Camera '{streamViewModel.CameraName}' highlighted");

                // Auto-clear highlight after 3 seconds
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        streamViewModel.IsMotionHighlighted = false;
                        System.Diagnostics.Debug.WriteLine($"[HIGHLIGHT] Camera '{streamViewModel.CameraName}' highlight cleared");
                    });
                });

                var timestamp = detectionTime.ToString("HH:mm:ss");
                
                // Add log entry
                streamViewModel.DetectionLogs.Add(string.Format(MainPage.MotionDetectedLogFormat, timestamp, streamViewModel.LastRatio));

                if (streamViewModel.DetectionLogs.Count > MainPage.MaxDetectionLogs)
                {
                    streamViewModel.DetectionLogs.RemoveAt(0);
                }

                // Play sound if enabled
                this.PlayMotionSound(streamViewModel);

                // Start recording if enabled AND not already recording
                if (streamViewModel.RecordingEnabled && !streamViewModel.IsRecording)
                {
                    System.Diagnostics.Debug.WriteLine($"[RECORDING] Motion detected - starting new recording");
                    this.StartRecording(streamViewModel, detectionTime);
                }
                else if (streamViewModel.IsRecording)
                {
                    System.Diagnostics.Debug.WriteLine($"[RECORDING] Motion detected but ALREADY RECORDING - ignoring to prevent frame loss");
                }
            });
        }

        private void StartRecording(CameraStreamViewModel streamViewModel, DateTime detectionTime)
        {
            System.Diagnostics.Debug.WriteLine($"[RECORDING] StartRecording called for {streamViewModel.CameraName}");
            System.Diagnostics.Debug.WriteLine($"[RECORDING] RecordingEnabled: {streamViewModel.RecordingEnabled}");
            System.Diagnostics.Debug.WriteLine($"[RECORDING] MP4: {streamViewModel.RecordingMp4}, GIF: {streamViewModel.RecordingGif}, PNG: {streamViewModel.RecordingPng}");

            streamViewModel.IsRecording = true;
            streamViewModel.RecordingStartTime = detectionTime;
            streamViewModel.RecordingFrames.Clear();

            // Get frames from buffer (uses camera-specific duration before detection)
            if (streamViewModel.FrameBuffer != null)
            {
                long currentTimestamp = Environment.TickCount64;
                var bufferedFrames = streamViewModel.FrameBuffer.GetFrames(currentTimestamp, streamViewModel.RecordingBeforeSeconds);
                streamViewModel.RecordingFrames.AddRange(bufferedFrames);
                System.Diagnostics.Debug.WriteLine($"[RECORDING] Added {bufferedFrames.Count} buffered frames ({streamViewModel.RecordingBeforeSeconds} seconds BEFORE detection)");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[RECORDING] WARNING: FrameBuffer is null!");
            }

            // Record for the configured duration after detection
            int afterSeconds = streamViewModel.RecordingAfterSeconds;
            System.Diagnostics.Debug.WriteLine($"[RECORDING] Starting {afterSeconds}-second capture period (AFTER detection)...");

            // Start task to stop recording after the configured duration
            _ = Task.Run(async () =>
            {
                await Task.Delay(afterSeconds * 1000);

                System.Diagnostics.Debug.WriteLine($"[RECORDING] {afterSeconds} seconds elapsed, stopping recording...");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (streamViewModel.IsRecording)
                    {
                        System.Diagnostics.Debug.WriteLine($"[RECORDING] Total frames captured: {streamViewModel.RecordingFrames.Count}");
                        this.StopRecording(streamViewModel, detectionTime);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[RECORDING] WARNING: IsRecording is false, skipping StopRecording");
                    }
                });
            });
        }

        private async void StopRecording(CameraStreamViewModel streamViewModel, DateTime detectionTime)
        {
            System.Diagnostics.Debug.WriteLine($"[RECORDING] StopRecording called");
            System.Diagnostics.Debug.WriteLine($"[RECORDING] Final frame count: {streamViewModel.RecordingFrames.Count}");

            streamViewModel.IsRecording = false;

            // Check if any format is selected
            if (!streamViewModel.RecordingGif && !streamViewModel.RecordingPng && !streamViewModel.RecordingMp4)
            {
                System.Diagnostics.Debug.WriteLine($"[RECORDING] WARNING: No output format selected! Aborting save.");
                streamViewModel.RecordingFrames.Clear();
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[RECORDING] Formats enabled - GIF: {streamViewModel.RecordingGif}, PNG: {streamViewModel.RecordingPng}, MP4: {streamViewModel.RecordingMp4}");

            // Get output directory
            string outputDir = streamViewModel.RecordingOutputPath ??
                Path.Combine(FileSystem.AppDataDirectory, "Recordings", streamViewModel.CameraName);

            System.Diagnostics.Debug.WriteLine($"[RECORDING] Output directory: {outputDir}");

            // IMPORTANT: Create a copy of frames BEFORE clearing, to avoid race condition
            var framesToSave = new List<FrameData>(streamViewModel.RecordingFrames);
            streamViewModel.RecordingFrames.Clear();  // Clear immediately so new recording can start

            System.Diagnostics.Debug.WriteLine($"[RECORDING] Created copy of {framesToSave.Count} frames for saving");

            try
            {
                System.Diagnostics.Debug.WriteLine($"[RECORDING] Calling SaveRecordingAsync with {framesToSave.Count} frames...");

                await this.detectionRecorder.SaveRecordingAsync(
                    framesToSave,  // Use the copy, not the original list
                    streamViewModel.CameraName,
                    outputDir,
                    streamViewModel.RecordingBeforeSeconds,
                    streamViewModel.RecordingAfterSeconds,
                    streamViewModel.RecordingGif,
                    streamViewModel.RecordingPng,
                    streamViewModel.RecordingMp4,
                    detectionTime);

                System.Diagnostics.Debug.WriteLine($"[RECORDING] SaveRecordingAsync completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RECORDING] ERROR saving recording: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[RECORDING] Stack trace: {ex.StackTrace}");
            }
        }

        private void PlayMotionSound(CameraStreamViewModel streamViewModel)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(MainPage.DebugPlayMotionSoundCalled);

                // Check if sound is enabled for this camera
                if (streamViewModel == null || !streamViewModel.SoundEnabled)
                {
                    System.Diagnostics.Debug.WriteLine("PlayMotionSound: Sound disabled for this camera");
                    return;
                }

                // Try to get audio service if not already resolved
                if (this.audioService == null)
                {
                    System.Diagnostics.Debug.WriteLine(MainPage.DebugAudioServiceNull);
                    var app = Application.Current;
                    if (app?.Handler?.MauiContext?.Services != null)
                    {
                        this.audioService = app.Handler.MauiContext.Services.GetService<IAudioService>();
                        System.Diagnostics.Debug.WriteLine(string.Format(MainPage.DebugServiceResolved, this.audioService != null));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(MainPage.DebugApplicationNull);
                    }
                }

                // If still null, can't play sound
                if (this.audioService != null)
                {
                    // Get the camera-specific sound file path
                    string? soundFilePath = streamViewModel.SoundFilePath;
                    System.Diagnostics.Debug.WriteLine(string.Format(MainPage.DebugSoundFilePath, soundFilePath ?? "(null)"));

                    if (!string.IsNullOrEmpty(soundFilePath) && File.Exists(soundFilePath))
                    {
                        // Get the camera-specific volume (default to 1.0 if not set)
                        double volume = streamViewModel.SoundVolume;
                        System.Diagnostics.Debug.WriteLine(string.Format(MainPage.DebugCallingPlaySound, soundFilePath));
                        // Play the sound with camera-specific volume
                        this.audioService.PlaySound(soundFilePath, volume);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(soundFilePath))
                        {
                            System.Diagnostics.Debug.WriteLine(MainPage.DebugNoSoundFilePath);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format(MainPage.DebugFileNotExists, soundFilePath));
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(MainPage.DebugAudioServiceStillNull);
                }
            }
            catch (Exception ex)
            {
                // Log error for debugging (can be removed in production)
                System.Diagnostics.Debug.WriteLine(string.Format(MainPage.DebugExceptionFormat, ex.GetType().Name, ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format(MainPage.DebugStackTrace, ex.StackTrace));
            }
        }

        private void OnError(CameraStreamViewModel streamViewModel, string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.DisplayAlert(string.Format(MainPage.StreamErrorTitleFormat, streamViewModel.CameraName), message, MainPage.OkButtonText);
                streamViewModel.IsRunning = false;
            });
        }

        private void UpdateStatus(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.StatusLabel.Text = string.Format(MainPage.StatusFormat, message, this.streams.Count(s => s.IsRunning), this.streams.Count);
            });
        }

        private void OnThresholdChanged(object sender, ValueChangedEventArgs e)
        {
            if (sender is Slider slider && slider.BindingContext is CameraStreamViewModel streamViewModel)
            {
                // Update the view model property (binding will handle this, but we also need to update the streamer)
                streamViewModel.MotionThresholdPercent = e.NewValue;

                // Update the streamer's threshold if it's running
                if (streamViewModel.Streamer != null)
                {
                    float thresholdRatio = (float)(e.NewValue / 100.0);
                    streamViewModel.Streamer.DifferenceThresholdRatio = thresholdRatio;
                }
            }
        }

        private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            if (sender is Slider slider && slider.BindingContext is CameraStreamViewModel streamViewModel)
            {
                // Update the view model property (binding and persistence will be handled by the property setter)
                streamViewModel.SoundVolume = e.NewValue;
            }
        }

        private void OnSoundEnabledToggled(object sender, ToggledEventArgs e)
        {
            if (sender is Switch switchControl && switchControl.BindingContext is CameraStreamViewModel streamViewModel)
            {
                streamViewModel.SoundEnabled = e.Value;
            }
        }

        private async void OnSelectSoundFileClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int id)
            {
                var stream = this.streams.FirstOrDefault(s => s.Id == id);
                if (stream != null)
                {
                    try
                    {
                        var customFileType = new FilePickerFileType(
                            new Dictionary<DevicePlatform, IEnumerable<string>>
                            {
                                { DevicePlatform.WinUI, new[] { ".wav" } }
                            });

                        var options = new PickOptions
                        {
                            PickerTitle = "Select a WAV file for this camera",
                            FileTypes = customFileType
                        };

                        var result = await FilePicker.Default.PickAsync(options);
                        if (result != null)
                        {
                            stream.SoundFilePath = result.FullPath;
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.DisplayAlert(MainPage.ErrorTitle, $"Failed to select file: {ex.Message}", MainPage.OkButtonText);
                    }
                }
            }
        }

        private void OnClearSoundFileClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int id)
            {
                var stream = this.streams.FirstOrDefault(s => s.Id == id);
                if (stream != null)
                {
                    stream.SoundFilePath = null;
                }
            }
        }

        private void OnTestSoundClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int id)
            {
                var stream = this.streams.FirstOrDefault(s => s.Id == id);
                if (stream != null && stream.SoundEnabled && !string.IsNullOrEmpty(stream.SoundFilePath) && File.Exists(stream.SoundFilePath))
                {
                    try
                    {
                        if (this.audioService == null)
                        {
                            var app = Application.Current;
                            if (app?.Handler?.MauiContext?.Services != null)
                            {
                                this.audioService = app.Handler.MauiContext.Services.GetService<IAudioService>();
                            }
                        }

                        if (this.audioService != null)
                        {
                            this.audioService.PlaySound(stream.SoundFilePath, stream.SoundVolume);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.DisplayAlert(MainPage.ErrorTitle, $"Failed to play sound: {ex.Message}", MainPage.OkButtonText);
                    }
                }
            }
        }

        private void OnRecordingEnabledToggled(object sender, ToggledEventArgs e)
        {
            if (sender is Switch switchControl && switchControl.BindingContext is CameraStreamViewModel streamViewModel)
            {
                streamViewModel.RecordingEnabled = e.Value;

                // Initialize or clear frame buffer based on setting (uses camera-specific duration)
                if (e.Value && streamViewModel.IsRunning)
                {
                    streamViewModel.FrameBuffer = new FrameBuffer(streamViewModel.RecordingBeforeSeconds, estimatedFps: 15);
                }
                else if (!e.Value)
                {
                    streamViewModel.FrameBuffer = null;
                }
            }
        }



        private void OnRecordingFormatChanged(object sender, CheckedChangedEventArgs e)
        {
            // Format changes are handled by binding, this is just for any additional logic if needed
        }

        private void OnRecordingBeforeDurationChanged(object sender, ValueChangedEventArgs e)
        {
            if (sender is Slider slider && slider.BindingContext is CameraStreamViewModel streamViewModel)
            {
                // Update the view model property only if it actually changed
                int newDuration = (int)e.NewValue;
                if (streamViewModel.RecordingBeforeSeconds != newDuration)
                {
                    streamViewModel.RecordingBeforeSeconds = newDuration;

                    // If recording is enabled, we need to update the frame buffer size
                    if (streamViewModel.RecordingEnabled && streamViewModel.IsRunning)
                    {
                        // Recreate buffer with new duration
                        // Note: This will clear existing buffered frames, but ensures correct duration going forward
                        streamViewModel.FrameBuffer = new FrameBuffer(newDuration, estimatedFps: 15);
                        System.Diagnostics.Debug.WriteLine($"[RECORDING] Updated buffer duration to {newDuration} seconds");
                    }
                }
            }
        }

        private void OnRecordingAfterDurationChanged(object sender, ValueChangedEventArgs e)
        {
            if (sender is Slider slider && slider.BindingContext is CameraStreamViewModel streamViewModel)
            {
                // Update the view model property only if it actually changed
                int newDuration = (int)e.NewValue;
                if (streamViewModel.RecordingAfterSeconds != newDuration)
                {
                    streamViewModel.RecordingAfterSeconds = newDuration;
                    System.Diagnostics.Debug.WriteLine($"[RECORDING] Updated recording duration to {newDuration} seconds");
                }
            }
        }

        private async void OnSelectOutputFolderClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int id)
            {
                var stream = this.streams.FirstOrDefault(s => s.Id == id);
                if (stream != null)
                {
                    try
                    {
                        // For folder picker, we'll use a simple approach
                        // Note: MAUI doesn't have a built-in folder picker, so we'll use a workaround
                        // For Windows, we can use platform-specific code, but for now we'll use a text input
                        string? result = await this.DisplayPromptAsync(
                            "Output Folder",
                            "Enter the folder path for recordings:",
                            initialValue: stream.RecordingOutputPath ?? Path.Combine(FileSystem.AppDataDirectory, "Recordings", stream.CameraName),
                            placeholder: "Folder path");

                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            stream.RecordingOutputPath = result;
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.DisplayAlert(MainPage.ErrorTitle, $"Failed to set output folder: {ex.Message}", MainPage.OkButtonText);
                    }
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            foreach (var stream in this.streams)
            {
                this.StopStream(stream);
            }
        }
    }
}
