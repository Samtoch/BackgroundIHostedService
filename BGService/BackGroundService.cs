using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundIHostedService.BGService
{

    public class BackGroundService : HostedService
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private readonly IDataLogic _dataLogic;
        public BackGroundService(IDataLogic dataLogic)
        {
            _dataLogic = dataLogic;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                log.Info("Doing Some Job inside BackGroundService, Happened at: " + DateTime.Now);
                //await _randomStringProvider.UpdateString(cancellationToken);
                await _dataLogic.ConfirmFreeAlertDay(); 
                await _dataLogic.ConfirmPremiumAlertDay();
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
            }
        }
    }
}
