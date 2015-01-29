using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace SecurityEssentials.ViewModel
{
    public class UsersViewModel
    {

        #region Declarations

		public int CurrentUserId { get; set; }

        #endregion

        #region Constructor

        public UsersViewModel(int currentUserId)            
        {
			CurrentUserId = currentUserId;
        }

        #endregion

    }
}