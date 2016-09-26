﻿using Challange.Domain.Entities;
using Challange.Domain.Infrastructure;
using Challange.Domain.Services.Challenge;
using Challange.Domain.Services.Settings;
using Challange.Domain.Services.Settings.SettingParser;
using Challange.Domain.Services.Settings.SettingTypes;
using Challange.Domain.Services.StreamProcess.Abstract;
using Challange.Domain.Services.StreamProcess.Concrete;
using Challange.Domain.Services.StreamProcess.Concrete.Pylon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Challange.Presenter.Presenters.MainPresenter
{
    public partial class MainPresenter
    {
        /// <summary>
        /// event which adds and supply concrete number of FPS object
        /// in buffer for past frames
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void InternalTimerEventForPastFrames()
        {
            if (HaveToRemovePastFps())
            {
                RemoveFirstFpsFromPastBuffer();
            }
            AddPastFpses();
            InitializeFpsContainer();
        }

        /// <summary>
        /// check is past frame buffer count equals
        /// to necessary number of past FPS
        /// </summary>
        /// <returns></returns>
        private bool HaveToRemovePastFps()
        {
            var pastFrames = challengeBuffers.GetFirstPastValue();
            return pastFrames.Count == challengeSettings.NumberOfPastFPS;
        }

        /// <summary>
        /// for example our past buffer is only for 20 FPS objects
        /// so on 21 second we have to remove the first object from this buffer
        /// </summary>
        private void RemoveFirstFpsFromPastBuffer()
        {
            challengeBuffers.RemoveFirstFpsFromPastBuffer();
        }

        /// <summary>
        /// adds past fps objects into buffer for past frames
        /// </summary>
        private void AddPastFpses()
        {
            List<Fps> temp;
            foreach (var fps in fpsContainer.Fpses)
            {
                temp = challengeBuffers.
                    GetPastCameraRecordsValueByKey(fps.Key);
                if (temp != null)
                {
                    temp.Add(fps.Value);
                }
                else
                {
                    temp = new List<Fps>();
                    temp.Add(fps.Value);
                    challengeBuffers.AddNewPastCameraRecord(fps.Key, temp);
                }
            }
        }

        /// <summary>
        /// this is temporary object which will keep fps objects
        /// from all cameras which we create every second
        /// </summary>
        private void InitializeFpsContainer()
        {
            fpsContainer = new FpsContainer(camerasContainer.GetCamerasFullNames);
        }

        /// <summary>
        /// Process frames for future collection
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void InternalTimerEventForFutureFrames()
        {
            if (HaveToAddFutureFps())
            {
                AddFutureFpses();
                InitializeFpsContainer();
            }
            else
            {
                ChangeActivityOfEventForFutureFrames(false);
                WriteChallangeAsVideo();
                InitializeFpsContainer();
                challengeBuffers = new ChallengeBuffers(camerasContainer);
                ChangeActivityOfEventForPastFrames(true);
            }
        }

        /// <summary>
        /// check is future frame buffer count equals
        /// to necessary number of future FPS
        /// </summary>
        /// <returns></returns>
        private bool HaveToAddFutureFps()
        {
            var futureFrames = challengeBuffers.GetFirstFutureValue();
            return futureFrames.Count != challengeSettings.NumberOfFutureFPS;
        }

        /// <summary>
        /// adds future fps objects into buffer for future frames
        /// </summary>
        private void AddFutureFpses()
        {
            List<Fps> temp;
            foreach (var fps in fpsContainer.Fpses)
            {
                temp = challengeBuffers.
                    GetFutureCameraRecordsValueByKey(fps.Key);
                if(temp != null)
                {
                    temp.Add(fps.Value);
                }
                else
                {
                    temp = new List<Fps>();
                    temp.Add(fps.Value);
                    challengeBuffers.AddNewFutureCameraRecord(fps.Key, temp);
                }
            }
        }

        /// <summary>
        /// subscribe if isActive true
        /// unsubscribe if isActive false
        /// </summary>
        /// <param name="isActive"></param>
        private void ChangeActivityOfEventForFutureFrames(bool isActive)
        {
            if (isActive)
            {
                internalChallengeTimer.EnableTimerEvent(InternalTimerEventForFutureFrames);
                IsEventForFutureFramesActive = true;
            }
            else
            {
                internalChallengeTimer.DisableTimerEvent();
                IsEventForFutureFramesActive = false;
            }
        }

        /// <summary>
        /// subscribe if isActive true
        /// unsubscribe if isActive false
        /// </summary>
        /// <param name="isActive"></param>
        private void ChangeActivityOfEventForPastFrames(bool isActive)
        {
            if (isActive)
            {
                internalChallengeTimer.EnableTimerEvent(InternalTimerEventForPastFrames);
                IsEventForPastFramesActive = true;
            }
            else
            {
                internalChallengeTimer.DisableTimerEvent();
                IsEventForPastFramesActive = false;
            }
        }

        /// <summary>
        /// Just initialize GameInformation object
        /// </summary>
        private void InitializeGameInformation()
        {
            gameInformation = new GameInformation();
        }

        /// <summary>
        /// Draws player panel
        /// </summary>
        private void DrawPlayers()
        {
            if (playerPanelSettings != null)
            {
                View.DrawPlayers(playerPanelSettings);
            }
        }
        #region settings
        /// <summary>
        /// read player panel settings from xml file
        /// this action occures only when we run our form
        /// </summary>
        /// <returns></returns>
        private PlayerPanelSettings GetPlayerPanelSettings()
        {
            var playerPanelSettingService =
                 new SettingsService<PlayerPanelSettings>(
                                new PlayerPanelSettingsParser());
            return playerPanelSettingService.
                        GetSetting();
        }

        /// <summary>
        /// read challenge settings from xml file
        /// this action occures only when we run our form
        /// </summary>
        /// <returns></returns>
        private ChallengeSettings GetChallengeSettings()
        {           
            var challengeSettingService =
                 new SettingsService<ChallengeSettings>(
                                new ChallengeSettingsParser());
            return challengeSettingService.
                        GetSetting();
        }
        #endregion



        /// <summary>
        /// Initializes devices
        /// </summary>
        private CamerasContainer InitializeDevices()
        {
            InitializeDevicesList();
            var camerasContainer = new CamerasContainer();
            PylonCamera tmpCamera;
            foreach (var cameraInfo in camerasInfo)
            {
                tmpCamera = new PylonCamera(cameraInfo.Index, cameraInfo.FullName);
                camerasContainer.AddCamera(tmpCamera);
            }
            IsDeviceListEmpty = camerasContainer.IsEmpty() ? true : false;
            return camerasContainer;
        }

        /// <summary>
        /// Initializes player for each camera
        /// </summary>
        private void BindPlayersToCameras()
        {
            Queue<string> camerasNames = new Queue<string>();
            foreach (Camera camera in camerasContainer.GetCameras)
            {
                camerasNames.Enqueue(camera.FullName);
            }
            View.BindPlayersToCameras(camerasNames);
        }

        /// <summary>
        /// Starts all devices
        /// </summary>
        private void StartDevices()
        {
            foreach (Camera camera in camerasContainer.GetCameras)
            {
                camera.NewFrameEvent += Camera_NewFrameEvent;
                camera.Start();
            }
        }

        /// <summary>
        /// Process new frame from device cameraName
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="cameraName"></param>
        private void Camera_NewFrameEvent(Bitmap frame, string cameraName)
        {
            View.DrawNewFrame(frame, cameraName);
        }

        /// <summary>
        /// Initializes devices list
        /// </summary>
        private void InitializeDevicesList()
        {
            pylonCameraProvider = new PylonCameraProvider();
            camerasInfo = pylonCameraProvider.GetConnectedCameras();
        }

        /// <summary>
        /// Initialize timer on time axis
        /// </summary>
        private void InitializeTimeAxisTimer()
        {
            View.InitializeTimer();
        }

        /// <summary>
        /// Initialize internal timer to create FPS object every second
        /// </summary>
        private void InitializeInternalTimer()
        {
            InitializeFpsContainer();
            internalChallengeTimer = new InternalChallengeTimer(1000, true);
            internalChallengeTimer.Start();
            internalChallengeTimer.EnableTimerEvent(InternalTimerEventForPastFrames);
        }

        /// <summary>
        /// makes challenge button enable
        /// </summary>
        private void ChangeStateOfChallengeButton(bool isEnable)
        {
            View.ToggleChallengeButton(isEnable);
        }

        /// <summary>
        /// makes start button enable
        /// </summary>
        private void ChangeStateOfStartButton(bool isEnable)
        {
            View.ToggleStartButton(isEnable);
        }

        /// <summary>
        /// makes stop button enable
        /// </summary>
        private void ChangeStateOfStopButton(bool isEnable)
        {
            View.ToggleStopButton(isEnable);
        }

        /// <summary>
        /// Changes the state of challenge button on pointed number of seconds
        /// if true - active
        /// if false - not active
        /// </summary>
        /// <param name="isEnable"></param>
        /// <param name="numberOfSeconds"></param>
        private void ChangeStateOfChallengeButtonIn(bool isEnable, int numberOfSeconds)
        {
            View.ToggleChallengeButtonIn(isEnable, numberOfSeconds);
            IsChallengeButtonEnable = isEnable;
        }      

        /// <summary>
        /// Resets timer on time axis and clear it from markers
        /// </summary>
        private void ResetTimeAxis()
        {
            View.ResetTimeAxis();
            WasTimeAxisResetted = true;
        }

        /// <summary>
        /// set streaming state on "state"
        /// </summary>
        /// <param name="state"></param>
        private void ChangeStreamingStatus(bool state)
        {
            IsStreamProcessOn = state;
            if (IsStreamProcessOn)
            {
                ChangeStateOfChallengeButton(true);
                ChangeStateOfStartButton(false);
                ChangeStateOfStopButton(true);
            }
            else
            {
                ChangeStateOfChallengeButton(false);
                ChangeStateOfStartButton(true);
                ChangeStateOfStopButton(false);
            }
        }

        /// <summary>
        /// Draws marker on time axis
        /// </summary>
        private void AddMarkerOnTimeAxis()
        {
            View.AddMarkerOnTimeAxis();
            MarkerWasAddedOntoTimeAxis = true;
        }

        /// <summary>
        /// Gets elapsed time from start of streaming
        /// </summary>
        /// <returns></returns>
        private string GetChallengeTime()
        {
            var elapsedTime = View.GetElapsedTime;
            ElapsedTimeWasGot = true;
            return elapsedTime;
        }

        /// <summary>
        /// Creates directory in file system for current challenge
        /// </summary>
        /// <param name="name"></param>
        private void CreateDirectoryForChallenge(string name)
        {
            FileService.CreateDirectory(name);
            DirectoryForChallengeWasCreated = true;
        }

        /// <summary>
        /// Writes challenge videos in file system
        /// </summary>
        private void WriteChallangeAsVideo()
        {
            var videos = UnitePastAndFutureFrames();
            var pathToChallenge = challenge.GetChallengeDirectoryPath;
            var challengeWriter = new ChallengeWriter(videos, pathToChallenge);
            challengeWriter.WriteChallenge();
            ClearChallengeBuffers();
        }

        /// <summary>
        /// Unites past and future frames collection in one
        /// </summary>
        /// <returns></returns>
        private List<Video> UnitePastAndFutureFrames()
        {
            var videos = new List<Video>();
            List<Fps> tempVideoFrames;
            string currentVideoName;
            foreach (var pastFrames in challengeBuffers.PastCameraRecords)
            {
                foreach (var futureFrames in challengeBuffers.FutureCameraRecords)
                {
                    if (pastFrames.Key == futureFrames.Key)
                    {
                        tempVideoFrames = new List<Fps>();
                        tempVideoFrames.AddRange(pastFrames.Value);
                        tempVideoFrames.AddRange(futureFrames.Value);
                        View.CamerasNames.TryGetValue(
                                pastFrames.Key, out currentVideoName);
                        videos.Add(new Video(currentVideoName, tempVideoFrames));
                        break;
                    }
                }
            }
            return videos;
        }

        /// <summary>
        /// Clears buffers for past and future frames
        /// </summary>
        private void ClearChallengeBuffers()
        {
            challengeBuffers.ClearBuffers();
        }

        /// <summary>
        /// Stops capturing of devices
        /// </summary>
        public void StopCaptureDevice()
        {
            if (camerasContainer != null)
            {
                foreach (var camera in camerasContainer.GetCameras)
                {
                    camera.Stop();
                }
            }
            IsCaptureDevicesEnable = false;
        }

        #region Show messages
        private void ShowChallengeSettingsFileParseProblemError()
        {
            View.ShowChallengeSettingsFileParseProblemError();
        }

        private void ShowPlayerPanelSettingsFileParseProblemError()
        {
            View.ShowPlayerPanelSettingsFileParseProblemError();
        }

        private void ShowEmptyDeviceContainerMessage()
        {
            View.ShowEmptyDeviceContainerMessage();
            WasDeviceListEmptyMessageShowed = true;
        }
        #endregion
    }
}
