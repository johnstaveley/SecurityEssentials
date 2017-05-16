using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityEssentials.Core
{
    public interface IAppSensor
    {
        void ValidateFormData(Controller controller, List<string> expectedFormKeys);
        void InspectModelStateErrors(Controller controller);
    }
}