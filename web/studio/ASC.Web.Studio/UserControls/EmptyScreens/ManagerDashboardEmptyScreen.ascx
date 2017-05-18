﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManagerDashboardEmptyScreen.ascx.cs" Inherits="ASC.Web.Studio.UserControls.EmptyScreens.ManagerDashboardEmptyScreen" %>
<%@ Import Namespace="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.People.Core" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>
<%@ Import Namespace="Resources" %>

<div id="manager-empty-screen">
    <div class="header-base large"><%= string.Format(Resource.ManagerEmptyScreen_WelcomeHeader.HtmlEncode(), CurrentUserName) %></div>
    <div class="header-base"><%= Resource.ManagerEmptyScreen_StepsHeader %></div>

    <% if (!IsVisitor && !ProductDisabled(WebItemManager.PeopleProductID))
       { %>
    <div class="module-item clearFix">
        <span class="main-title-icon people"></span>
        <a href="<%= VirtualPathUtility.ToAbsolute(PeopleProduct.GetStartURL()) %>" class="link underline medium" target="_blank">
            <%= Resource.ManagerEmptyScreen_PeopleLink %>
        </a>
        <div class="module-link-dscr"><%= CustomNamingPeople.Substitute<Resource>("ManagerEmptyScreen_PeopleLinkDscr").HtmlEncode() %></div>
    </div>
    <% } %>

    <div class="module-item clearFix">
        <span class="main-title-icon documents"></span>
        <a href="<%= VirtualPathUtility.ToAbsolute(ASC.Web.Files.Classes.PathProvider.StartURL) %>" class="link underline medium" target="_blank">
            <%= Resource.ManagerEmptyScreen_DocumentsLink %>
        </a>
        <div class="module-link-dscr"><%= !IsVisitor ? Resource.ManagerEmptyScreen_DocumentsLinkDscr : string.Empty %></div>
    </div>

    <% if (!IsVisitor && !ProductDisabled(WebItemManager.CRMProductID))
       { %>
    <div class="module-item clearFix">
        <span class="main-title-icon crm"></span>
        <a href="<%= VirtualPathUtility.ToAbsolute(ASC.Web.CRM.PathProvider.StartURL()) %>" class="link underline medium" target="_blank">
            <%= Resource.ManagerEmptyScreen_CRMLink %>
        </a>
        <div class="module-link-dscr"><%= Resource.ManagerEmptyScreen_CRMLinkDscr %></div>
    </div>
    <% } %>

    <% if (!ProductDisabled(WebItemManager.ProjectsProductID))
       { %>
    <div class="module-item clearFix">
        <span class="main-title-icon projects"></span>
        <a href="<%= VirtualPathUtility.ToAbsolute(ASC.Web.Projects.Classes.PathProvider.BaseVirtualPath) %>" class="link underline medium" target="_blank">
            <%= Resource.ManagerEmptyScreen_ProjectsLink %>
        </a>
        <div class="module-link-dscr"><%= !IsVisitor ? Resource.ManagerEmptyScreen_ProjectsLinkDscr : string.Empty %></div>
    </div>
    <% } %>
</div>