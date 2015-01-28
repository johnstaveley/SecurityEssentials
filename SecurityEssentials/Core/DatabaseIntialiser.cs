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

			context.Role.Add(new Role() { Id = 1, Description = "Admin" });

			context.User.Add(new User() { 
				Id = 1, 
				DateCreated = DateTime.Now, 
				FirstName = "Admin", 
				LastName = "User",
				PasswordHash = "BpC/5HcMA4pnktXCPGY6HeNY9fPPk24JvvN2YyR3JFcd2j6Nen0sZHrf1mucLSMuuxp3CfHWaPIct8jp11YYyUXgihhS+9VA4OUJVz7Ak1uvuT6M+qItK1+tdlsihrpk3PkiuWafte0lcStImz2sCJroxtoGzOxOGSnpFehPIgd5TZBvmI3Crphdxq/dJhRwHIVQrnrXzwA+Aapy3bcXvutFmxS9F3/31BU4F5dJcYWHu+KbPydUlFl7RnM6A7DsnNKVcoDnk1CJZiJCz7WWNos+m+iv0CBE4ENDuP20sLW6x51S/ktcz3mdbn9wT38JM5CoLbS1UdVxdYC+Dkv+kQ==", // Password xsHDjxshdjkKK917&
				Salt = "K6GuRmwFwOupdDba+C1FqKYwyBuxCykesgiY+fmCVBNVwr7qafuQ7oj9HrgM3LTXMB9LtOkWc4Z7VzB3AjobRk4trmwy7yOyvXnZj9XcBom2s5htHz8tiYhgsV/fHLlNfbeFseOXMLqUN4AFf+/+07j2NiaQK+qLFDSOAFpvsfB6kHF5vk2JgJb8qQSaLAW5FrDFn4f6cqYQJg8H127xPm8WYJiU94sw4dd13XxneKUbzez3yikR20U7rfQMRFKUr2a14vApH4kGsg3F89n8B+w2A/Orz/iarA9uzATag0t2r5MPnQeG58odK5uOPTbWz1mka+gXVcY620SAdyo07Q==", // Password xsHDjxshdjkKK917&
				Title = "Mrs",
				UserName = "Admin"
			}); 
			context.User.Add(new User() { 
				Id = 2, 
				DateCreated = DateTime.Now, 
				FirstName = "Standard", 
				LastName = "User",
				PasswordHash = "8FEhrfoeG+vhIwFUEGvEMv/dSYs4dfnjZJRMeSrgkOtrnwPUAwc4Y35eGiRMU3Gw2NS/sCbnHUpeOn+4kNR/AN+FuyfkZBuZZ/72WiAbFIy+o8CzlCSkvv3H7z2rXyw6UgQXJtYcFLJr0GfIZC2xOWbeaBHvFJLpyxkvXORLHzAM779UPHS9at+wOwAr0cf0nfEIpI58tzCQCadBi9fyg4lxDEvpkISDFxE7YKiSrAp5bofosOnNuDRdidBUwqOGvZM9IvaNUigSpY6LKXpe/x7pv72+4jnSmK5QFylXfA33dc8Jl5r1LGDsJa/hKajoCjACYeD0L5ShPd4RrKbPCw==", // Password x12a;pP02icdjshER
				Salt = "weSUvc9heWYq/6v1OeefzxiFoQBW8f0+g2nV7d77xGeOwKLR5FG/KTWyjfBED7g3vQIr2lL7Nm6kY1XfQQwAL5A6dhy2lS7CSLxUnmifIPqThKuzyL54xzRfBIdqtrAT+TF74BeMXoIW/KdFXYdHMf8hgSHbDyKQkQQ29bpSLb/ieQPniwTeQTUkI+FE5Mgz2wst2uM/76GWo5QIkxRztQ141I0dpdFn7XoNdOFmMnyg2wDceK73nWi3E4ehuHHGuKLfxQTeRKpV183OW6RHMMSpt97g6VPSS1S367nTMHjj0fYFEtBgdSDPHXdpA0m1ZJwbPzzv+xOX0TIBGdNJdQ==", // Password x12a;pP02icdjshER
				Title = "Mr", 
				UserName = "User" });
			base.Seed(context);
		}
	}
}