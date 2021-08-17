using Hive.Backend.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface INotificationService
    {
        Task SendNotification(List<Guid> notifiers, string text, PushActionTypes action, Guid idEntity = new Guid());
        Task RegisterTags(string installationId, string registrationId, IEnumerable<string> tags, string platform);
        Task Unsubscribe(string installationId);
        Task NotifyForResults(string idUser, ICardService _cardService);
    }
}
