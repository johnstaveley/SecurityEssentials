using System.Web.Mvc;

namespace SecurityEssentials.Core
{
    public interface IRecaptcha
    {
        bool ValidateRecaptcha(Controller controller);
    }
}