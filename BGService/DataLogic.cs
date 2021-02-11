using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundIHostedService.BGService
{
    public class DataLogic : IDataLogic
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private MySql.Data.MySqlClient.MySqlConnection conn;
        public DataLogic(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionString").Value;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = _connectionString;
        }

        public Task ConfirmFreeAlertDay()
        {
            DateTime dateTime = DateTime.Now; //.
            string dateTimeStr = dateTime.ToString("HH:mm");

            string latestTime = GetLatestAlertDay("FREE");
            if (dateTimeStr == latestTime)
            {
                log.Info("There was a match in ConfirmFreeAlertDay at: " + DateTime.Now);
            }
            else
            {
                log.Info("There was mismatch in ConfirmFreeAlertDay at " + DateTime.Now + ", Nigerian Time: " + DateTime.Now.AddHours(6));
            }

            return Task.CompletedTask;
        }

        public Task ConfirmPremiumAlertDay()
        {
            DateTime dateTime = DateTime.Now; //.
            string dateTimeStr = dateTime.ToString("HH:mm");

            string latestTime = GetLatestAlertDay("PREMIUM");
            if (dateTimeStr == latestTime)
            {
                log.Info("There was a match in ConfirmPremiumAlertDay at: " + DateTime.Now);
            }
            else
            {
                log.Info("There was mismatch in ConfirmPremiumAlertDay at " + DateTime.Now + ", Nigerian Time: " + DateTime.Now.AddHours(6));
            }

            return Task.CompletedTask;
        }

        public string GetLatestAlertDay(string alertType)
        {
            string ALERT_DAY;
            if (alertType == "PREMIUM")
            {
                ALERT_DAY = "MAIN_ALERT_DAY = DATE(SYSDATE())";
            }
            else
            {
                ALERT_DAY = "FREE_ALERT_DAY = DATE(SYSDATE())";
            }
            string timeStr = null;
            MySql.Data.MySqlClient.MySqlDataReader mysqlReader = null;

            string sqlSelect = "SELECT TIME FROM ALERT_CALENDER WHERE GAME_YEAR = YEAR(SYSDATE()) AND " + ALERT_DAY + " AND RUNFLG = 'Y' ORDER BY TIME ASC LIMIT 1";
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlSelect, conn);

            try
            {
                conn.Open();
                mysqlReader = cmd.ExecuteReader();
                if (mysqlReader.Read())
                {
                    timeStr = (mysqlReader.IsDBNull(mysqlReader.GetOrdinal("TIME")) ? "" : mysqlReader.GetString(mysqlReader.GetOrdinal("TIME")));

                    mysqlReader.Close();
                    conn.Close();
                    return timeStr;
                }
                mysqlReader.Close();
                conn.Close();
                return timeStr;
            }
            catch (Exception ex)
            {
                log.Error("Unable to GetLatestAlertDay at: " + DateTime.Now + " MSG: " + ex);
            }
            return timeStr;
        }

    }


}
