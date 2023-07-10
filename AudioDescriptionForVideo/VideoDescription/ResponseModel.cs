using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    internal class ResponseModel
    {
        public string Caption { get; set; }
        public string[] Objects { get; set; }

        public string[] tags { get; set; }

        public string ocrText { get; set; }

        public string[] landmarks { get; set; }

        public string imageType { get; set; }

        public string colorScheme { get; set; }
    }
}
