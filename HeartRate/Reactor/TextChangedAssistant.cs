using System;
using System.Threading;

// https://stackoverflow.com/questions/33776387/dont-raise-textchanged-while-continuous-typing
namespace Reactor
{
    public class TextChangedTimer
    {
        public event EventHandler Idled = delegate { };
        public int WaitingDuration { get; set; }
        private Timer waitingTimer;

        public TextChangedTimer(int waitingDuration = 600)
        {
            this.WaitingDuration = waitingDuration;
            this.waitingTimer = new Timer(p =>
            {
                Idled(this, EventArgs.Empty);
            });
        }
        public void StartWaitTimer()
        {
            this.waitingTimer.Change(WaitingDuration, Timeout.Infinite);
        }
    }
}
