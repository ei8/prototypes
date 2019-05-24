using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactor.ResultMarkers
{
    public class ResultMarkerEventArgs : EventArgs
    {
        public ResultMarkerEventArgs(ResultMarker marker)
        {
            this.Marker = marker;
        }

        public ResultMarker Marker { get; private set; }
    }
}
