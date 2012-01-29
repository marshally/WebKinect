using System;
using Microsoft.Research.Kinect.Nui;

namespace WebKinect.Utils
{
    // For hit testing, a dictionary of BoneData items, keyed off the endpoints
    // of a segment (Bone) is used.  The velocity of these endpoints is estimated
    // and used during hit testing and updating velocity vectors after a hit.

    public struct Bone
    {
        public JointID joint1;
        public JointID joint2;
        public Bone(JointID j1, JointID j2)
        {
            joint1 = j1;
            joint2 = j2;
        }
    }

    public struct Segment
    {
        public double x1;
        public double y1;
        public double x2;
        public double y2;
        public double radius;

        public Segment(double x, double y)
        {
            radius = 1;
            x1 = x2 = x;
            y1 = y2 = y;
        }

        public Segment(double x_1, double y_1, double x_2, double y_2)
        {
            radius = 1;
            x1 = x_1;
            y1 = y_1;
            x2 = x_2;
            y2 = y_2;
        }

        public bool IsCircle()
        {
            return ((x1 == x2) && (y1 == y2));
        }
    }

    public struct BoneData
    {
        public Segment seg;
        public Segment segLast;
        public double xVel;
        public double yVel;
        public double xVel2;
        public double yVel2;
        private const double smoothing = 0.8;
        public DateTime timeLastUpdated;

        public BoneData(Segment s)
        {
            seg = segLast = s;
            xVel = yVel = 0;
            xVel2 = yVel2 = 0;
            timeLastUpdated = DateTime.Now;
        }

        // Update the segment's position and compute a smoothed velocity for the circle or the
        // endpoints of the segment based on  the time it took it to move from the last position
        // to the current one.  The velocity is in pixels per second.

        public void UpdateSegment(Segment s)
        {
            segLast = seg;
            seg = s;

            DateTime cur = DateTime.Now;
            double fMs = cur.Subtract(timeLastUpdated).TotalMilliseconds;
            if (fMs < 10.0)
                fMs = 10.0;
            double fFPS = 1000.0 / fMs;
            timeLastUpdated = cur;

            if (seg.IsCircle())
            {
                xVel = xVel * smoothing + (1.0 - smoothing) * (seg.x1 - segLast.x1) * fFPS;
                yVel = yVel * smoothing + (1.0 - smoothing) * (seg.y1 - segLast.y1) * fFPS;
            }
            else
            {
                xVel = xVel * smoothing + (1.0 - smoothing) * (seg.x1 - segLast.x1) * fFPS;
                yVel = yVel * smoothing + (1.0 - smoothing) * (seg.y1 - segLast.y1) * fFPS;
                xVel2 = xVel2 * smoothing + (1.0 - smoothing) * (seg.x2 - segLast.x2) * fFPS;
                yVel2 = yVel2 * smoothing + (1.0 - smoothing) * (seg.y2 - segLast.y2) * fFPS;
            }
        }

        // Using the velocity calculated above, estimate where the segment is right now.

        public Segment GetEstimatedSegment(DateTime cur)
        {
            Segment estimate = seg;
            double fMs = cur.Subtract(timeLastUpdated).TotalMilliseconds;
            estimate.x1 += fMs * xVel / 1000.0;
            estimate.y1 += fMs * yVel / 1000.0;
            if (seg.IsCircle())
            {
                estimate.x2 = estimate.x1;
                estimate.y2 = estimate.y1;
            }
            else
            {
                estimate.x2 += fMs * xVel2 / 1000.0;
                estimate.y2 += fMs * yVel2 / 1000.0;
            }
            return estimate;
        }
    }

    public enum PolyType
    {
        None = 0x00,
        Triangle = 0x01,
        Square = 0x02,
        Star = 0x04,
        Pentagon = 0x08,
        Hex = 0x10,
        Star7 = 0x20,
        Circle = 0x40,
        Bubble = 0x80,
        All = 0x7f
    }

    public enum HitType
    {
        None = 0x00,
        Hand = 0x01,
        Arm = 0x02,
        Squeezed = 0x04,
        Popped = 0x08
    }
}