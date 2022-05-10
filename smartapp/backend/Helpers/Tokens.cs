using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hive.Backend.Auth;
using Hive.Backend.Models;
using Newtonsoft.Json;

namespace Hive.Backend.Helpers
{
    public class Tokens
    {
      public static async Task<string> GenerateJwt(Guid userProfileId, ClaimsIdentity identity, IJwtFactory jwtFactory,string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
      {
        var response = new
        {
          //id = identity.Claims.Single(c => c.Type == "id").Value,
          id = userProfileId,
          auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
          expires_in = (int)jwtOptions.ValidFor.TotalSeconds
        };

        return JsonConvert.SerializeObject(response, serializerSettings);
      }
    }
}
