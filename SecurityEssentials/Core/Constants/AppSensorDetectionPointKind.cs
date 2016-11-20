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
		RE1 = 1,
		/// <summary>
		/// Attempt to Invoke Unsupported HTTP Method e.g. HttpPut called on Login form
		/// </summary>
		[Description("Attempt to Invoke Unsupported HTTP Method")]
		RE2 = 2,
		RE3 = 3,
		RE4 = 4,
		[Description("Additional/Duplicated data in Request")]
		RE5 = 5,
		[Description("Data Missing from Request")]
		RE6 = 6,
		[Description("Unexpected Quantity of Characters in Parameter")]
		RE7 = 7,
		RE8 = 8,

		/// <summary>
		/// Multiple Failed Passwords - For the same user
		/// </summary>
		[Description("Multiple Failed Passwords")]
		AE1 = 9,

		/// <summary>
		/// High Rate of login attempts (for different usernames)
		/// </summary>
		[Description("High Rate of login attempts")]
		AE2 = 10,

		AE3 = 11,
		[Description("Unexpected Quantity of Characters in Username")]
		AE4 = 12,
		[Description("Unexpected Quantity of Characters in Password")]
		AE5 = 13,
		[Description("Providing Only the Username")]
		AE6 = 14,
		[Description("Providing Only the Password")]
		AE7 = 15,
		AE8 = 16,
		AE9 = 17,

		/// <summary>
		/// Additional POST Variable - could be used to detect an MVC overposting attack
		/// </summary>
		[Description("Additional POST Variable")]
		AE10 = 18,
		AE11 = 19,

		/// <summary>
		/// Utilization of Common Usernames - if not currently in use, things like Administrator, admin, test etc
		/// </summary>
		[Description("Utilization of Common Usernames")]
		AE12 = 20,

		[Description("Deviation from Normal GEO Location")]
		AE13 = 21,

		SE1 = 22,
		SE2 = 23,
		SE3 = 24,
		SE4 = 25,
		SE5 = 26,
		SE6 = 27,
		ACE1 = 28,
		ACE2 = 29,
		ACE3 = 30,
		ACE4 = 31,
		[Description("Cross Site Scripting Attempt")]
		IE1 = 32,
		[Description("Violation Of Implemented White Lists")]
		IE2 = 33,
		[Description("Violation Of Implemented Black Lists")]
		IE3 = 34,
		[Description("Violation Of Input data integrity")]
		IE4 = 35,
		IE5 = 36,
		IE6 = 37,
		IE7 = 38,
		EE1 = 39,
		EE2 = 40,
		[Description("Blacklist Inspection for Common SQL Injection Values")]
		CIE1 = 41,
		CIE2 = 42,
		CIE3 = 43,
		CIE4 = 44,
		FIO1 = 45,
		FIO2 = 46,
		HT1 = 47,
		HT2 = 48,
		HT3 = 49,
		UT1 = 50,
		UT2 = 51,
		UT3 = 53,
		UT4 = 54,
		STE1 = 55,
		STE2 = 56,
		STE3 = 57,
		RP1 = 58,
		RP2 = 59,
		RP3 = 60,
		RP4 = 61

	}
}