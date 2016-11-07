﻿using Challange.Presenter.Base;
using Challange.Presenter.Views;
using Challange.Domain.Services.Settings.SettingTypes;
using Challange.Domain.Services.Settings;
using Challange.Domain.Services.Settings.SettingParser;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using Challange.Domain.Entities;
using System.Timers;
using Challange.Domain.Infrastructure;
using Challange.Domain.Services.StreamProcess.Concrete.Pylon;
using System.Drawing;
using static PylonC.NETSupportLibrary.DeviceEnumerator;
using Challange.Domain.Services.StreamProcess.Concrete;
using Challange.Domain.Services.StreamProcess.Abstract;
using System.Linq;
using Challange.Domain.Services.Replay;
using Challange.Domain.Services.Message;

namespace Challange.Presenter.Presenters.MainPresenter
{
    public partial class MainPresenter : BasePresenter<IMainView>
    {
        // settings
        private PlayerPanelSettings playerPanelSettings;
        private ChallengeSettings challengeSettings;
        private FtpSettings ftpSettings;
        //
        private GameInformation gameInformation;
        // video streaming
        private CamerasContainer camerasContainer;
        private PylonCameraProvider pylonCameraProvider;

        // challenge
        private ChallengeBuffers challengeBuffers;
        // this is temporary object which will keep fps objects
        // from all cameras which we create every second
        private FpsContainer fpsContainer;
        private InternalChallengeTimer internalChallengeTimer;
        private ChallengeObject challenge;

        private MessageParser messageParser;

        private FileService fileService;
        private ProcessStarter processStarter;

        private ZoomCalculator zoomCalculator;
        private Zoomer zoomer;

        public MainPresenter(IApplicationController controller,
                             IMainView mainView) : 
                             base(controller, mainView)
        {
            SubscribePresenters();
            zoomCalculator = new ZoomCalculator();
            zoomer = new Zoomer(zoomCalculator);
            messageParser = new MessageParser();
            processStarter = new ProcessStarter();
            fileService = new FileService(processStarter);
        }

        private void SubscribePresenters()
        {
            View.OpenPlayerPanelSettings +=
                        ChangePlayerPanelSettings;
            View.OpenChallengeSettings +=
                                    ChangeChallengeSettings;
            View.OpenFtpSettings +=
                        ChangeFtpSettings;
            View.OpenDevicesList += ShowDevicesList;
            View.StartStream += StartStream;
            View.StopStream += StopStream;
            View.OpenGameFolder += OpenGameFolder;
            View.MainFormClosing += FormClosing;
            View.CreateChallange += CreateChallange;
            View.NewFrameCallback += AddNewFrame;
            View.OpenChallengePlayer += OpenChallengePlayer;
            View.MakeZoom += MakeZoom;
            View.PassCamerasNamesToPresenterCallback += PassCamerasNamesToPresenter;
            View.OpenChallengePlayerForLastChallenge += OpenChallengePlayerForLastChallenge;
            View.OpenBroadcastForm += OpenBroadcastForm;
        }
        
        public GameInformation GameInformation
        {
            set
            {
                gameInformation = value;
            }
        }   

        public InternalChallengeTimer InternalChallengeTimer
        {
            set
            {
                internalChallengeTimer = value;
            }
        }

        public ChallengeSettings ChallengeSettings
        {
            set
            {
                challengeSettings = value;
            }
        }

        public FtpSettings FtpSettings
        {
            set
            {
                ftpSettings = value;
            }
        }
    }
}
