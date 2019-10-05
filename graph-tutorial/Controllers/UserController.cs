using graph_tutorial.Helpers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var users = await GraphHelper.GetUsersAsync();

            return View(users);
        }
    }
}