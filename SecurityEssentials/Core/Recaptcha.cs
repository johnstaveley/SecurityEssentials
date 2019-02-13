using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System;
using System.Web.Mvc;

namespace SecurityEssentials.Core
{
    public class SecurityCheckRecaptcha : IRecaptcha
    {

        public bool ValidateRecaptcha(Controller controller)
        {

            bool recaptchaSuccess = true;

            RecaptchaVerificationHelper recaptchaHelper = controller.GetRecaptchaVerificationHelper();

            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                controller.ModelState.AddModelError("", "Captcha answer cannot be empty.");
                recaptchaSuccess = false;
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                controller.ModelState.AddModelError("", "Incorrect captcha answer.");
                recaptchaSuccess = false;
            }
            return recaptchaSuccess;
        }


    }
}