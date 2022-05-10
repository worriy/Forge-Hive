
using System.Collections.Generic;
using System.Linq;

namespace Hive.Backend.Security
{
    public class RoleDefinition
    {
        public RoleDefinition(string key, int value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; private set; }
        public int Value { get; private set; }
    }

    public class Roles
    {
        public const string RolesValueClaimType = "RolesValue";
				
		public class BuiltinRoles
		{
			public const string DataEditorKey = "DataEditor";
			public const string AdminKey = "Admin";
			public const string UserKey = "User";

			public static readonly RoleDefinition Admin = new RoleDefinition(AdminKey, 1);
			public static readonly RoleDefinition User = new RoleDefinition(UserKey, 2);
			public static readonly RoleDefinition DataEditor = new RoleDefinition(DataEditorKey, 4);

			public static IEnumerable<RoleDefinition> GetAll()
			{
				yield return BuiltinRoles.Admin;
				yield return BuiltinRoles.User;
				yield return BuiltinRoles.DataEditor;
			}
	    }

        public static IEnumerable<RoleDefinition> GetAll()
        {
            foreach (var builtInRole in Roles.BuiltinRoles.GetAll())
                yield return builtInRole;

			            
        }

        public static int GetRolesValue(IEnumerable<string> roleNames)
        {
            return Roles.GetAll()
                 .Where(role => roleNames.Any(r => r.Equals(role.Key, System.StringComparison.OrdinalIgnoreCase)))
                 .Select(r => r.Value)
                 .Sum();
    }
}
}