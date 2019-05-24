using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactor.ResultMarkers
{
    public class ResultMarkerService : IResultMarkerService
    {
        private IList<ResultMarker> resultMarkers = new List<ResultMarker>();
        private DateTime initializedTime;

        public IEnumerable<ResultMarker> Markers => this.resultMarkers;

        public event EventHandler Initialized;
        public event EventHandler<ResultMarkerEventArgs> Updated;
        public event EventHandler<ResultMarkerEventArgs> Added;
        public event EventHandler<ResultMarkerEventArgs> Removed;

        public void Add(ResultMarker value)
        {
            this.resultMarkers.Add(value);
            this.Added?.Invoke(this, new ResultMarkerEventArgs(value));
        }

        public void Initialize()
        {
            this.initializedTime = DateTime.Now;
            this.resultMarkers.ToList().ForEach(rm => { rm.Fired = false; rm.ElapsedTime = 0; });
            this.Initialized?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateIfMarker(string id)
        {
            var value = this.resultMarkers.FirstOrDefault(rm => rm.Id == id);
            if (value != null)
            {
                value.Fired = true;
                value.ElapsedTime = (int) DateTime.Now.Subtract(this.initializedTime).TotalMilliseconds;
                this.Updated?.Invoke(this, new ResultMarkerEventArgs(value));
            }
        }

        public void Remove(ResultMarker value)
        {
            this.resultMarkers.Remove(value);
            this.Removed?.Invoke(this, new ResultMarkerEventArgs(value));
        }
    }
}
