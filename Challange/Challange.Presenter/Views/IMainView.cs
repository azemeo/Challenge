﻿using System;
using Challange.Presenter.Base;
using Challange.Domain.Services.Settings.SettingTypes;
using System.Drawing;
using System.Collections.Generic;

namespace Challange.Presenter.Views
{
    public interface IMainView : IView
    {
        event Action OpenPlayerPanelSettings;

        event Action OpenChallengeSettings;

        event Action OpenDevicesList;

        event Action StartStream;

        event Action StopStream;

        event Action OpenGameFolder;

        event Action MainFormClosing;

        event Action CreateChallange;

        event Action NewFrameCallback;

        event Action<Dictionary<string, string>> PassCamerasNamesToPresenterCallback;

        Tuple<string, Bitmap> CurrentFrameInfo { get; }

        Dictionary<string, string> CamerasNames { get; }

        string GetElapsedTime { get; }

        void DrawPlayers(PlayerPanelSettings settings);

        void InitializeTimer();

        void ResetTimeAxis();

        void AddMarkerOnTimeAxis();

        void ToggleChallengeButton(bool state);

        void ToggleChallengeButtonIn(bool state, int seconds);

        void ToggleStartButton(bool state);

        void ToggleStopButton(bool state);

        void DrawNewFrame(Bitmap frame, string cameraName);

        void BindPlayersToCameras(Queue<string> camerasNames);

        void ShowEmptyDeviceContainerMessage();

        void ShowChallengeSettingsFileParseProblemError();

        void ShowPlayerPanelSettingsFileParseProblemError();
    }
}
