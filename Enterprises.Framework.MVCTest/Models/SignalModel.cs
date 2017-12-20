using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Enterprises.Framework.MVCTest.Models
{
    public class SignalModel
    {
    }

    public class FckTestViewModel
    {
        [DataType(DataType.MultilineText)]
        public string Contents { get; set; }
        public string OtherTest { get; set; }
    }
}