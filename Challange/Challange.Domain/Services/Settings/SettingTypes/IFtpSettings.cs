﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challange.Domain.Services.Settings.SettingTypes
{
    public interface IFtpSettings
    {
        string FtpAddress { get; set; }

        string UserName { get; set; }

        string Password { get; set; }
    }
}
