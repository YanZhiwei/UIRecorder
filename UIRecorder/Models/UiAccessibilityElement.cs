using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;

namespace UIRecorder.Models
{
    internal class UiAccessibilityElement
    {
        public AutomationElement[] Child { get; set; }

        public string Name { get; set; }
    }
}
