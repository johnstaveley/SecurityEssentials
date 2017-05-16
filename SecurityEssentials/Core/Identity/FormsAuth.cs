using System.Web.Security;

namespace SecurityEssentials.Core.Identity
{
    public class FormsAuth : IFormsAuth
    {
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}