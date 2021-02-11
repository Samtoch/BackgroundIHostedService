using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundIHostedService.BGService
{
    public interface IDataLogic
    {
        Task ConfirmFreeAlertDay(); 
        Task ConfirmPremiumAlertDay();
    }
}
