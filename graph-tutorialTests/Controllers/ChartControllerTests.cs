using Microsoft.VisualStudio.TestTools.UnitTesting;
using graph_tutorial.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graph_tutorial.Controllers.Tests
{
    [TestClass()]
    public class ChartControllerTests
    {
        [TestMethod()]
        public void IndexTest()
        {
            ChartController chart = new ChartController();
            chart.Index();
            
        }
    }
}