using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using WebKinect;
using System.Linq;
//using System.Windows;
//using System.Windows.Documents;
//using System.Windows.Input;
using Microsoft.Research.Kinect.Nui;
//using Microsoft.Samples.Kinect.WpfViewers;

namespace WebKinect.Models
{
    public class Kinect
    {
        private static Dictionary<String, Runtime> _sensors;

        #region Finders
        public static Runtime FindByIndex(int i)
        {
            if (i > _sensors.Count) return null;

            return new List<Runtime>(_sensors.Values)[i];
        }

        public static Runtime FindByInstanceName(String s)
        {
            Runtime r;
            if (_sensors.TryGetValue(s, out r))
                return r;
            else
                return null;
        }
        #endregion

        #region Setup/Teardown
        public static void Init()
        {
            Trace.WriteLine("Init()");
            if (null==_sensors)
            {
                _sensors = new Dictionary<String, Runtime>();
            }

            foreach (var kinect in Runtime.Kinects)
            {
                Trace.WriteLine(kinect.InstanceName);
                _sensors[kinect.InstanceName] = kinect;
                kinect.Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking);
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonsReady);
            }

            //Runtime.Kinects.StatusChanged += new EventHandler<StatusChangedEventArgs>(Kinects_StatusChanged);
        }

        public static void Shutdown()
        {
            foreach (var kinect in _sensors.Values)
            {
                Uninitialize(kinect);
            }
        }

        private static void Uninitialize(Runtime kinect)
        {
            kinect.Uninitialize();
            kinect.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonsReady);
        }
        #endregion

        #region Event Handlers

        // doesn't work
        //private static void Kinects_StatusChanged(object sender, StatusChangedEventArgs e)
        //{
        //    switch (e.Status)
        //    {
        //        case KinectStatus.Connected:
        //            if (!_sensors.ContainsKey(e.KinectRuntime.InstanceName))
        //            {
        //                _sensors[e.KinectRuntime.InstanceName] = e.KinectRuntime;
        //            }
        //            break;
        //        case KinectStatus.Disconnected:
        //            if (_sensors.ContainsKey(e.KinectRuntime.InstanceName))
        //            {
        //                Uninitialize(_sensors[e.KinectRuntime.InstanceName]);
        //                _sensors[e.KinectRuntime.InstanceName] = null;
        //            }
        //            break;
        //        default:
        //            if (e.Status.HasFlag(KinectStatus.Error))
        //            {
        //                if (_sensors.ContainsKey(e.KinectRuntime.InstanceName))
        //                {
        //                    Uninitialize(_sensors[e.KinectRuntime.InstanceName]);
        //                    _sensors[e.KinectRuntime.InstanceName] = null;
        //                }
        //            }
        //            break;
        //    }
        //}

        private static void SkeletonsReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            

                // SO the Kinect throws this event like 15 times per second or something. whether it has actual data or not.
                SkeletonFrame skeletonFrame = e.SkeletonFrame;

                //KinectSDK TODO: This nullcheck shouldn't be required. 
                //Unfortunately, this version of the Kinect Runtime will continue to fire some skeletonFrameReady events after the Kinect USB is unplugged.
                if (skeletonFrame == null)
                {
                    return;
                }

                int i = 0;

                foreach (SkeletonData data in skeletonFrame.Skeletons)
                {
                    // this conditional basically says only do it if a player is actually being tracked.
                    // stupid API always returns 6 player objects though. whether they are tracked or not.
                    if (SkeletonTrackingState.Tracked == data.TrackingState)
                    {
                        Trace.WriteLine("Tracked!");
                    }
                    Player.FindOrCreateByIndex(i).Update(data);
                    i++;
                }
            
        }
        #endregion


    }

}