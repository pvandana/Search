<%@ Page Title="Processor Search" 
         Language="C#" 
         MasterPageFile="~/MasterPages/PopUp.Master" 
         AutoEventWireup="true" 
         CodeBehind="Search.aspx.cs" 
         Inherits="Reimbursements.Processor.Search" %>

<asp:Content ID="Content1" 
             ContentPlaceHolderID="HeadContent" 
             runat="server">
    <link href="/Styles/Grid.css" 
          rel="stylesheet" 
          type="text/css" />
    <script language="JavaScript"
            type="text/javascript" 
            src="../Scripts/GridView.js">
    </script>

    <script type="text/javascript">

        function ValidateNameId(oSrc, args) 
        {
            var txtLastName = document.getElementById("<%= txtLastName.ClientID %>").value
            var txtEmplId = document.getElementById("<%= txtEmplId.ClientID %>").value

            if ((txtLastName.length == 0)
                && (txtEmplId.length == 0)) 
            {
                args.IsValid = false;
                return;
            }

            return true;
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" 
             ContentPlaceHolderID="MainContent" 
             runat="server">

    <asp:HiddenField ID="hdnTuitWW" 
                     runat="server" 
                     ClientIDMode="Static" 
                     Value="T" />

    <asp:Panel ID="pnlCriteria" 
               runat="server" 
               CssClass="Panel" 
               GroupingText="Search Criteria">
        <table cellpadding="10" 
               cellspacing="1" 
               width="99%">
            <tr>
                <td align="right" 
                    nowrap="nowrap" 
                    width="45%">
                    Search By:
                </td>
                <td width="55%">
                    <asp:DropDownList ID="ddlSearchBy" 
                                      runat="server" 
                                      AutoPostBack="True" 
                                      OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                        <asp:ListItem Selected="True" Text="Employee ID" Value="0" />
                        <asp:ListItem Text="Name" Value="1" />
                    </asp:DropDownList>
<%-- 
                         <asp:ListItem Text="Request ID" Value="2" />
--%>
                </td>
            </tr>

            <tr>
                <td align="center" 
                    colspan="2" 
                    nowrap="nowrap">
                    <asp:MultiView ID="mvSearchBy" 
                                   runat="server" 
                                   ActiveViewIndex="0">
                        <asp:View ID="vEmplId" 
                                  runat="server">
                            Employee ID:
                            &nbsp;&nbsp;
                            <asp:TextBox ID="txtEmplId" 
                                         runat="server" 
                                         ClientIDMode="Static" 
                                         MaxLength="11" />
                            <asp:RequiredFieldValidator ID="rfvEmplId" 
                                                        runat="server" 
                                                        ControlToValidate="txtEmplId" 
                                                        Display="Dynamic" 
                                                        ErrorMessage="Employee Id" 
                                                        ForeColor="Red" 
                                                        SetFocusOnError="True" 
                                                        Text="*" />
                            <asp:RegularExpressionValidator ID="revEmplId" 
                                                            runat="server" 
                                                            ControlToValidate="txtEmplId" 
                                                            Display="Dynamic" 
                                                            ErrorMessage="Employee ID must be numeric"
                                                            ForeColor="Red" 
                                                            SetFocusOnError="True"
                                                            Text="*"
                                                            ValidationExpression="^\d+$" />
                        </asp:View>

                        <asp:View ID="vName" 
                                  runat="server">
                            Last Name:
                            &nbsp;&nbsp;
                            <asp:TextBox ID="txtLastName" 
                                         runat="server"
                                         width="200px" />
                            <asp:RequiredFieldValidator ID="rfvLastName" 
                                                        runat="server" 
                                                        ControlToValidate="txtLastName" 
                                                        Display="Dynamic" 
                                                        ErrorMessage="Last Name" 
                                                        ForeColor="Red" 
                                                        SetFocusOnError="True" 
                                                        Text="*" />
                            <asp:RegularExpressionValidator ID="revLastName" 
                                                            runat="server" 
                                                            ControlToValidate="txtLastName" 
                                                            Display="Dynamic" 
                                                            ErrorMessage="You must enter at least the first two letters in the last name."
                                                            ForeColor="Red" 
                                                            SetFocusOnError="True"
                                                            Text="*"
                                                            ValidationExpression=".{2}.*"  />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            First Name:
                            &nbsp;&nbsp;
                            <asp:TextBox ID="txtFirstName" 
                                            runat="server"
                                            width="200px" />
                        </asp:View>

                        <asp:View ID="vRequestId" 
                                  runat="server">
                            Request ID:
                            &nbsp;&nbsp;
                            <asp:TextBox ID="txtRequestId" 
                                         runat="server" />
                            <asp:RequiredFieldValidator ID="rfvRequestId" 
                                                        runat="server" 
                                                        ControlToValidate="txtRequestId" 
                                                        Display="Dynamic" 
                                                        ErrorMessage="Request Id" 
                                                        ForeColor="Red" 
                                                        SetFocusOnError="True" 
                                                        Text="*" />
                            <asp:RegularExpressionValidator ID="revRequestId" 
                                                            runat="server" 
                                                            ControlToValidate="txtRequestId" 
                                                            Display="Dynamic" 
                                                            ErrorMessage="Request ID must be numeric"
                                                            ForeColor="Red" 
                                                            SetFocusOnError="True"
                                                            Text="*"
                                                            ValidationExpression="^\d+$" />
                        </asp:View>
                    </asp:MultiView>
                    <br />
                </td>
            </tr>

            <tr>
                <td align="center"
                    colspan="2" 
                    nowrap="nowrap">
                    <asp:Button ID="btnSearch" 
                                runat="server" 
                                onclick="btnSearch_Click" 
                                Text="    Search    " />
                    <asp:ValidationSummary ID="ValidationSummary1" 
                                           runat="server" 
                                           ShowMessageBox="true" 
                                           ShowSummary="false" 
                                           HeaderText="Please enter one of the following:" />
                    &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                    <asp:Button ID="btnCancel" 
                                runat="server" 
                                CausesValidation="false" 
                                onclick="btnCancel_Click" 
                                Text="    Cancel    " />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />

    <asp:Panel ID="pnlResults" 
               runat="server" 
               CssClass="Panel" 
               GroupingText="Search Results">
        <asp:SqlDataSource ID="sdsResults" 
                           runat="server"
                           CancelSelectOnNullParameter="False"
                           ConnectionString="<%$ ConnectionStrings:REIMB %>" 
                           OnSelected="sdsResults_Selected"
                           SelectCommand="Get_Search_Results"
                           SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:ControlParameter ControlID="txtEmplId"  
                                      DbType="String"
                                      Name="EmplId" 
                                      PropertyName="Text" />
                <asp:ControlParameter ControlID="txtLastName" 
                                      DbType="String"
                                      Name="LastName" 
                                      PropertyName="Text" />
                <asp:ControlParameter ControlID="txtFirstName" 
                                      DbType="String"
                                      Name="FirstName" 
                                      PropertyName="Text" />
                <asp:ControlParameter ControlID="txtRequestId" 
                                      DbType="String"
                                      Name="RequestId" 
                                      PropertyName="Text" />
                <asp:ControlParameter ControlID="hdnTuitWW" 
                                      Name="TuitWW"
                                      PropertyName="Value" 
                                      Type="String" />
            </SelectParameters>
		</asp:SqlDataSource>

        <asp:GridView ID="gvResults" 
                        runat="server" 
                        AllowPaging="True" 
                        AllowSorting="True" 
                        AutoGenerateColumns="False"
                        CellPadding="4" 
                        ClientIDMode="Static"
                        DataKeyNames="EmplId"
                        DataSourceID="sdsResults"
                        EmptyDataText="No Data Found" 
                        OnRowDataBound="gvResults_RowDataBound"
                        OnSelectedIndexChanged="gvResults_SelectedIndexChanged"
                        PagerSettings-Mode="NumericFirstLast"
                        PageSize="20"
                        ShowHeaderWhenEmpty="True">

            <%--     GridView Columns     --%>
            <Columns>
                <%--     Employee Id     --%>
                <asp:BoundField DataField="EmplId"
                                HeaderText="Employee ID" 
                                HeaderStyle-Width="100px" 
                                ItemStyle-Wrap="False" 
                                ReadOnly="True" 
                                SortExpression="EmplId" />

                <%--     Name     --%>
                <asp:BoundField DataField="Name"
                                HeaderText="Name" 
                                HeaderStyle-Width="150px" 
                                ItemStyle-Wrap="False" 
                                ReadOnly="True" 
                                SortExpression="Name" />

                <%--     Job Title & Organziation     --%>
                <asp:TemplateField HeaderText="Job Title/Organization" 
                                   ControlStyle-Width="150px" 
                                   ItemStyle-HorizontalAlign="Left"
                                   ItemStyle-Wrap="False"
                                   SortExpression="Job_Org">
                    <ItemTemplate>
                        <%# ((string)Eval("Job_Org")).Replace("\n", "<br/>")%>
                    </ItemTemplate>
                </asp:TemplateField>

                <%--     Address     --%>
                <asp:TemplateField HeaderText="Business Address" 
                                   ControlStyle-Width="150px" 
                                   ItemStyle-HorizontalAlign="Left"
                                   ItemStyle-Wrap="False"
                                   SortExpression="Business_Address">
                    <ItemTemplate>
                        <%# ((string)Eval("Business_Address")).Replace("\n", "<br/>")%>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Mgr_Name" 
                                HeaderText="Manager" 
                                HeaderStyle-Width="150px" 
                                ItemStyle-Wrap="False" 
                                ReadOnly="True" 
                                SortExpression="Mgr_Name" />

                <%--     Select Button    --%>
                <asp:CommandField ButtonType="Button"
                                  HeaderStyle-CssClass="hideColumn"
                                  ItemStyle-CssClass="hideColumn"
                                  ShowSelectButton="True" />
            </Columns>

            <%--     GridView Styles     --%>
            <AlternatingRowStyle CssClass="gvAlternatingRow" />                    
            <EditRowStyle CssClass="gvEditRow" />
            <FooterStyle CssClass="gvFooter" />
            <HeaderStyle CssClass="gvHeader" />
            <PagerStyle CssClass="gvPager" />
        </asp:GridView>
    </asp:Panel>
</asp:Content>
