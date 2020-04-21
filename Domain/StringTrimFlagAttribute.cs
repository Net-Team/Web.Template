using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringTrimFlagAttribute : Attribute
    {
    }
}
