using Newtonsoft.Json;
using SecurityEssentials.Core.Attributes;
using SecurityEssentials.Model;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace SecurityEssentials.Controllers
{

	public class SecurityController : Controller
	{

		[HttpPost]
		[AllowXRequestsEveryXSeconds(Name = "CspReporting", ContentName = "TooManyRequests", Requests = 10, Seconds = 60)]
		public JsonResult CspReporting([ModelBinder(typeof(JsonNetModelBinder))] CspHolder data)
		{
			Serilog.Log.Logger.Warning($"Content Security Policy Violation {data}");
//			Serilog.Log.Logger.Warning($"Content Security Policy Violation {data.CspReport?.BlockedUri}");
			return Json(new {success = true});
		}
		
	}

	internal class JsonNetModelBinder : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			controllerContext.HttpContext.Request.InputStream.Position = 0;
			var stream = controllerContext.RequestContext.HttpContext.Request.InputStream;
			var readStream = new StreamReader(stream, Encoding.UTF8);
			var json = readStream.ReadToEnd();
			return JsonConvert.DeserializeObject(json, bindingContext.ModelType);
		}
	}
}
