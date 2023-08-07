using System;
using System.Timers;

namespace Kata_s_Logs
{
    class BugTimer
    {
        private Timer timer = new Timer();
        private DateTime startTime;

        public void StartTimer(double timerInterval)
        {
            timer.Interval = timerInterval;
            startTime = DateTime.Now.AddMilliseconds(timerInterval);
            timer.Start();
        }

        /// <summary>
        /// Return miliseconds left on timer
        /// </summary>
        /// <returns>timeRemaining</returns>
        public double TimeRemaining()
        {
            return (startTime - DateTime.Now).TotalMilliseconds; 
        }

        public bool isEnabled()
        {
            return timer.Enabled;
        }
    }
}
