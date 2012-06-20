using System;
using System.Xml;
using RoundScheduler.Model;

namespace RoundScheduler.Dto
{
    [Serializable]
    public class RoundSerializable
    {
        public RoundSerializable()
        {
        }

        public RoundSerializable(Round round)
        {
            this.RoundTime = XmlConvert.ToString(round.RoundTime);
            this.RestTime = XmlConvert.ToString(round.RestTime);
        }

        public string RoundTime { get; set; }
        public string RestTime { get; set; }

        public Round GetRound()
        {
            return new Round(XmlConvert.ToTimeSpan(RoundTime), XmlConvert.ToTimeSpan(RestTime));
        }
    }
}
