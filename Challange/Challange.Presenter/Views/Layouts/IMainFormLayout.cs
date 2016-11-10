﻿using Challange.Domain.Services.Settings.SettingTypes;
using Challange.Presenter.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Challange.Presenter.Views.Layouts
{
    public interface IMainFormLayout : ILayout
    {
        void BindForm(Form form);

        void BindMainFormPlayerPanel(FlowLayoutPanel mainFormPlayerPanel);

        void DrawPlayers(PlayerPanelSettings settings, int numberOfPlayers);
    }
}
