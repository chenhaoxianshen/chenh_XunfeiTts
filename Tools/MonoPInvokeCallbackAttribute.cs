using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chenh_XunfeiTts.Tools
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute(Type type) { }
    }
}
