using System;
using System.Resources;

namespace OgilvyOne.Common.Extensions
{
    public static class ResourceExtension
    {
        public static string GetEnumString(this ResourceManager resourceManager, Enum enumValue)
        {
            return resourceManager.GetString(string.Format("{0}.{1}", enumValue.GetType().Name, enumValue.ToString()));
        }
    }
}
