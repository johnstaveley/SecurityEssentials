using System.Web.Mvc;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;

namespace SecurityEssentials.Core
{
    public class SecurityCheckRecaptcha : IRecaptcha
    {
        public bool ValidateRecaptcha(Controller controller)
        {
            var recaptchaSuccess = true;

            var recaptchaHelper = controller.GetRecaptchaVerificationHelper();

            if (string.IsNullOrEmpty(recaptchaHelper.Response))
            {
                controller.ModelState.AddModelError("", "Captcha answer cannot be empty.");
                recaptchaSuccess = false;
            }

            var recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                controller.ModelState.AddModelError("", "Incorrect captcha answer.");
                recaptchaSuccess = false;
            }
            return recaptchaSuccess;
        }
    }
}