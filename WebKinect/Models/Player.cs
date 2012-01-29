using System;
using System.Collections.Generic;
using WebKinect.Utils;

using Microsoft.Research.Kinect.Nui;

using System.Windows;
using System.Diagnostics;

namespace WebKinect
{
    public class Player
    {
        private static Dictionary<int, Player> _players;

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
            lastUpdatedAt = DateTime.Now;
        }

        #region Instance Variables
        public DateTime lastUpdatedAt;
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
        public static Player FindByIndex(int i)
        {
            if (_players == null) _players = new Dictionary<int, Player>();

            if (!_players.ContainsKey(i)) return null;

            return _players[i];
        }

        public static Player FindOrCreateByIndex(int i)
        {
            var p = FindByIndex(i);

            if (null == p)
            {
                p = new Player(i);
                _players[i] = p;
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
            lastUpdatedAt = DateTime.Now;

            // Update player's bone and joint positions
            if (data.Joints.Count > 0)
            {
                isAlive = true;

                foreach (Joint joint in data.Joints)
                {
                    Trace.WriteLine("Joint data found X:" + joint.Position.X.ToString());
                    Trace.WriteLine(joint.ToString());
                }
                // Head, hands, feet (hit testing happens in order here)
                //UpdateJointPosition(data.Joints, JointID.Head);
                //UpdateJointPosition(data.Joints, JointID.ShoulderLeft);
                //UpdateJointPosition(data.Joints, JointID.ShoulderCenter);
                //UpdateJointPosition(data.Joints, JointID.ShoulderRight);

                //UpdateJointPosition(data.Joints, JointID.ElbowLeft);
                //UpdateJointPosition(data.Joints, JointID.ElbowRight);
                //UpdateJointPosition(data.Joints, JointID.WristLeft);
                //UpdateJointPosition(data.Joints, JointID.WristRight);
                //UpdateJointPosition(data.Joints, JointID.HandLeft);
                //UpdateJointPosition(data.Joints, JointID.HandRight);

                //UpdateJointPosition(data.Joints, JointID.HipLeft);
                //UpdateJointPosition(data.Joints, JointID.HipCenter);
                //UpdateJointPosition(data.Joints, JointID.HipRight);
                //UpdateJointPosition(data.Joints, JointID.KneeLeft);
                //UpdateJointPosition(data.Joints, JointID.KneeRight);
                //UpdateJointPosition(data.Joints, JointID.AnkleLeft);
                //UpdateJointPosition(data.Joints, JointID.AnkleRight);
                //UpdateJointPosition(data.Joints, JointID.FootLeft);
                //UpdateJointPosition(data.Joints, JointID.FootRight);

                //// Hands and arms
                //UpdateBonePosition(data.Joints, JointID.HandRight, JointID.WristRight);
                //UpdateBonePosition(data.Joints, JointID.WristRight, JointID.ElbowRight);
                //UpdateBonePosition(data.Joints, JointID.ElbowRight, JointID.ShoulderRight);

                //UpdateBonePosition(data.Joints, JointID.HandLeft, JointID.WristLeft);
                //UpdateBonePosition(data.Joints, JointID.WristLeft, JointID.ElbowLeft);
                //UpdateBonePosition(data.Joints, JointID.ElbowLeft, JointID.ShoulderLeft);

                //// Head and Shoulders
                //UpdateBonePosition(data.Joints, JointID.ShoulderCenter, JointID.Head);
                //UpdateBonePosition(data.Joints, JointID.ShoulderLeft, JointID.ShoulderCenter);
                //UpdateBonePosition(data.Joints, JointID.ShoulderCenter, JointID.ShoulderRight);

                //// Legs
                //UpdateBonePosition(data.Joints, JointID.HipLeft, JointID.KneeLeft);
                //UpdateBonePosition(data.Joints, JointID.KneeLeft, JointID.AnkleLeft);
                //UpdateBonePosition(data.Joints, JointID.AnkleLeft, JointID.FootLeft);

                //UpdateBonePosition(data.Joints, JointID.HipRight, JointID.KneeRight);
                //UpdateBonePosition(data.Joints, JointID.KneeRight, JointID.AnkleRight);
                //UpdateBonePosition(data.Joints, JointID.AnkleRight, JointID.FootRight);

                //UpdateBonePosition(data.Joints, JointID.HipLeft, JointID.HipCenter);
                //UpdateBonePosition(data.Joints, JointID.HipCenter, JointID.HipRight);

                //// Spine
                //UpdateBonePosition(data.Joints, JointID.HipCenter, JointID.ShoulderCenter);
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