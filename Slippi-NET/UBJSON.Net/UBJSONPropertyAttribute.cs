using System;

namespace UBJSON.Net
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UbjsonPropertyAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
