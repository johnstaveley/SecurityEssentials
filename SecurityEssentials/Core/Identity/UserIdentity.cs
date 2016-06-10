using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SecurityEssentials.Core.Identity
{
    public class UserIdentity : IUserIdentity
    {

        public int GetUserId(Controller controller)
        {
            return Convert.ToInt32(controller.User.Identity.GetUserId());

        }
        public string GetUserName(Controller controller)
        {
            return controller.User.Identity.Name;
        }
    }
}