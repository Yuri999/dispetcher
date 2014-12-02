using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispetcher.Common.IoC
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ComponentInterfaceAttribute: Attribute
    {
        public ComponentLifeTime LifeTime { get; set; } 
    }
}
