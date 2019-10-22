using graph_tutorial.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    public class ExcelChartController : BaseController
    {
        // GET: Chart
        public async Task<ActionResult> Index()
        {
            var chart = await GraphHelper.GetChart();

            return View(chart);
        }
    }
}