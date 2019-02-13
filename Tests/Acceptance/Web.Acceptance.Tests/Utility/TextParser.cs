using System;

namespace SecurityEssentials.Acceptance.Tests.Utility
{
	public static class TextParser
	{
		public static DateTime? ConvertDescriptionToNullableDate(string humanDate)
		{
			if (string.IsNullOrWhiteSpace(humanDate)) return null;
			if (!(humanDate.IndexOf("[", StringComparison.Ordinal) >= 0)) return DateTime.Parse(humanDate);
			humanDate = humanDate.Replace("[", "").Replace("]", "");
			switch (humanDate.ToLower())
			{
				case "1 year ago":
					return DateTime.Now.Date.AddYears(-1);
				case "1.5 years ago":
					return DateTime.Now.Date.AddYears(-1).AddMonths(-6);
				case "1 month ago":
					return DateTime.Now.Date.AddMonths(-1);
				case "1 month from now":
					return DateTime.Now.Date.AddMonths(1);
				case "4 days ago":
					return DateTime.Now.AddDays(-4);
				case "3 days ago":
					return DateTime.Now.AddDays(-3);
				case "2 days ago":
				case "day before yesterday":
					return DateTime.Now.AddDays(-2);
				case "1 day ago":
				case "yesterday":
					return DateTime.Now.AddDays(-1);
				case "tomorrow":
					return DateTime.Now.AddDays(1);
				case "today":
				case "now":
					return DateTime.Now;
			}

			throw new ArgumentException(nameof(humanDate));
		}

		public static DateTime ConvertDescriptionToDate(string humanDate, bool useTodayAsDefault = true)
		{
			if (string.IsNullOrWhiteSpace(humanDate))
			{
				if (useTodayAsDefault)
				{
					humanDate = "[Today]";
				}
				else
				{
					throw new ArgumentException(nameof(humanDate));
				}
			}
			var date = ConvertDescriptionToNullableDate(humanDate);
			if (!date.HasValue) throw new Exception($"Unable to convert date {humanDate} to valid datetime");
			return date.Value;
		}
	}
}
