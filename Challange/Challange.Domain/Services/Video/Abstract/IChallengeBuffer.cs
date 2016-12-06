﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Challange.Domain.Entities;
using Challange.Domain.Abstract;

namespace Challange.Domain.Services.Video.Abstract
{
    public interface IChallengeBuffers
    {
        int MaxElementsInPastCollection { get; set; }

        int MaxElementsInFutureCollection { get; set; }

        Dictionary<string, List<IFps>> PastCameraRecords { get; }

        Dictionary<string, List<IFps>> FutureCameraRecords { get; }

        void SetNumberOfPastAndFutureElements(int maxElementsInPastCollection, int maxElementsInFutureCollection);

        List<IFps> GetPastCameraRecordsValueByKey(string key);

        List<IFps> GetFutureCameraRecordsValueByKey(string key);

        List<IFps> GetFirstPastValue();

        List<IFps> GetFirstFutureValue();

        void ClearBuffers();

        List<Concrete.Video> ConvertToVideoContainer();

        void RemoveFirstFpsFromPastBuffer();

        void AddPastFpses(IFpsContainer fpsContainer);

        void AddFutureFpses(IFpsContainer fpsContainer);

        bool HaveToRemovePastFps();

        bool HaveToAddFutureFps();
    }
}
