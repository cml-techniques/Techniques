﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlyReport.aspx.cs" Inherits="CmlTechniques.CMS.MonthlyReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
    <link href="../page.css" rel="stylesheet" type="text/css" />
    <link href="../Assets/css/Style.css" rel="stylesheet" type="text/css" />
    <link href="../Assets/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function pageLoad() {
        }
    
    </script>

    <style type="text/css">
        .gvHeaderRow
        {
            background-image: url('../images/head_bg.png');
            background-repeat: repeat;
            font-family: Verdana;
            font-size: x-small;
            font-weight: normal;
        }
        .btnstyle
        {
            font-size: xx-small;
            cursor: pointer;
            color: Red;
        }
     
    </style>

    <script type="text/javascript">
        function Fullscreen() {
            var myFrameset = parent.document.getElementById("main");
            var value = myFrameset.getAttribute("cols");
            if (value == "210px,100%") {
                parent.document.getElementById("main").cols = "0px,100%";
                parent.document.getElementById("container").rows = "0px,100%";
            }
            else {
                parent.document.getElementById("main").cols = "210px,100%";
                parent.document.getElementById("container").rows = "115px,100%";
            }
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:Label ID="lblprj" runat="server" Text="" Style="display: none"></asp:Label>
            <asp:Label ID="lblid" runat="server" Text="" style="display:none"></asp:Label>
    <asp:Label ID="lblmod" runat="server" Text="" style="display:none"></asp:Label>
    <div class="fixedhead">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <div id="infobar">
                    <div class="prjinfo">
                        <div class="pullleft font-big">
                            <a href="#" onclick="Fullscreen();"><i class="fa fa-align-justify"></i></a> CMS :
                            <asp:Label ID="prj" runat="server" Style="color: #e6422c"></asp:Label></div>
                        <div class="pullright font-big">
                            <asp:Label ID="package" runat="server" Style="color: #e6422c"></asp:Label>
                        </div>
                    </div>
                </div>
                <div id="pagetitle" runat="server">
                <div class="title pull-left">
                <asp:Label ID="phead" runat="server" Text="Monthly Reports"></asp:Label>
                </div>
                    
                    <div class="btns pull-right">
         <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                        <asp:Button ID="btnuploadnew" runat="server" Text="Upload New" onclick="btnuploadnew_Click" />
             </ContentTemplate>
             </asp:UpdatePanel>
        </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="fixedcontent">
    <div id="doc_list" >
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Repeater ID="rpt_latest" runat="server" 
                    OnItemCommand="rpt_latest_ItemCommand" 
                    onitemdatabound="rpt_latest_ItemDataBound">
                    <HeaderTemplate>
                        <table class="tablestyle" width="100%" cellpadding="0" cellspacing="0" border="0">
                            <tr class="head">
                                <td style="width: 39%;" align="left">
                                    Document Name
                                </td>
                                <td style="width: 39%" align="left">
                                    File Name
                                </td>
                                <td style="width: 7%" align="center">
                                    Version
                                </td>
                                <td style="width: 10%" align="center">
                                    Upload Date
                                </td>
                                <td style="width: 5%" align="center" id="tdHistory" runat="server">
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="row">
                             <td style="width: 39%" align="left">
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("doc_name")%>'></asp:Label>
                            </td>
                            <td style="width: 39%;" align="left">
                            <asp:LinkButton ID="btnview" runat="server"  Text ='<%# DataBinder.Eval(Container.DataItem, "file_name")%>' CommandName="view"  CommandArgument='<%# Container.ItemIndex %>'  /> 
                             <asp:Label ID="lbldocname" runat="server" Text='<%# Eval("file_name")%>' style="display:none"></asp:Label>
                            </td>
                            <td style="width: 7%" align="center">
                                <%# Eval("Version")%>
                            </td>
                            <td style="width: 10%" align="center">
                                <%# Eval("upload_date","{0:dd-MM-yyyy}") %>
                            </td>
                            <td style="width: 5%" align="center" id="tdHistory" runat="server">
                                <asp:ImageButton ID="btnbutton" runat="server" ImageUrl="~/images/delete_.png" CommandName="delete1"
                                    CommandArgument='<%# Container.ItemIndex %>' />
                                    <asp:Label ID="lbldocid" runat="server" Text='<%# Eval("doc_id")%>' style="display:none"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </asp:UpdatePanel>
       </div>
     </div>
    <asp:UpdatePanel ID="UpdatePane5" runat="server">
        <ContentTemplate>
            <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="Panel1"
                TargetControlID="lblqns">
            </asp:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server" Style="display: none; width: 200px; background-color: White;
                border-width: 2px; border-color: Black; border-style: solid; padding: 20px;">
                <asp:Label ID="lblqns" Text="" runat="server"></asp:Label>
                <asp:Label ID="lblitmid" Text="0" runat="server" style="display:none"></asp:Label>
                <br />
                <br />
                <asp:Button ID="Button3" runat="server" Text="OK" OnClick="Delete_Click" />
                <asp:Button ID="Button4" runat="server" Text="Cancel" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>