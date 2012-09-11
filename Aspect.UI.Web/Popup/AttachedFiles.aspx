<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="AttachedFiles.aspx.cs" Inherits="Aspect.UI.Web.Popup.AttachedFiles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<link href="../css/fileuploader.css" rel="stylesheet" type="text/css">	
<div style="min-width:300px;width:100%;overflow:auto;float:left;border-right:dotted 1px black;height:100%;height:450px;">
     <table class="type1">
        <tr>
            <th colspan="3">
                Присоединённые файлы               
            </th>
        </tr>
        <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="ItemCommand">
            <ItemTemplate>                
                <tr>
                    <td>
                        <asp:HiddenField runat="server" ID="hiddenID" Value="<%# (Container.DataItem as FileRow).ID %>" />
                        <asp:TextBox  EnableViewState="true" Width="100%" ID="textUserName"  runat="server" Text="<%# (Container.DataItem as FileRow).UserName %>"></asp:TextBox></td>
                    <td>
                        <asp:HyperLink runat="server" Text="Скачать" NavigateUrl="<%# (Container.DataItem as FileRow).FileName %>"></asp:HyperLink></td>
                    <td>
                        <asp:LinkButton runat="server" Text="Удалить" CommandName="com.delete" CommandArgument="<%# (Container.DataItem as FileRow).ID %>"></asp:LinkButton></td>                    
                </tr>
            </ItemTemplate>
        </asp:Repeater>        
    </table>    
</div>

<div style="border-top:solid 1px black;width:100%;float:right">
    <div style="float:left;padding:10px;">
        <asp:FileUpload id="FileUpload1" runat="server">
        </asp:FileUpload>
        <asp:Button id="UploadButton" 
           Text="Загрузить файл"
           OnClick="UploadButton_Click"
           runat="server">
        </asp:Button>    
    </div>
    <div style="float:right;padding:15px 30px 0px 0px;">         
        <span style="margin-right:10px"> 
            <asp:LinkButton  runat="server" Text="Сохранить имена" OnClick="SavaFileNames"></asp:LinkButton></span>
        <a onclick="self.parent.tb_remove();return false;" href="javascript:void(0);">Закрыть</a>
    </div>
</div>
</asp:Content>
