using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace WebKinect.Models
{
    public class PointCloud
    {
        public ConcurrentBag<Position> Positions;
        public double Time { get; set; }
    }
}