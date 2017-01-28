<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebClient.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
            <tr>
                <td> WNID</td>
                <td> <asp:TextBox ID="txtWnid" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td> Name</td>
                <td> <asp:TextBox ID="txtName" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td> Category</td>
                <td> <asp:TextBox ID="txtCategory" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td> Description</td>
                <td> <asp:TextBox ID="txtGloss" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td> Count of pictures </td>
                <td> <asp:TextBox ID="txtCount" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td> Popularity</td>
                <td> <asp:TextBox ID="txtPopularity" runat="server"></asp:TextBox></td>
            </tr>
             <tr>
                <td><asp:Button ID="btnGetWord" runat="server" Text="Get Word" OnClick="btnGetWord_Click" /></td>
                <td><asp:Button ID="btnSetWord" runat="server" Text="Set Word" OnClick="btnSetWord_Click" /></td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
