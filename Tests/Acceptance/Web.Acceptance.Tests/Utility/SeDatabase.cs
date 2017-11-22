using SecurityEssentials.Core;
using SecurityEssentials.Model;
using System.Collections.Generic;
using System.Linq;

namespace SecurityEssentials.Acceptance.Tests.Utility
{
	public static class SeDatabase
	{

		public static void ClearDatabase()
		{
			SeContext seContext = new SeContext();
			var existingUsers = seContext.User
				.Include("PreviousPasswords")
				.Include("UserLogs")
				.Include("UserRoles")
				.ToList();
			existingUsers.ForEach(a => { a.PreviousPasswords.ToList().ForEach(b => seContext.SetDeleted(b)); });
			existingUsers.ForEach(a => { a.UserLogs.ToList().ForEach(b => seContext.SetDeleted(b)); });
			existingUsers.ForEach(a => { a.UserRoles.ToList().ForEach(b => seContext.SetDeleted(b)); });
			seContext.SaveChanges();
			existingUsers.ForEach(a => { seContext.SetDeleted(a); });
			var lookupItems = seContext.LookupItem.ToList();
			foreach (var lookupItem in lookupItems) { seContext.SetDeleted(lookupItem); }
			seContext.SaveChanges();
			var lookupTypes = seContext.LookupType.ToList();
			foreach (var lookupType in lookupTypes) { seContext.SetDeleted(lookupType); }
			seContext.SaveChanges();
			var logs = seContext.Log.ToList();
			foreach (var log in logs) { seContext.SetDeleted(log); }
			seContext.SaveChanges();
		}

		public static List<LookupItem> GetLookupItemsByLookupType(int lookupTypeId)
		{
			SeContext seContext = new SeContext();
			return seContext.LookupItem.Where(a => a.LookupTypeId == lookupTypeId).ToList();
		}
		public static LookupItem GetLookupItemByLookupTypeAndDescription(int lookupTypeId, string description)
		{
			SeContext seContext = new SeContext();
			return seContext.LookupItem.Single(a => a.LookupTypeId == lookupTypeId && a.Description == description);
		}
	
		public static List<PreviousPassword> GetPreviousPasswords()
		{
			SeContext seContext = new SeContext();
			return seContext.PreviousPassword.ToList();
		}	
		public static Role GetRoleByName(string description)
		{
			SeContext seContext = new SeContext();
			return seContext.Role.SingleOrDefault(a => a.Description == description);
		}
		public static List<Log> GetSystemErrors()
		{
			SeContext seContext = new SeContext();
			return seContext.Log.Where(a => a.Level == "Error").ToList();
		}
		public static List<Log> GetCspWarnings()
		{
			SeContext seContext = new SeContext();
			return seContext.Log.Where(a => a.Level == "Warning" && a.Message.StartsWith("Content Security Policy Violation")).ToList();
		}
		public static User GetUserByUserName(string userName)
		{
			SeContext seContext = new SeContext();
			return seContext.User.Single(a => a.UserName == userName);
		}

		public static List<UserLog> GetUserLogs()
		{
			SeContext seContext = new SeContext();
			return seContext.UserLog.ToList();
		}
		public static List<User> GetUsers()
		{
			SeContext seContext = new SeContext();
			return seContext.User.ToList();
		}
	
		public static void SetLogs(List<Log> logs)
		{
			SeContext seContext = new SeContext();
			logs.ForEach(a => seContext.Log.Add(a));
			seContext.SaveChanges();
		}
		public static void SetLookupItems(List<LookupItem> lookupItems)
		{
			SeContext seContext = new SeContext();
			lookupItems.ForEach(a => seContext.LookupItem.Add(a));
			seContext.SaveChanges();
		}
		public static void SetLookupTypes(List<LookupType> lookupTypes)
		{
			SeContext seContext = new SeContext();
			lookupTypes.ForEach(a => seContext.LookupType.Add(a));
			seContext.SaveChanges();
		}	
		public static void SetRoles(List<Role> roles)
		{
			SeContext seContext = new SeContext();
			roles.ForEach(a => seContext.Role.Add(a));
			seContext.SaveChanges();
		}

		public static void SetUserRoles(List<UserRole> userRoles)
		{
			SeContext seContext = new SeContext();
			userRoles.ForEach(a => seContext.UserRole.Add(a));
			seContext.SaveChanges();
		}
		public static void SetUsers(List<User> users)
		{
			SeContext seContext = new SeContext();
			users.ForEach(a => seContext.User.Add(a));
			seContext.SaveChanges();
		}	
	}
}
