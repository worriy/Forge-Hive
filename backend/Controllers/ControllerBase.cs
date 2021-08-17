using Microsoft.AspNetCore.Mvc;

namespace Hive.Backend.Controllers
{
    public abstract partial class ControllerBase<T> : Controller where T : class
    {
		public ControllerBase()
        {
        }
    }
}
