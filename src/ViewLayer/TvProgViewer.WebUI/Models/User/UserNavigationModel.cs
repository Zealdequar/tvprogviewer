﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record UserNavigationModel : BaseTvProgModel
    {
        public UserNavigationModel()
        {
            UserNavigationItems = new List<UserNavigationItemModel>();
        }

        public IList<UserNavigationItemModel> UserNavigationItems { get; set; }

        public int SelectedTab { get; set; }
    }

    public partial record UserNavigationItemModel : BaseTvProgModel
    {
        public string RouteName { get; set; }
        public string Title { get; set; }
        public int Tab { get; set; }
        public string ItemClass { get; set; }
    }

    public enum UserNavigationEnum
    {
        Info = 0,
        Addresses = 10,
        Orders = 20,
        BackInStockSubscriptions = 30,
        ReturnRequests = 40,
        DownloadableTvChannels = 50,
        RewardPoints = 60,
        ChangePassword = 70,
        Avatar = 80,
        ForumSubscriptions = 90,
        TvChannelReviews = 100,
        VendorInfo = 110,
        GdprTools = 120,
        CheckGiftCardBalance = 130,
        MultiFactorAuthentication = 140
    }
}