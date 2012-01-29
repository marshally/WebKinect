using System;
using System.Collections.Generic;
using WebKinect.Utils;
using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Research.Kinect.Nui;

using System.Linq;
using System.Threading;
using System.Windows;
using System.Diagnostics;

namespace WebKinect.Models
{
    public class Player
    {
        public Player(int SkeletonSlot)
        {
            id = SkeletonSlot;

            // Generate one of 7 colors for player
            int[] iMixr = { 1, 1, 1, 0, 1, 0, 0 };
            int[] iMixg = { 1, 1, 0, 1, 0, 1, 0 };
            int[] iMixb = { 1, 0, 1, 1, 0, 0, 1 };
            byte[] iJointCols = { 245, 200 };
            byte[] iBoneCols = { 235, 160 };

            int i = colorId;
            colorId = (colorId + 1) % iMixr.GetLength(0);

            //brJoints = new SolidColorBrush(Color.FromRgb(iJointCols[iMixr[i]], iJointCols[iMixg[i]], iJointCols[iMixb[i]]));
            //brBones = new SolidColorBrush(Color.FromRgb(iBoneCols[iMixr[i]], iBoneCols[iMixg[i]], iBoneCols[iMixb[i]]));
        }

        public static ConcurrentBag<Player> Players;

        #region properties
        public ConcurrentStack<PointCloud> Clouds;
        #endregion

        #region Instance Variables
        public bool isAlive = false;

        int colorId;
        int id;

        //private Rect playerBounds;
        //private Point playerCenter;
        //private double playerScale;

        private const double BONE_SIZE = 0.01;
        private const double HEAD_SIZE = 0.075;
        private const double HAND_SIZE = 0.03;

        // Keeping track of all bone segments of interest as well as head, hands and feet
        public Dictionary<Bone, BoneData> segments = new Dictionary<Bone, BoneData>();

        #endregion

        #region Finders

        public static Player FindOrCreateByIndex(int i)
        {
            if (Players == null)
            {
                Players = new ConcurrentBag<Player>();
            }
            var p = Players.ElementAtOrDefault(i);

            if (null == p)
            {
                p = new Player(i);
                Players.Add(p);
            }

            return p;

        }
        #endregion

        //public void setBounds(Rect r)
        //{
        //    playerBounds = r;
        //    playerCenter.X = (playerBounds.Left + playerBounds.Right) / 2;
        //    playerCenter.Y = (playerBounds.Top + playerBounds.Bottom) / 2;
        //    playerScale = Math.Min(playerBounds.Width, playerBounds.Height / 2);
        //}

        public void Update(SkeletonData data)
        {
            
                if (Clouds == null)
                {
                    Clouds = new ConcurrentStack<PointCloud>();
                }

                TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                //double timestamp = t.TotalMilliseconds;


                // Update player's bone and joint positions
                if (data.Joints.Count > 0)
                {
                    var c = new PointCloud() { Time = t.TotalMilliseconds, Positions = new ConcurrentBag<Position>() };
                    Trace.WriteLine("Joint");
                    foreach (Joint j in data.Joints)
                    {

                        if (j.TrackingState == JointTrackingState.Tracked)
                        {
                            

                            var p = new Position
                            {
                                X = j.Position.X,
                                Y = j.Position.Y,
                                Z = j.Position.Z,
                                W = j.Position.W,
                                Name = Enum.GetName(typeof(JointID), j.ID)
                            };
                            c.Positions.Add(p);
                        }
                    }

                    if (c.Positions.Count > 0 && 
                            (Clouds.Count() == 0 || Math.Abs(Clouds.First().Time - t.TotalMilliseconds) > 250)
                        )
                        Clouds.Push(c);

                    PointCloud extra;

                    while (Clouds.Count() > 120)
                        Clouds.TryPop(out extra);
                }
                else
                {
                    isAlive = false;
                }

            
        }
        //private void UpdateSegmentPosition(JointID j1, JointID j2, Segment seg)
        //{
        //    var bone = new Bone(j1, j2);
        //    if (segments.ContainsKey(bone))
        //    {
        //        BoneData data = segments[bone];
        //        data.UpdateSegment(seg);
        //        segments[bone] = data;
        //    }
        //    else
        //        segments.Add(bone, new BoneData(seg));
        //}

        //private void UpdateBonePosition(Microsoft.Research.Kinect.Nui.JointsCollection joints, JointID j1, JointID j2)
        //{
        //    var seg = new Segment(joints[j1].Position.X * playerScale + playerCenter.X,
        //                          playerCenter.Y - joints[j1].Position.Y * playerScale,
        //                          joints[j2].Position.X * playerScale + playerCenter.X,
        //                          playerCenter.Y - joints[j2].Position.Y * playerScale);
        //    seg.radius = Math.Max(3.0, playerBounds.Height * BONE_SIZE) / 2;
        //    UpdateSegmentPosition(j1, j2, seg);
        //}

        //private void UpdateJointPosition(Microsoft.Research.Kinect.Nui.JointsCollection joints, JointID j)
        //{
        //    var seg = new Segment(joints[j].Position.X * playerScale + playerCenter.X,
        //                          playerCenter.Y - joints[j].Position.Y * playerScale);
        //    seg.radius = playerBounds.Height * ((j == JointID.Head) ? HEAD_SIZE : HAND_SIZE) / 2;
        //    UpdateSegmentPosition(j, j, seg);
        //}
    }
}