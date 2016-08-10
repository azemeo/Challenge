﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Challange.Presenter.Views;
using Challange.Domain.Services.Settings.SettingTypes;
using AForge.Video.DirectShow;
using AForge.Video;

namespace Challange.Forms
{
    public partial class MainForm : Form, IMainView
    {
        private readonly ApplicationContext context;
        private int numberOfPlayers;
        private const int autosizeWidthCoefficient = 5;
        private const int autosizeHeightCoefficient = 3;
        private Timer timer;
        private Timer timeToRemoveOldFrames;
        private List<PictureBox> allPlayers;

        private List<Bitmap> pastFrames;
        private List<Bitmap> futureFrames;

        public MainForm(ApplicationContext context)
        {
            this.context = context;
            InitializeComponent();
            playerPanelSettings.Click += (sender, args) =>
                            Invoke(OpenPlayerPanelSettings);
            gameSettings.Click += (sender, args) =>
                            Invoke(OpenGameSettings);
            startStreamButton.Click += (sender, args) =>
                            Invoke(StartStream);
            stopStreamButton.Click += (sender, args) =>
                            Invoke(StopStream);
            FormClosing += (sender, args) =>
                            Invoke(MainFormClosing);
            addChallange.Click += (sender, args) =>
                            Invoke(CreateChallange);
            allPlayers = new List<PictureBox>();
            pastFrames = new List<Bitmap>();
        }

        #region events

        private void Timer_Tick(object sender, EventArgs e)
        {
            challangeTimeAxis.UpdateTimeAxis();
            elapsedTimeFromStart.Text = challangeTimeAxis.ElapsedTimeFromStart;
            temp.Text = pastFrames.Count.ToString();
            if(elapsedTimeFromStart.Text == "00:00:10")
            {
                timeToRemoveOldFrames = new Timer();
                timeToRemoveOldFrames.Tick += new EventHandler(timeToRemoveOldFrames_Tick);
                timeToRemoveOldFrames.Interval = 1000; // in miliseconds
                timeToRemoveOldFrames.Start();
            }
        }

        private void timeToRemoveOldFrames_Tick(object sender, EventArgs e)
        {
            pastFrames.RemoveRange(0, 8);
        }

        private void PlayerPanel_Click(object sender, EventArgs e)
        {
            var clickedPlayer = sender as PictureBox;
            if (!IsReplaceMode())
            {
                firstSelectedPlayer = clickedPlayer;
                SetCursor(Cursors.NoMove2D);
                SetBorderStyle(BorderStyle.Fixed3D);
            }
            else
            {
                secondSelecterPlayer = clickedPlayer;
                ReplacePlayers();
                SetCursor(Cursors.Default);
                SetBorderStyle(BorderStyle.None);
            }
            ToggleReplaceMode();
        }

        private void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap video = (Bitmap)eventArgs.Frame.Clone();
            pastFrames.Add(video);
            foreach (var player in allPlayers)
            {
                player.Image = video;
            }
        }

        public event Action OpenPlayerPanelSettings;

        public event Action OpenGameSettings;

        public event Action StartStream;

        public event Action StopStream;

        public event Action MainFormClosing;

        public event Action CreateChallange;
        #endregion

        #region replace players fields
        bool replaceMode = false;
        PictureBox firstSelectedPlayer, secondSelecterPlayer;
        private void ReplacePlayers()
        {
            var firstPlayerIndex = GetPlayerIndex(firstSelectedPlayer);
            var secondPlayerIndex = GetPlayerIndex(secondSelecterPlayer);
            playerPanel.Controls.SetChildIndex(firstSelectedPlayer,
                                            secondPlayerIndex);
            playerPanel.Controls.SetChildIndex(secondSelecterPlayer,
                                            firstPlayerIndex);
        }

        private int GetPlayerIndex(PictureBox player)
        {
            return playerPanel.Controls.GetChildIndex(player);
        }

        private void SetCursor(Cursor cursorType)
        {
            Cursor = cursorType;
        }

        private void SetBorderStyle(BorderStyle borderStyle)
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                allPlayers.ElementAt(i).BorderStyle = borderStyle;
            }
        }

        private bool IsReplaceMode()
        {
            if (replaceMode)
            {
                return true;
            }
            return false;
        }

        private void ToggleReplaceMode()
        {
            replaceMode = !replaceMode;
        }
        #endregion


        public void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Start();
            timer.Tick += new EventHandler(Timer_Tick);
        }     

        public void ResetTimeAxis()
        {
            timer.Stop();
            timer.Dispose();
            challangeTimeAxis.Reset();
            elapsedTimeFromStart.ResetText();
        }

        public void SubscribeNewFrameEvent(VideoCaptureDevice finalVideo)
        {
            finalVideo.NewFrame += FinalVideo_NewFrame;
        }

        public new void Show()
        {
            context.MainForm = this;
            Application.Run(context);
        }

        #region DrawPlayers
        public void DrawPlayers(PlayerPanelSettings settings)
        {
            playerPanel.Controls.Clear();
            numberOfPlayers = settings.NumberOfPlayers;
            var playerSize = GetPlayerSize(settings);
            var playerWidth = playerSize.Width;
            var playerHeight = playerSize.Height;
            for (int i = 0; i < numberOfPlayers; i++)
            {
                var player = InitializePlayer(playerWidth,
                                    playerHeight, i.ToString());
                DrawPlayer(player);
                AddPlayerIntoPlayerList(player);
            }        
        }

        private Size GetPlayerSize(PlayerPanelSettings settings)
        {
            Size size = new Size();
            if (settings.AutosizeMode)
            {
                size.Width = playerPanel.Width / autosizeWidthCoefficient;
                size.Height = playerPanel.Height / autosizeHeightCoefficient;
            }
            else
            {
                size.Width = settings.PlayerWidth;
                size.Height = settings.PlayerHeight;
            }
            return size;
        }

        private void AddPlayerIntoPlayerList(PictureBox player)
        {
            allPlayers.Add(player);
        }

        private void DrawPlayer(PictureBox player)
        {
            playerPanel.Controls.Add(player);
        }

        private PictureBox InitializePlayer(int playerWidth, int playerHeight,
                                string playerName)
        {
            PictureBox newPictureBox;
            newPictureBox = new PictureBox();
            newPictureBox.BackColor = Color.Red;
            newPictureBox.Height = playerHeight;
            newPictureBox.Width = playerWidth;
            newPictureBox.Controls.Add(new Label() { Text = playerName });
            newPictureBox.Click += new EventHandler(PlayerPanel_Click);
            newPictureBox.Tag = "playerPanel";
            return newPictureBox;
        }
        #endregion

        public void ClearPlayers()
        {
            foreach (var player in allPlayers)
            {
                player.Image = null;
            }
        }

        public void AddMarketOnTimeAxis()
        {
            challangeTimeAxis.CreateMarker();
        }
    }
}
