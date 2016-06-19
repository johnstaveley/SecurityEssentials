using System.Web.Mvc;

namespace SecurityEssentials.Core.Identity
{
    public interface IUserIdentity
    {
        int GetUserId(Controller controller);

        string GetUserName(Controller controller);

        bool IsUserInRole(Controller controller, string role);
    }
}