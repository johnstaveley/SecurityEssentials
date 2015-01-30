using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SecurityEssentials.Model;

namespace SecurityEssentials.Core
{
	public class SEDatabaseIntialiser : DropCreateDatabaseAlways<SEContext>
	{

		protected override void Seed(SEContext context)
		{

			// Roles
			var adminRole = new Role() { Id = 1, Description = "Admin" };
			context.Role.Add(adminRole);

			// Users
			context.User.Add(new User() { 
				Id = 1,
				Approved = true,
				DateCreated = DateTime.Now,
				Enabled = true,
				FirstName = "Admin", 
				LastName = "User",
				TelNoMobile = "07740101235",
				PasswordHash = "BpC/5HcMA4pnktXCPGY6HeNY9fPPk24JvvN2YyR3JFcd2j6Nen0sZHrf1mucLSMuuxp3CfHWaPIct8jp11YYyUXgihhS+9VA4OUJVz7Ak1uvuT6M+qItK1+tdlsihrpk3PkiuWafte0lcStImz2sCJroxtoGzOxOGSnpFehPIgd5TZBvmI3Crphdxq/dJhRwHIVQrnrXzwA+Aapy3bcXvutFmxS9F3/31BU4F5dJcYWHu+KbPydUlFl7RnM6A7DsnNKVcoDnk1CJZiJCz7WWNos+m+iv0CBE4ENDuP20sLW6x51S/ktcz3mdbn9wT38JM5CoLbS1UdVxdYC+Dkv+kQ==", // Password xsHDjxshdjkKK917&
				Salt = "K6GuRmwFwOupdDba+C1FqKYwyBuxCykesgiY+fmCVBNVwr7qafuQ7oj9HrgM3LTXMB9LtOkWc4Z7VzB3AjobRk4trmwy7yOyvXnZj9XcBom2s5htHz8tiYhgsV/fHLlNfbeFseOXMLqUN4AFf+/+07j2NiaQK+qLFDSOAFpvsfB6kHF5vk2JgJb8qQSaLAW5FrDFn4f6cqYQJg8H127xPm8WYJiU94sw4dd13XxneKUbzez3yikR20U7rfQMRFKUr2a14vApH4kGsg3F89n8B+w2A/Orz/iarA9uzATag0t2r5MPnQeG58odK5uOPTbWz1mka+gXVcY620SAdyo07Q==", // Password xsHDjxshdjkKK917&
				SecurityAnswer = "Chairman Meow",
				SecurityQuestionLookupItemId = 271,
				Title = "Mrs",
				UserName = "admin@admin.com",
				UserRoles = new List<UserRole>() { new UserRole() { RoleId = adminRole.Id, UserId = 1 } }
				//,PasswordResetToken = "abc",
				//PasswordResetExpiry = DateTime.Now.AddDays(1)
			}); 

			context.User.Add(new User() { 
				Id = 2,
				Approved = true,
				DateCreated = DateTime.Now,
				Enabled = true,
				FirstName = "Standard", 
				LastName = "User",
				TelNoMobile = "07881231234",
				PasswordHash = "8FEhrfoeG+vhIwFUEGvEMv/dSYs4dfnjZJRMeSrgkOtrnwPUAwc4Y35eGiRMU3Gw2NS/sCbnHUpeOn+4kNR/AN+FuyfkZBuZZ/72WiAbFIy+o8CzlCSkvv3H7z2rXyw6UgQXJtYcFLJr0GfIZC2xOWbeaBHvFJLpyxkvXORLHzAM779UPHS9at+wOwAr0cf0nfEIpI58tzCQCadBi9fyg4lxDEvpkISDFxE7YKiSrAp5bofosOnNuDRdidBUwqOGvZM9IvaNUigSpY6LKXpe/x7pv72+4jnSmK5QFylXfA33dc8Jl5r1LGDsJa/hKajoCjACYeD0L5ShPd4RrKbPCw==", // Password x12a;pP02icdjshER
				Salt = "weSUvc9heWYq/6v1OeefzxiFoQBW8f0+g2nV7d77xGeOwKLR5FG/KTWyjfBED7g3vQIr2lL7Nm6kY1XfQQwAL5A6dhy2lS7CSLxUnmifIPqThKuzyL54xzRfBIdqtrAT+TF74BeMXoIW/KdFXYdHMf8hgSHbDyKQkQQ29bpSLb/ieQPniwTeQTUkI+FE5Mgz2wst2uM/76GWo5QIkxRztQ141I0dpdFn7XoNdOFmMnyg2wDceK73nWi3E4ehuHHGuKLfxQTeRKpV183OW6RHMMSpt97g6VPSS1S367nTMHjj0fYFEtBgdSDPHXdpA0m1ZJwbPzzv+xOX0TIBGdNJdQ==", // Password x12a;pP02icdjshER
				SecurityAnswer = "Mr Miggins",
				SecurityQuestionLookupItemId = 271,
				Title = "Mr",
				UserName = "user@user.com"
			});

			// Lookup Types
			context.LookupType.Add(new LookupType() { Id = 1, Description = "Bad passwords" }); // SECURE: I've only included passwords which comply with the password policy (ignorning case)
			context.LookupType.Add(new LookupType() { Id = 2, Description = "Security Questions" }); // SECURE: I've included some sample security questions which are difficult to obtain from the public domain

			// Lookup Items
			context.LookupItem.Add(new LookupItem() { Id = 1, Description = "primetime21", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 2, Description = "1234567890", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 3, Description = "postov1000", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 4, Description = "charlie123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 5, Description = "1q2w3e4r5t", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 6, Description = "123456789", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 7, Description = "password1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 8, Description = "987654321", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 9, Description = "iloveyou1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 10, Description = "football1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 11, Description = "123123123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 12, Description = "qwerty123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 13, Description = "birthday1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 14, Description = "baseball1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 15, Description = "slimed123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 16, Description = "fortune12", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 17, Description = "12345678", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 18, Description = "trustno1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 19, Description = "11111111", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 20, Description = "rush2112", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 21, Description = "87654321", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 22, Description = "1q2w3e4r", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 23, Description = "1qaz2wsx", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 24, Description = "abcd1234", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 25, Description = "blink182", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 26, Description = "ncc1701d", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 27, Description = "michael1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 28, Description = "letmein1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 29, Description = "charlie1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 30, Description = "qwerty12", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 31, Description = "q1w2e3r4", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 32, Description = "21122112", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 33, Description = "mustang1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 34, Description = "12341234", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 35, Description = "access14", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 36, Description = "asdf1234", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 37, Description = "12121212", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 38, Description = "wrinkle1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 39, Description = "1234qwer", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 40, Description = "ncc1701e", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 41, Description = "51505150", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 42, Description = "yankees1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 43, Description = "jessica1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 44, Description = "hello123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 45, Description = "freedom1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 46, Description = "1passwor", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 47, Description = "11223344", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 48, Description = "welcome1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 49, Description = "passwor1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 50, Description = "william1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 51, Description = "matthew1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 52, Description = "13131313", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 53, Description = "thunder1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 54, Description = "heather1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 55, Description = "anthony1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 56, Description = "asshole1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 57, Description = "chelsea1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 58, Description = "fuckyou1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 59, Description = "a1b2c3d4", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 60, Description = "1234abcd", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 61, Description = "richard1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 62, Description = "1qazxsw2", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 63, Description = "1x2zkg8w", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 64, Description = "ncc1701a", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 65, Description = "scooter1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 66, Description = "formula1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 67, Description = "12344321", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 68, Description = "raiders1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 69, Description = "cowboys1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 70, Description = "qwer1234", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 71, Description = "gateway1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 72, Description = "jackson1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 73, Description = "phoenix1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 74, Description = "diamond1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 75, Description = "patrick1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 76, Description = "zaq12wsx", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 77, Description = "melissa1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 78, Description = "rangers1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 79, Description = "gandalf1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 80, Description = "pool6123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 81, Description = "brandon1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 82, Description = "test1234", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 83, Description = "testing1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 84, Description = "fordf150", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 85, Description = "18436572", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 86, Description = "apollo13", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 87, Description = "happy123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 88, Description = "shannon1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 89, Description = "ferrari1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 90, Description = "chicago1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 91, Description = "arsenal1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 92, Description = "bulldog1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 93, Description = "panther1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 94, Description = "pass1234", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 95, Description = "jasmine1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 96, Description = "marino13", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 97, Description = "chicken1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 98, Description = "america1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 99, Description = "fishing1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 101, Description = "packers1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 102, Description = "chester1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 103, Description = "11235813", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 104, Description = "charles1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 105, Description = "florida1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 106, Description = "rebecca1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 107, Description = "19691969", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 108, Description = "10101010", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 109, Description = "newyork1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 110, Description = "digital1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 111, Description = "porsche1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 112, Description = "dolphin1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 113, Description = "21212121", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 114, Description = "scorpio1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 115, Description = "madison1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 116, Description = "tiffany1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 117, Description = "vikings1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 118, Description = "12qwaszx", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 119, Description = "playboy1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 120, Description = "captain1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 121, Description = "monster1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 122, Description = "31415926", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 123, Description = "1a2b3c4d", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 124, Description = "genesis1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 125, Description = "19841984", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 126, Description = "care1839", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 127, Description = "maxwell1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 128, Description = "broncos1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 129, Description = "crystal1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 130, Description = "winston1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 131, Description = "warrior1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 132, Description = "myspace1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 133, Description = "spencer1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 134, Description = "drummer1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 135, Description = "private1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 136, Description = "fuckoff1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 137, Description = "johnson1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 138, Description = "hotmail1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 139, Description = "kordell1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 140, Description = "vincent1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 141, Description = "windows1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 142, Description = "12312312", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 143, Description = "perfect1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 144, Description = "pantera1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 145, Description = "qwert123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 146, Description = "cezer121", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 147, Description = "19781978", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 148, Description = "1michael", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 149, Description = "dodgers1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 150, Description = "soccer12", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 151, Description = "soccer10", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 152, Description = "stephen1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 153, Description = "monkey12", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 154, Description = "voyager1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 155, Description = "cartman1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 156, Description = "green123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 157, Description = "sabrina1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 158, Description = "success1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 159, Description = "abc12345", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 160, Description = "apple123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 161, Description = "rainbow1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 162, Description = "cricket1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 163, Description = "20012001", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 164, Description = "cameron1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 165, Description = "a1234567", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 166, Description = "penguin1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 167, Description = "bigdick1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 168, Description = "buffalo1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 169, Description = "natalie1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 170, Description = "78945612", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 171, Description = "cygnusx1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 172, Description = "peaches1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 173, Description = "austin31", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 174, Description = "jupiter1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 175, Description = "trouble1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 176, Description = "gizmodo1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 177, Description = "yamahar1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 178, Description = "pussy123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 179, Description = "mercury1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 180, Description = "11112222", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 181, Description = "beatles1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 182, Description = "zachary1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 183, Description = "houston1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 184, Description = "phantom1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 185, Description = "151nxjmt", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 186, Description = "bubba123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 187, Description = "frankie1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 188, Description = "skipper1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 189, Description = "eclipse1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 190, Description = "tiger123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 191, Description = "13576479", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 192, Description = "therock1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 193, Description = "zaq1xsw2", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 194, Description = "pumpkin1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 195, Description = "special1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 196, Description = "patches1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 197, Description = "14789632", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 198, Description = "trinity1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 199, Description = "trooper1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 201, Description = "dilbert1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 202, Description = "bubbles1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 203, Description = "spartan1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 204, Description = "stanley1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 205, Description = "154ugeiu", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 206, Description = "abcdefg1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 207, Description = "buddy123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 208, Description = "melanie1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 209, Description = "jackass1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 210, Description = "money123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 211, Description = "claudia1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 212, Description = "zxcvbnm1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 213, Description = "aaaaaaa1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 214, Description = "nirvana1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 215, Description = "trumpet1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 216, Description = "basebal1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 217, Description = "gabriel1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 218, Description = "destiny1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 219, Description = "14141414", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 220, Description = "general1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 221, Description = "isacs155", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 222, Description = "allison1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 223, Description = "1million", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 224, Description = "1letmein", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 225, Description = "express1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 226, Description = "gregory1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 227, Description = "gsxr1000", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 228, Description = "dragon12", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 229, Description = "russell1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 230, Description = "kristin1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 231, Description = "vampire1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 232, Description = "vanessa1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 233, Description = "12345679", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 234, Description = "14725836", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 235, Description = "montana1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 236, Description = "blue1234", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 237, Description = "alpha123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 238, Description = "xxxxxxx1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 239, Description = "marines1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 240, Description = "save13tx", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 241, Description = "17171717", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 242, Description = "17011701", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 243, Description = "stewart1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 244, Description = "911turbo", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 245, Description = "mounta1n", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 246, Description = "master12", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 247, Description = "sooners1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 248, Description = "skeeter1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 249, Description = "nancy123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 250, Description = "natasha1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 251, Description = "tmjxn151", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 252, Description = "chris123", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 253, Description = "551scasi", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 254, Description = "11001001", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 255, Description = "thumper1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 256, Description = "19741974", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 257, Description = "huskers1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 258, Description = "dreamer1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 259, Description = "samsung1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 260, Description = "soccer11", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 261, Description = "lincoln1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 262, Description = "12locked", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 263, Description = "arizona1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 264, Description = "paladin1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 265, Description = "shadow12", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 266, Description = "201jedlz", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 267, Description = "hooters1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 268, Description = "england1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 269, Description = "prelude1", LookupTypeId = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 270, Description = "19721972", LookupTypeId = 1 });

			// Security Questions
			context.LookupItem.Add(new LookupItem() { Id = 271, Description = "What is the name of your first pet?", LookupTypeId = 2, Ordinal = 2 });
			context.LookupItem.Add(new LookupItem() { Id = 272, Description = "What is your mother's maiden name?", LookupTypeId = 2, Ordinal = 1 });
			context.LookupItem.Add(new LookupItem() { Id = 273, Description = "What is the name of the first person you kissed?", LookupTypeId = 2, Ordinal = 4 });
			context.LookupItem.Add(new LookupItem() { Id = 274, Description = "What was your childhood nickname?", LookupTypeId = 2, Ordinal = 3 });
			context.LookupItem.Add(new LookupItem() { Id = 275, Description = "What was your favourite childhood film?", LookupTypeId = 2, Ordinal = 5 });

			base.Seed(context);
		}

	}
}