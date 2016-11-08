﻿using Challange.Domain.Abstract;
using Challange.Domain.Entities;
using Challange.Domain.Services.StreamProcess.Abstract;
using Challange.Domain.Services.StreamProcess.Concrete.Pylon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PylonC.NETSupportLibrary.DeviceEnumerator;

namespace Challange.Domain.Services.StreamProcess.Concrete.Pylon
{
    public class CamerasContainer : ICamerasContainer
    {
        private List<ICamera> camerasContainer;

        private ICameraProvider cameraProvider;

        public CamerasContainer(ICameraProvider cameraProvider)
        {
            this.cameraProvider = cameraProvider;
            camerasContainer = new List<ICamera>();
        }

        public void InitializeCameras()
        {
            var deviceList = cameraProvider.GetConnectedCameras();
            ICamera tmpCamera;
            foreach (var cameraInfo in deviceList)
            {
                tmpCamera = new Camera(cameraInfo.Index, cameraInfo.FullName);
                AddCamera(tmpCamera);
            }
        }

        public int CamerasNumber
        {
            get
            {
                return camerasContainer.Count;
            }
        }

        public List<ICamera> GetCameras
        {
            get
            {
                return camerasContainer;
            }
        }

        public List<string> GetCamerasKeys
        {
            get
            {
                List<string> camerasKeys = new List<string>();
                foreach (var camera in camerasContainer)
                {
                    camerasKeys.Add(camera.FullName);
                }
                return camerasKeys;
            }
        }

        public List<string> GetCamerasNames
        {
            get
            {
                List<string> camerasNames = new List<string>();
                foreach (var camera in camerasContainer)
                {
                    camerasNames.Add(camera.Name);
                }
                return camerasNames;
            }
        }

        public void SetCameraName(string fullName, string cameraName)
        {
            var camera = camerasContainer.Find(cam => cam.FullName == fullName);
            if(camera != null)
            {
                camera.Name = cameraName;
            }
        }

        public void AddCamera(ICamera camera)
        {
            camerasContainer.Add(camera);
        }

        public void RemoveCamera(ICamera camera)
        {
            camerasContainer.Remove(camera);
        }

        public bool IsEmpty()
        {
            return camerasContainer.Count == 0;
        }

        public void StopAllCameras()
        {
            foreach (ICamera camera in camerasContainer)
            {
                camera.Stop();
            }
        }

        public ICamera GetCameraByKey(string key)
        {
            return camerasContainer.Find(camera => camera.FullName == key);
        }
    }
}
