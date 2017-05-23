using System.ComponentModel;

namespace SecurityEssentials.Core.Constants
{

	/// <summary>
	/// Enumeration of the different AppSensor detection points
	/// </summary>
	/// <remarks>https://www.owasp.org/index.php/AppSensor_DetectionPoints</remarks>
	public enum AppSensorDetectionPointKind
	{
		/// <summary>
		/// Unexpected HTTP Command e.g. call to route that does not exist
		/// </summary>
		[Description("Unexpected HTTP Command")]
		Re1 = 1,
		/// <summary>
		/// Attempt to Invoke Unsupported HTTP Method e.g. HttpPut called on Login form
		/// </summary>
		[Description("Attempt to Invoke Unsupported HTTP Method")]
		Re2 = 2,
		Re3 = 3,
		Re4 = 4,
		[Description("Additional/Duplicated data in Request")]
		Re5 = 5,
		[Description("Data Missing from Request")]
		Re6 = 6,
		[Description("Unexpected Quantity of Characters in Parameter")]
		Re7 = 7,
		Re8 = 8,

		/// <summary>
		/// Multiple Failed Passwords - For the same user
		/// </summary>
		[Description("Multiple Failed Passwords")]
		Ae1 = 9,

		/// <summary>
		/// High Rate of login attempts (for different usernames)
		/// </summary>
		[Description("High Rate of login attempts")]
		Ae2 = 10,

		Ae3 = 11,
		[Description("Unexpected Quantity of Characters in Username")]
		Ae4 = 12,
		[Description("Unexpected Quantity of Characters in Password")]
		Ae5 = 13,
		[Description("Providing Only the Username")]
		Ae6 = 14,
		[Description("Providing Only the Password")]
		Ae7 = 15,
		Ae8 = 16,
		Ae9 = 17,

		/// <summary>
		/// Additional POST Variable - could be used to detect an MVC overposting attack
		/// </summary>
		[Description("Additional POST Variable")]
		Ae10 = 18,
		Ae11 = 19,

		/// <summary>
		/// Utilization of Common Usernames - if not currently in use, things like Administrator, admin, test etc
		/// </summary>
		[Description("Utilization of Common Usernames")]
		Ae12 = 20,

		[Description("Deviation from Normal GEO Location")]
		Ae13 = 21,

		Se1 = 22,
		Se2 = 23,
		Se3 = 24,
		Se4 = 25,
		Se5 = 26,
		Se6 = 27,
		Ace1 = 28,
		Ace2 = 29,
		Ace3 = 30,
		Ace4 = 31,
		[Description("Cross Site Scripting Attempt")]
		Ie1 = 32,
		[Description("Violation Of Implemented White Lists")]
		Ie2 = 33,
		[Description("Violation Of Implemented Black Lists")]
		Ie3 = 34,
		[Description("Violation Of Input data integrity")]
		Ie4 = 35,
		Ie5 = 36,
		Ie6 = 37,
		Ie7 = 38,
		Ee1 = 39,
		Ee2 = 40,
		[Description("Blacklist Inspection for Common SQL Injection Values")]
		Cie1 = 41,
		Cie2 = 42,
		Cie3 = 43,
		Cie4 = 44,
		Fio1 = 45,
		Fio2 = 46,
		Ht1 = 47,
		Ht2 = 48,
		Ht3 = 49,
		Ut1 = 50,
		Ut2 = 51,
		Ut3 = 53,
		Ut4 = 54,
		Ste1 = 55,
		Ste2 = 56,
		Ste3 = 57,
		Rp1 = 58,
		Rp2 = 59,
		Rp3 = 60,
		Rp4 = 61

	}
}