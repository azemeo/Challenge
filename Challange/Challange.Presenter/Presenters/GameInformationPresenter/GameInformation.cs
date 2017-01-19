﻿using Challange.Domain.Entities;
using Challange.Domain.Services.FileSystem.Abstract;
using Challange.Presenter.Base;
using Challange.Presenter.Views;

namespace Challange.Presenter.Presenters.GameInformationPresenter
{
    // TODO Remake FormatDirectoryName (use name formatter to make it testable)
    // TODO Make FtpConnector as service
    // TODO Make ZoomData part of service
    // TODO Make GameInformation as service
    public partial class GameInformationPresenter :
                    BasePresenter<IGameInformationView, GameInformation>
    {
        private GameInformation gameInformation;

        private IFileService fileService;

        private IXmlWorker fileWorker;

        private IPathFormatter pathFormatter;

        public GameInformationPresenter(
        IApplicationController controller,
        IGameInformationView gameInformationView,
        IFileService fileService,
        IXmlWorker fileWorker,
        IPathFormatter pathFormatter
        ) :
                base(controller, gameInformationView)
        {
            this.fileService = fileService;
            this.fileWorker = fileWorker;
            this.pathFormatter = pathFormatter;
            SubscribePresenters();
        }

        private void SubscribePresenters()
        {
            View.SetGameInformation += PrepareApplication;
        }
    }
}
