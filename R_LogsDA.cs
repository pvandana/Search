using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Reimbursements.Utilities;

//------------------------------------------------------------------------------
//                                    HRSCLogsDA.cs
//
//      This class handles the HRSC NET Logs Data Access
// 
//------------------------------------------------------------------------------
// 
//                          Modification Control Log                           
//                                                                             
//    Date     By                 Description                                
//  --------  ---  -------------------------------------------------------------
//  01-14-14  JV   Initial creation of program                               
//  07-02-15  TJM  Modified to use SQL Server                               
// 
//------------------------------------------------------------------------------

namespace Reimbursements.DataAccess
{
    public class HRSCLogsDA
    {
        #region Member Variables

        // Variables
        private SqlDatabase _DB = null;

        #endregion

　
        #region Constructors

        public HRSCLogsDA()
        {
            // Initialize Variables
            ErrMsg = string.Empty;

            try
            {
                // Create an instance of the SQL Database class
                _DB = new SqlDatabase(ConfigurationManager.ConnectionStrings["REIMB"].ToString());
            }
            catch (Exception err)
            {
                // Database Error
                ErrMsg = string.Format("{0} - Connecting to Database Error - {1}",
                                       GetType().FullName,
                                       err.Message);

                // Save the Error
                CommonDA.SaveError(ErrMsg, err);
            }
        }

        #endregion

　
        #region Enums

        #endregion

　
        #region Properties

        public string ErrMsg { get; set; }

        #endregion

　
        #region Methods

        /// <summary>
        /// Inserts a record in the HRSC NET Logs Table
        /// </summary>
        /// <param name="action">Action</param>
        public void Insert(string action)
        {
            // Define Constants
            const string sp = "HRSCLogs.dbo.Insert_Activity_Log";

            // Set Stored Procedure
            DbCommand dbCommand = _DB.GetStoredProcCommand(sp);

            // Get the Application Id
            string appName = string.Empty;

            try
            {
                try
                {
                    appName = ConfigurationManager.AppSettings["Application_Name"];

                    if (String.IsNullOrEmpty(appName))
                    {
                        appName = "REIMB Web";
                    }
                }
                catch (Exception)
                {
                    appName = "REIMB Web";
                }

                // Get the User's IP Address - Returns 127.0.0.1
                //HttpRequest currentRequest = HttpContext.Current.Request;
                //string ipAddress = currentRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

                //if (ipAddress == null 
                //    || ipAddress.ToLower() == "unknown")
                //{
                //    ipAddress = currentRequest.ServerVariables["REMOTE_ADDR"];
                //}

                //string ipAddress = HttpContext.Current.Request.UserHostAddress;

                // Get the User's IP Address - Actual workstation ip address when coming through VPM (MyEd)
                //string ipAddress = Dns.GetHostAddresses(strHostName).GetValue(1).ToString();
                //string ipAddress = Dns.GetHostAddresses(strHostName).GetValue(0).ToString();

                // Get the User's IP Address
                string strHostName = Dns.GetHostName();
                //string ipAddress = Dns.GetHostAddresses(strHostName).GetValue(Dns.GetHostAddresses(strHostName).Count() - 1).ToString();
                //string ipAddress = Dns.GetHostAddresses(strHostName).GetValue(0).ToString();
                //string ipAddress = HttpContext.Current.Session["IPAddress"].ToString();
                string ipAddress = string.Empty;

                // Using Proxy?
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                    {
                        // Return real client IP.
                        ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                    }
                    // Another option for Proxies
                    else if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
                    {
                        // Return real client IP.
                        ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"].ToString();
                    }
                    // not using proxy or can't get the Client IP
                    else
                    {
                        // While it can't get the Client IP, it will return proxy IP.
                        ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    }
                }

                // Set Input Parameters
                _DB.AddInParameter(dbCommand, "AppId", DbType.String, appName);
                _DB.AddInParameter(dbCommand, "Action", DbType.String, action);
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session["EmplId"] != null)
                    {
                        _DB.AddInParameter(dbCommand, "ActionBy", DbType.String, HttpContext.Current.Session["EmplId"].ToString().Trim());
                    }
                }
                _DB.AddInParameter(dbCommand, "IPAddress", DbType.String, ipAddress);
                _DB.AddInParameter(dbCommand, "ServerName", DbType.String, Environment.MachineName.ToString());
                _DB.AddOutParameter(dbCommand, "RowCount", DbType.Int32, 0);
                _DB.ExecuteNonQuery(dbCommand);

                int rowCount = Convert.ToInt32(_DB.GetParameterValue(dbCommand, "@RowCount"));

                if (rowCount == null
                    || rowCount.ToString() == "0")
                {
                    // Format the Parameters
                    string parms = CommonDA.FormatParmsSQL(dbCommand);

                    // Error
                    ErrMsg = string.Format("{0} - Insert Error - Parameters = {1} - No rows were Inserted.",
                                           GetType().FullName,
                                           parms);

                    // Save the Error
                    Exception err = new Exception();
                    CommonDA.SaveError(ErrMsg, err);
                }
            }
            catch (InternalException err)
            {
                // Re-Throw the Internal Exception
                throw;
            }
            catch (Exception err)
            {
                // Format the Parameters
                string parms = CommonDA.FormatParmsSQL(dbCommand);

                // Error
                ErrMsg = string.Format("{0} - Insert Error - Parameters = {1} - {2}",
                                       GetType().FullName,
                                       parms,
                                       err.Message);

                // Save the Error
                CommonDA.SaveError(ErrMsg, err);
            }
        }

        #endregion
    }
}
