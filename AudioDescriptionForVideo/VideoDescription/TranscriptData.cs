using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDescription
{
    class TranscriptData
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public double Confidence { get; set; }
        public int SpeakerId { get; set; }
        public string Language { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }

}
