using System.Web;
using System.Web.Mvc;
using SecurityEssentials.Core.Constants;

namespace SecurityEssentials.Core.Identity
{
    public interface IUserIdentity
    {
        int GetUserId(Controller controller);

        string GetUserName(Controller controller);

        bool IsUserInRole(Controller controller, string role);

        string GetClientIpAddress(HttpRequestBase request);

        Requester GetRequester(Controller controller, AppSensorDetectionPointKind? appSensorDetectionPointKind = null);
    }
}