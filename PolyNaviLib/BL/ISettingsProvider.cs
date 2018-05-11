using System;
using System.Collections.Generic;
using System.Text;

namespace PolyNaviLib.BL
{
    public interface ISettingsProvider
    {
		object this[string key] { get; set; }
    }
}
