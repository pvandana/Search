using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Reimbursements.BusinessObjects;
using Reimbursements.DataAccess;
using Reimbursements.Utilities;

//------------------------------------------------------------------------------
//                                Search.ascx.cs
//
//      This class is the code behind for the Reports To Search screen.
// 
//------------------------------------------------------------------------------
// 
//                          Modification Control Log                           
//                                                                             
//    Date     By                 Description                                
//  --------  ---  -------------------------------------------------------------
//  12-16-14  TJM  Initial creation of program                               
// 
//------------------------------------------------------------------------------

namespace Reimbursements.Processor
{
    public partial class Search : System.Web.UI.Page
    {
        #region Member Variables

        // Module Level Variables
        HRSCLogsDA _HRSCLogsDA = null;

        #endregion

　
        #region Enums

        public enum SearchTypeEnum
        {
            EmplId,
            Name,
            TuitionRequestId,
            WWRequestId
        }

        #endregion

　
        /// <summary>
        /// Page Load   
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize the Logging Class
            _HRSCLogsDA = new HRSCLogsDA();

            // Check if 1st time
            if (!IsPostBack)
            {
                //  Log the Action
                _HRSCLogsDA.Insert("Processor Search Screen");

                string searchType = string.Empty;
                string screenName = string.Empty;

                // Get the parameter passed to the screen
                if (Request.QueryString.Count > 0)
                {
                    searchType = Request.QueryString["searchType"];
                    Session["SearchType"] = searchType;
                }

                if (searchType == "NewTuitionRequest")
                {
                    screenName = "New Tuition Request Search";
                    hdnTuitWW.Value = "T";
                }
                else if (searchType == "NewWWRequest")
                {
                    screenName = "New Weight Watchers Request Search";
                    hdnTuitWW.Value = "W";
                }
                else if (searchType == "TuitionReassign")
                {
                    screenName = "Tuition Reassign Search";
                    hdnTuitWW.Value = "T";
                    
                    // Save the Request Id
                    Session["RequestIdU"] = Request.QueryString["requestId"].ToString();
                }
                else if (searchType == "WWReassign")
                {
                    screenName = "Weight Watchers Reassign Search";
                    hdnTuitWW.Value = "W";

                    // Save the Request Id
                    Session["RequestIdU"] = Request.QueryString["requestId"].ToString();
                }
                else if (searchType == "CSRTuitionRequest")
                {
                    screenName = "CSR Tuition Search";
                    hdnTuitWW.Value = "T";
                    ddlSearchBy.Items.Add(new ListItem("Request ID", "2"));
                }
                else if (searchType == "CSRWWRequest")
                {
                    screenName = "CSR Weight Watchers Search";
                    hdnTuitWW.Value = "W";
                    ddlSearchBy.Items.Add(new ListItem("Request ID", "2"));
                }
                else if (searchType == "TuitionCompletedRequests")
                {
                    screenName = "Tuition Completed Requests Search";
                    hdnTuitWW.Value = "T";
                    ddlSearchBy.Items.Add(new ListItem("Request ID", "2"));
                }
                else
                {
                    //screenName = "Tuition Request Search";
                    screenName = "Team Member Profile Search";
                    hdnTuitWW.Value = "T";
                    ddlSearchBy.Items.Add(new ListItem("Tuition Request ID", "2"));
                    ddlSearchBy.Items.Add(new ListItem("Weight Watchers Request ID", "3"));
                    Session.Remove("SearchType");
                }

                // Display Screen Name on the Master Page
                Label lblScreenName = (Label)Master.FindControl("lblScreenName");

                if (lblScreenName != null)
                {
                    lblScreenName.Text = screenName;
                    Title = screenName;
                }

                // Initialize the Search Criteria
                InitializeCriteria();
            }
            else
            {
                // Display the Results
                pnlResults.Visible = true;
            }
        }

　
        /// <summary>
        /// Cancel button Click   
        /// </summary>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //  Log the Action
            _HRSCLogsDA.Insert("Processor Search Screen - Cancel button Clicked");

            // Close the Window and Refresh the page
            ClientScript.RegisterStartupScript(Page.GetType(), "CloseWindow", "CloseWindow();", true);
        }

　
        /// <summary>
        /// Search button Click   
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //  Log the Action
            _HRSCLogsDA.Insert("Processor Search Screen - Search button Clicked");
        }

　
        protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Initialize the Search Criteria
            InitializeCriteria();
        }

　
        /// <summary>
        /// gvResults - OnRowDataBound   
        /// </summary>
        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // Process DataRow
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Check if row is NOT in Edit mode
                    if (e.Row.RowState != DataControlRowState.Edit
                        && e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate))
                    {
                        // Change the Background Color of the row during Hoover
                        GridFormatting.GridViewStyle(e);

                        // Set OnClick event to go into Select Mode
                        e.Row.Attributes.Add("onClick", Page.ClientScript.GetPostBackClientHyperlink(gvResults, "Select$" + e.Row.RowIndex));
                    }
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Error
                string errMsg = string.Format("{0} - gvResults Row Data Bound Error - {1}",
                                              GetType().FullName,
                                              err.Message);

                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Save a User Friendly Error Message in Session Variable
                errMsg = "There was a problem creating a row in the Results grid.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// gvResults - SelectedIndexChanged   
        /// </summary>
        protected void gvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Process Selected Row
                if (gvResults.SelectedValue != null)
                {
                    string tmEmplId = gvResults.SelectedValue.ToString();

                    //  Log the Action
                    string logMsg = string.Format("Processor Search Screen - Row Selected - EmplId: {0}",
                                                  tmEmplId);

                    _HRSCLogsDA.Insert(logMsg);

                    if (Session["SearchType"] != null)
                    {
                        if (Session["SearchType"].ToString() == "NewTuitionRequest")
                        {
                            // Save Employee Id
                            Session["TMEmplIdU"] = tmEmplId;

                            // Get Team Member's Name
                            Session["TMNameU"] = GetName(tmEmplId);

                            // Set Tuition Request Id to New Request
                            Session["RequestIdU"] = "0";

                            // Get the Team Member's Balances
                            TMAvailableBalanceDA tmAvailableBalanceDA = new TMAvailableBalanceDA();
                            DataTable dtTMBal = tmAvailableBalanceDA.GetBal(tmEmplId, System.DateTime.Now);

                            foreach (DataRow dr in dtTMBal.Rows)
                            {
                                if (dr["Reimbursement_Type"].ToString() == "Tuition")
                                {
                                    // Need to save the TM Available Balance for use in the Tuition System Eligibility Checks
                                    Session["TuitionAvailBal"] = dr["AvailAmt"];
                                }
                            }

                            // Display the New Tuition Request screen
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openScreen('/TuitionRequest/NewRequest.aspx');", true);
                        }
                        else if (Session["SearchType"].ToString() == "NewWWRequest")
                        {
                            // Save Employee Id
                            Session["TMEmplIdU"] = tmEmplId;

                            // Get Team Member's Name
                            Session["TMNameU"] = GetName(tmEmplId);

                            // Set Weight Watcher Request Id to New Request
                            Session["RequestIdU"] = "0";

                            // Display the New Weight Watchers Request screen
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openScreen('/WeightWatcherRequest/WWNewRequest.aspx');", true);
                        }
                        else if (Session["SearchType"].ToString() == "TuitionReassign")
                        {
                            // Get the Tuition Request record
                            TuitionRequestsBO trBO = new TuitionRequestsBO();
                            TuitionRequestDA trDA = new TuitionRequestDA();

                            trBO = trDA.GetTuitionRequest(Convert.ToInt32(Session["RequestIdU"]));

                            // Reassign the Processor
                            trBO.Processor = tmEmplId;
                            trBO.Modified_By = Session["EmplId"].ToString();

                            // Update the Tuition Request
                            trDA.UpdateTuitionRequestStatus(trBO);

                            Session.Remove("SearchType");

                            // Display the Processor Tuition Assigned screen
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openScreen('/Processor/TuitionAssigned.aspx');", true);
                        }
                        else if (Session["SearchType"].ToString() == "WWReassign")
                        {
                            // Get the Weight Watchers Request record
                            WWRequestBO wwrBO = new WWRequestBO();
                            WWRequestDA wwrDA = new WWRequestDA();

                            wwrBO = wwrDA.GetWWRequest(Convert.ToInt32(Session["RequestIdU"]));

                            // Reassign the Processor
                            wwrBO.Processor = tmEmplId;
                            wwrBO.Modified_By = Session["EmplId"].ToString();

                            // Update the Weight Watchers Request
                            wwrDA.UpdateWWRequestStatus(wwrBO);

                            Session.Remove("SearchType");

                            // Display the Processor Weight Watchers Assigned screen
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openScreen('/Processor/WWAssigned.aspx');", true);
                        }
                        else if (Session["SearchType"].ToString() == "CSRTuitionRequest")
                        {
                            // Save Employee Id
                            Session["TMEmplId"] = tmEmplId;

                            // Get Team Member's Name
                            Session["TMName"] = GetName(tmEmplId);

                            // Display the New Tuition Request screen
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openScreen('/TuitionRequest/TransactionHistory.aspx');", true);
                        }
                        else if (Session["SearchType"].ToString() == "CSRWWRequest")
                        {
                            // Save Employee Id
                            Session["TMEmplId"] = tmEmplId;

                            // Get Team Member's Name
                            Session["TMName"] = GetName(tmEmplId);

                            // Display the New Tuition Request screen
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openScreen('/WeightWatcherRequest/WWTransactionHistory.aspx');", true);
                        }
                        else if (Session["SearchType"].ToString() == "TuitionCompletedRequests")
                        {
                            // Save Employee Id
                            Session["TCREmplId"] = tmEmplId;

                            // Get Team Member's Name
                            Session["TMName"] = GetName(tmEmplId);

                            // Display the Tuition Completed Requests screen
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openScreen('/QualityReview/TuitionCompletedRequests.aspx');", true);
                        }
                        else
                        {
                            // Save Employee Id
                            Session["TMEmplId"] = tmEmplId;

                            // Get Team Member's Name
                            Session["TMName"] = GetName(tmEmplId);

                            Session.Remove("SearchType");

                            // Display the Team Member Profile screen
                            //((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openPopUp2('/Processor/TMProfile.aspx', 'TMProfile', 760, 825); window.close();", true);
                            ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openPopUp('/Processor/TMProfile.aspx?TMEmplId=" + tmEmplId + "', 'TMProfile', 760, 825); window.close();", true);
                        }
                    }
                    else
                    {
                        // Save Employee Id
                        Session["TMEmplId"] = tmEmplId;

                        // Get Team Member's Name
                        Session["TMName"] = GetName(tmEmplId);

                        Session.Remove("SearchType");

                        // Display the Team Member Profile screen
                        //((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openPopUp2('/Processor/TMProfile.aspx', 'TMProfile', 760, 825); window.close();", true);
                        ((GridView)sender).Page.ClientScript.RegisterStartupScript(((GridView)sender).Page.GetType(), "OpenScreen", "openPopUp('/Processor/TMProfile.aspx?TMEmplId=" + tmEmplId + "', 'TMProfile', 760, 825); window.close();", true);
                    }
                }
            }
            catch (InternalException err)
            {
                // Display a Messagebox
                AlertMessage.Show(err.UserFriendlyMsg, this.Page);
            }
            catch (Exception err)
            {
                // Database Error
                string errMsg = string.Format("{0} - gvResults Selected Index Changed Error - {1}",
                                              GetType().FullName,
                                              err.Message);
                // Log the Error 
                AppLogWrapper.LogError(errMsg);

                // Display a User Friendly Error Message 
                errMsg = "There was a problem selecting a record on the Search screen.  If the problem persists, please contact Technical Support.";

                // Display a Messagebox
                AlertMessage.Show(errMsg, this.Page);
            }
        }

　
        /// <summary>
        /// Retrieves the Team Member's Name   
        /// </summary>
        /// <param name="emplId"></param>
        private string GetName(string emplId)
        {
            string fullName = string.Empty;

            // Check if valid Employee Id
            FullRosterDA fullRosterDA = new FullRosterDA();
            DataTable dtFullRoster = fullRosterDA.Get(emplId);

            // Was Employee Found?
            if (dtFullRoster.Rows.Count > 0)
            {
                // Process 1st row in datatable
                DataRow dr = dtFullRoster.Rows[0];

                // Format the Name to Upper and Lower Case letters
                //fullName = Formatting.FormatName(dr["NAME"].ToString());
                fullName = dr["NAME"].ToString();
            }

            return fullName;
        }

　
        /// <summary>
        /// Initialize Search Criteria   
        /// </summary>
        private void InitializeCriteria()
        {
            // Initialize the Search Criteria
            if (ddlSearchBy.SelectedIndex == 3)
            {
                // Set to Request Id View
                mvSearchBy.ActiveViewIndex = 2;
            }
            else
            {
                // Set to Selected View
                mvSearchBy.ActiveViewIndex = ddlSearchBy.SelectedIndex;
            }

            txtEmplId.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtRequestId.Text = string.Empty;

            // Place the Cursor on the corresponding field
            switch (ddlSearchBy.SelectedIndex)
            {
                case (int)SearchTypeEnum.EmplId:
                    txtEmplId.Focus();
                    break;

                case (int)SearchTypeEnum.Name:
                    txtLastName.Focus();
                    break;

                case (int)SearchTypeEnum.TuitionRequestId:
                    txtRequestId.Focus();
                    break;

                case (int)SearchTypeEnum.WWRequestId:
                    txtRequestId.Focus();
                    hdnTuitWW.Value = "W";
                    break;

                default:
                    break;
            }

            // Hide the Results
            pnlResults.Visible = false;
        }

　
        /// <summary>
        /// sdsResults - Selected   
        /// </summary>
        protected void sdsResults_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check for an exception
            if (e.Exception != null
                && e.Exception.InnerException != null)
            {
                Exception inner = e.Exception.InnerException;
                string errMsg = string.Empty;

                // Check for a previously handled exception
                if (inner is InternalException)
                {
                    // Already Logged
                }
                else
                {
                    // Error
                    errMsg = string.Format("{0} - sdsResults Selected Error - {1}",
                                           GetType().FullName,
                                           inner.Message);

                    // Log the Error 
                    AppLogWrapper.LogError(errMsg);
                }

                // Indicate that the exception has been handled
                e.ExceptionHandled = true;
            }
        }
    }
}
