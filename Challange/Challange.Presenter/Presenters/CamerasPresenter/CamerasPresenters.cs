﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PylonC.NETSupportLibrary.DeviceEnumerator;

namespace Challange.Presenter.Presenters.CamerasPresenter
{
    public partial class CamerasPresenter
    {
        public override void Run(List<string> argument)
        {
            connectedCameras = argument;
            FillCamerasListView();
            View.Show();

            CamerasListWindowIsOpened = true;
        }
    }
}
