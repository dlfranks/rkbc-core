using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkbcMobile.Views
{

    public class TestPageMasterMenuItem
    {
        public TestPageMasterMenuItem()
        {
            TargetType = typeof(TestPageMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}