﻿using Challange.Domain.Services.Message;
using Challange.Domain.Services.StreamProcess.Concrete.Pylon;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Challange.Presenter.Presenters.MainPresenter
{
    public partial class MainPresenter
    {
        /// <summary>
        /// event which adds and supply concrete number of FPS object
        /// in buffer for past frames
        /// </summary>
        public void InternalTimerEventForPastFrames()
        {
            if (_challengeBuffers.HaveToRemovePastFps(_settingsContext.ChallengeSetting.NumberOfPastFPS))
            {
                _challengeBuffers.RemoveFirstFpsFromPastBuffer();
            }
            _challengeBuffers.AddPastFpses(_fpsContainer);
            InitializeFpsContainer();
        }

        /// <summary>
        /// Process frames for future collection
        /// </summary>
        public void InternalTimerEventForFutureFrames()
        {
            if (_challengeBuffers.HaveToAddFutureFps(_settingsContext.ChallengeSetting.NumberOfFutureFPS))
            {
                _challengeBuffers.AddFutureFpses(_fpsContainer);
            }
            else
            {
                ChangeActivityOfEventForFutureFrames(false);
                WriteChallangeAsVideo();
                _challengeBuffers.ClearBuffers();
                ChangeActivityOfEventForPastFrames(true);
            }
            InitializeFpsContainer();
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
                EnableTimerEvent(InternalTimerEventForFutureFrames);
            }
            else
            {
                DisableTimerEvent();
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
                EnableTimerEvent(InternalTimerEventForPastFrames);
            }
            else
            {
                DisableTimerEvent();
            }
        }

        /// <summary>
        /// Draws player panel
        /// </summary>
        private void DrawPlayers()
        {
            var camerasNames = _camerasProvider.CamerasContainer.GetCamerasKeys();
            View.DrawPlayers(_settingsContext.PlayerPanelSetting,
                _camerasProvider.CamerasContainer.CamerasNumber, camerasNames);
        }
        #region settings
        /// <summary>
        /// Read all settings via setting context from xml files
        /// </summary>
        private void ReadAllSettings()
        {
            _settingsContext.GetPlayerPanelSetting();
            _settingsContext.GetChallengeSetting();
            _settingsContext.GetFtpSetting();
            _settingsContext.GetRewindSetting();
        }

        /// <summary>
        /// Checks all setting on null and if setting is null it adds into 
        /// null container. NullSettingTypes is list with types of all null settings
        /// </summary>
        private void CheckAllSettingsOnNull()
        {
            _nullSettingsContainer.CheckPlayerPanelSettingOnNull(_settingsContext.PlayerPanelSetting);
            _nullSettingsContainer.CheckChallengeSettingOnNull(_settingsContext.ChallengeSetting);
            _nullSettingsContainer.CheckFtpSettingOnNull(_settingsContext.FtpSetting);
            _nullSettingsContainer.CheckRewindSettingOnNull(_settingsContext.RewindSetting);
        }
        #endregion

        /// <summary>
        /// Initializes devices
        /// </summary>
        private void InitializeDevices()
        {
            _camerasProvider.InitializeCameras();
        }

        /// <summary>
        /// Starts all devices
        /// </summary>
        private void StartDevices()
        {
            _camerasProvider.StartAllCameras(Camera_NewFrameEvent);
        }

        [ExcludeFromCodeCoverage]
        /// <summary>
        /// Process new frame from device cameraFullName
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="cameraName"></param>
        private void Camera_NewFrameEvent(object sender, EventArgs args)
        {
            NewFrameEventArgs newFrameEventArgs = args as NewFrameEventArgs;
            View.DrawNewFrame(newFrameEventArgs.Frame, newFrameEventArgs.CameraName);
        }

        /// <summary>
        /// Initialize timer on time axis
        /// </summary>
        private void InitializeTimeAxisTimer()
        {
            View.InitializeTimer();
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
        /// makes challenge button invisible
        /// and makes challenge recording image visible
        /// </summary>
        /// <param name="numberOfSeconds"></param>
        private void SetUIAsChallengeRecordingOn(int numberOfSeconds)
        {
            MakeChallengeButtonInvisibleOn(numberOfSeconds);
            MakeChallengeRecordingImageVisibleOn(numberOfSeconds);
        }

        /// <summary>
        /// Makes visible challenge recording image on pointed number of seconds
        /// </summary>
        /// <param name="numberOfSeconds"></param>
        private void MakeChallengeRecordingImageVisibleOn(int numberOfSeconds)
        {
            View.MakeChallengeRecordingImageVisibleOn(numberOfSeconds);
        }

        /// <summary>
        /// Makes invisible add challenge button on pointed number of seconds
        /// </summary>
        /// <param name="isVisible"></param>
        /// <param name="numberOfSeconds"></param>
        private void MakeChallengeButtonInvisibleOn(int numberOfSeconds)
        {
            View.MakeChallengeButtonInvisibleOn(numberOfSeconds);
        }      

        /// <summary>
        /// Resets timer on time axis and clear it from markers
        /// </summary>
        private void ResetTimeAxis()
        {
            View.ResetTimeAxis();
        }

        /// <summary>
        /// set streaming state on "state"
        /// </summary>
        /// <param name="state"></param>
        private void ChangeStreamingStatus(bool state)
        {
            if (state)
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
            View.ToggleVisibilityOfViewLastChallengeButton();
        }

        /// <summary>
        /// Draws marker on time axis
        /// </summary>
        private void AddMarkerOnTimeAxis(string pathToChallenge)
        {
            View.AddMarkerOnTimeAxis(pathToChallenge);
        }

        /// <summary>
        /// Gets elapsed time from start of streaming
        /// </summary>
        /// <returns></returns>
        private string GetChallengeTime()
        {
            return View.GetElapsedTime;
        }

        /// <summary>
        /// Writes challenge videos in file system
        /// </summary>
        private void WriteChallangeAsVideo()
        {
            var unitedBuffers = _challengeBuffers.UniteBuffers(_camerasProvider.CamerasContainer);
            _challengeWriter.WriteChallenge(_videoContainer.ConvertToVideoContainer(unitedBuffers),
                                           _challenge.PathToChallengeDirectory);
        }

        private void SetChallengeDirectoryPath() 
        {
            var pathToRootDirectory = _challenge.PathToRootDirectory;
            var challengeFolderName = _challenge.ChallengeFolderName;
            _challenge.PathToChallengeDirectory = 
                _pathFormatter.FormatPath(pathToRootDirectory, challengeFolderName);
        }

        /// <summary>
        /// Stops capturing of devices
        /// </summary>
        public void StopCaptureDevice()
        {
            _camerasProvider.StopAllCameras();
        }

        private void CreateDirectoryForChallenge()
        {
            _fileService.CreateDirectory(_challenge.PathToChallengeDirectory);
        }

        private void ShowMessage(MessageType type)
        {
            ChallengeMessage message = _messageParser.GetMessage(type);
            View.ShowMessage(message);
        }

        private void InitializeFpsContainer()
        {
            _fpsContainer.InitializeFpses(_camerasProvider.CamerasContainer.GetCamerasNames());
        }

        private void EnableTimerEvent(Action action)
        {
            _internalChallengeTimer.EnableTimerEvent(action);
        }

        private void DisableTimerEvent()
        {
            _internalChallengeTimer.DisableTimerEvent();
        }
    }
}
