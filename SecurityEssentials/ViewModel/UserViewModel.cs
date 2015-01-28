using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using SecurityEssentials.Models;

namespace SecurityEssentials.ViewModel
{

    public class UserViewModel
    {

        #region Declarations

        public User User { get; set; }
        public bool IsOwnProfile { get; set; }

        #endregion

        #region Constructor

        public UserViewModel(int currentUserId, User user)
        {
            if (currentUserId == user.Id) IsOwnProfile = true; else IsOwnProfile = false;
            User = user;

        }

        #endregion

    }
}