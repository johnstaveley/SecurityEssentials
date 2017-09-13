using SecurityEssentials.Core.Constants;
using System.Web;
using System.Web.Mvc;

namespace SecurityEssentials.Core.Identity
{
    public interface IUserIdentity
    {
        int GetUserId(Controller controller);

        string GetUserName(Controller controller);

        bool IsUserInRole(Controller controller, string role);

		string GetClientIpAddress(HttpRequestBase request);

		Requester GetRequester(Controller controller, AppSensorDetectionPointKind? appSensorDetectionPointKind = null);
	    void RemoveAntiForgeryCookie(Controller controller);

    }
}