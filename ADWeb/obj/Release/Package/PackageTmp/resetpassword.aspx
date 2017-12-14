<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="resetpassword.aspx.vb" Inherits="ADWeb.resetpassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            text-align: center;
        }
        .auto-style2 {
            text-align: left;
            height: 31px;
        }
        .auto-style3 {
            text-align: justify;
            height: 43px;
            z-index: 1;
            left: 8px;
            top: -23px;
            position: relative;
            width: 1265px;
        }
    </style>
</head>
<body style="z-index: 1; left: 0px; top: 0px; position: relative; height: 270px; width: 1265px">
    <form id="form1" runat="server">
        <div>
        </div>
        <p class="auto-style3">
            <asp:Label ID="Label1" runat="server" style="z-index: 1; left: 3px; top: 11px; position: absolute; bottom: 31px; width: 98px;" Text="UserName:"></asp:Label>
            <asp:TextBox ID="txtUserName" runat="server" style="position: absolute; top: 14px; left: 131px; height: 23px; width: 227px; z-index: 1;"></asp:TextBox>
        </p>
        <p class="auto-style3">
            <asp:Label ID="Label2" runat="server" style="position: absolute; z-index: 1; top: 17px; left: -1px; height: 19px; width: 52px; bottom: 68px;" Text="Email:"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" style="position: absolute; top: 15px; left: 136px; width: 207px; height: 23px; z-index: 1;"></asp:TextBox>
        </p>
        <p class="auto-style1">
            &nbsp;</p>
        <p class="auto-style2">
            &nbsp;</p>
        <p>
            &nbsp;</p>
        <p style="position: relative; top: -160px; left: 7px; height: 71px">
            <asp:Button ID="btnSubmit" runat="server" style="height: 25px" Text="Submit" />
            <br />
            <br />
            <asp:Label ID="lblMsg" runat="server" style="z-index: 1; left: 126px; top: 224px; position: relative; width: 663px"></asp:Label>
        </p>
        <p>
            &nbsp;</p>
        <p>
            &nbsp;</p>
        <p>
            &nbsp;</p>
    </form>
</body>
</html>
