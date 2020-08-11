using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Notifications;

namespace Listener.Models
{
    public class Address
    {
        public string StreetName { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public int Building { get; set; }
    }

    public class Driver
    {
        public string MobileNum { get; set; }
    }

    public class Chance
    {
        public string DateStart { get; set; }
    }

    public class ChanceReq
    {
        public Address Address { get; set; }
        public Driver Driver { get; set; }
        public Chance Chance { get; set; }
    }

    public class WaNotification
    {
        public uint Id { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string TitleText { get; set; }
        public string BodyText { get; set; }

        public WaNotification(UserNotification userNotification)
        {
            string titleText = "", bodyText = "";
            NotificationBinding toastBinding = userNotification.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);

            if (toastBinding != null)
            {
                IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();
                titleText = textElements.FirstOrDefault()?.Text;
                bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text));
            }

            this.Id = userNotification.Id;
            this.CreationTime = userNotification.CreationTime;
            this.Description = userNotification.AppInfo.DisplayInfo.Description;
            this.DisplayName = userNotification.AppInfo.DisplayInfo.DisplayName;
            this.TitleText = titleText;
            this.BodyText = bodyText;
        }
    }
}
