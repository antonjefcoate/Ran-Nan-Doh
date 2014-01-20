using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCQRS
{
    public class Event : Message
    {
        public int Version;
    }
}
