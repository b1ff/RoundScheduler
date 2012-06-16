using System;
using RoundScheduler.Model;

namespace RoundScheduler.Events
{
    public delegate void RoundEndedEventHandler(object sender, RoundEndedEventArgs e);

    public class RoundEndedEventArgs : EventArgs
    {
        public RoundEndedEventArgs(Round lastRound)
        {
            LastRound = lastRound;
        }

        public Round LastRound { get; set; }
    }
}
