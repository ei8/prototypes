using System.Collections.Generic;
using System.Linq;

namespace IPCameraViewer.Services
{
    public class FrameBuffer
    {
        private readonly Queue<FrameData> frames = new();
        private readonly object lockObject = new();
        private readonly int maxFrames;
        private readonly int maxDurationMs;

        public FrameBuffer(int maxDurationSeconds, int estimatedFps = 15)
        {
            // Calculate max frames needed (add 20% buffer)
            this.maxFrames = (int)(maxDurationSeconds * estimatedFps * 1.2);
            this.maxDurationMs = maxDurationSeconds * 1000;
        }

        public void AddFrame(byte[] jpegBytes, long timestampMs)
        {
            lock (this.lockObject)
            {
                // Remove old frames that exceed the duration
                while (this.frames.Count > 0)
                {
                    var oldestFrame = this.frames.Peek();
                    if (timestampMs - oldestFrame.TimestampMs > this.maxDurationMs)
                    {
                        this.frames.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }

                // Remove oldest if we exceed max frames
                if (this.frames.Count >= this.maxFrames)
                {
                    this.frames.Dequeue();
                }

                this.frames.Enqueue(new FrameData
                {
                    JpegBytes = jpegBytes,
                    TimestampMs = timestampMs
                });
            }
        }

        public List<FrameData> GetFrames(long currentTimestampMs, int beforeSeconds)
        {
            lock (this.lockObject)
            {
                long cutoffTime = currentTimestampMs - (beforeSeconds * 1000);
                return this.frames
                    .Where(f => f.TimestampMs >= cutoffTime)
                    .ToList();
            }
        }

        public List<FrameData> GetFrames()
        {
            lock (this.lockObject)
            {
                return new List<FrameData>(this.frames);
            }
        }

        public void Clear()
        {
            lock (this.lockObject)
            {
                this.frames.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.frames.Count;
                }
            }
        }
    }

    public class FrameData
    {
        public byte[] JpegBytes { get; set; } = Array.Empty<byte>();
        public long TimestampMs { get; set; }
    }
}

