using Hive.Backend.Models.Helpers;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class NotificationService: INotificationService
    {
        private readonly NotificationHubClient hub = null;

        public NotificationService()
        {
            if (hub == null)
            {
                hub = NotificationHubClient
                    .CreateClientFromConnectionString(
                 "Endpoint=sb://hivenamepace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=urVXpet7q6hstwN28RmHQyeIlkh30m4dZyhgpkYEQ1c=",
                 "Hive"
                 );

            }
        }

        public async Task RegisterTags(string installationId, string registrationId, IEnumerable<string> tags, string platform)
        {

            Installation installation = new Installation
            {
                InstallationId = installationId,
                PushChannel = registrationId,
                Tags = tags.ToList<string>()
            };
            switch (platform.ToLower())
            {
                case "android":
                    installation.Platform = NotificationPlatform.Fcm;
                    break;
                case "ios":
                    installation.Platform = NotificationPlatform.Apns;
                    break;
                default:
                    Console.WriteLine("Invalid Platform for notifications, skipping Tags registering");
                    return;
            }

            await hub.CreateOrUpdateInstallationAsync(installation);

        }

        public async Task Unsubscribe(string installationId)
        {
            await hub.DeleteInstallationAsync(installationId);
        }

        public async Task SendNotification(List<Guid> notifiers, string text, PushActionTypes action, Guid idEntity = new Guid())
        {
            string notificationAPNS = "{\"aps\":{\"alert\":\"" + text + "\",\"action\":\"" + action.ToString() + "\""; //}}";
            //string notificationWNS = "";
            string notificationFCM = "{\"data\":{\"message\":\"" + text + "\",\"action\":\"" + action.ToString() + "\"";
            if (idEntity == Guid.Empty)
            {
                notificationAPNS += ",\"id\":" + idEntity.ToString();
                //notificationWNS += "";
                notificationFCM += ",\"id\":" + idEntity.ToString();
            }

            notificationAPNS += "}}";
            //notificationWNS += "";
            notificationFCM += "}}";
            if (notifiers == null)
            {
                await hub.SendAppleNativeNotificationAsync(notificationAPNS);
                //await hub.SendWindowsNativeNotificationAsync(notificationWNS);
                await hub.SendFcmNativeNotificationAsync(notificationFCM);
            }
            else
            {
                foreach (Guid notifier in notifiers)
                {
                    await hub.SendAppleNativeNotificationAsync(notificationAPNS, notifier.ToString());
                    //await hub.SendWindowsNativeNotificationAsync(notificationWNS, notifier.ToString());
                    await hub.SendFcmNativeNotificationAsync(notificationFCM, notifier.ToString());
                }
            }
        }

        public async Task NotifyForResults(string idUser, ICardService _cardService)
        {
            var cards = _cardService.GetAll()
                .Where(card => card.CreatedBy.UserId == idUser)
                .Where(card => card.EndDate < DateTime.Now && card.EndDate.Date > DateTime.Now.AddDays(-2))
                .Select(card => new { userId = card.CreatedById, cardId = card.Id, cardContent = card.Content });

            if (cards.Any())
            {
                var notifiers = new List<Guid> { cards.FirstOrDefault().userId };
                string notificationMessage = "";
                PushActionTypes action = PushActionTypes.post_details;
                var idEntity = Guid.Empty;
                if (cards.Count() > 1)
                {
                    notificationMessage += "Some ideas has reached their end date, check the results!";
                    action = PushActionTypes.post_list;
                }
                else
                {
                    notificationMessage = "The idea '" + cards.First().cardContent + "' Has reached its end date, check the results!";
                    action = PushActionTypes.post_details;
                    idEntity = cards.First().cardId;
                }

                //await this.SendNotification(null, notificationMessage, action);

            }
        }
    }
}
