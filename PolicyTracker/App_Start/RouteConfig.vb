Imports System.Web.Routing
Imports Microsoft.AspNet.FriendlyUrls

Public Module RouteConfig
    Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.MapPageRoute("", "PA/RecipGroupSettings/{RecipGroupId}", "~/PA/RecipGroupSettings.aspx", True)
        routes.MapPageRoute("", "OA/Release/{ReleaseId}", "~/OA/Release.aspx", True)
        routes.MapPageRoute("", "OA/ReleaseNotice/{ReleaseNoticeId}", "~/OA/ReleaseNotice.aspx", True)
        routes.MapPageRoute("", "USER/Packet/{ReleaseId}/{RecipientId}", "~/USER/Packet.aspx", True)
        routes.MapPageRoute("", "PA/Policy/{PolicyId}", "~/PA/Policies.aspx", True,
                            New RouteValueDictionary({"PolicyId", Nothing}))
        routes.MapPageRoute("", "PA/Schedule/{ScheduleId}", "~/PA/Schedules.aspx", True,
                            New RouteValueDictionary({"ScheduleId", Nothing}))

        Dim settings As FriendlyUrlSettings = New FriendlyUrlSettings()
        settings.AutoRedirectMode = RedirectMode.Permanent
        routes.EnableFriendlyUrls(settings)
    End Sub
End Module
