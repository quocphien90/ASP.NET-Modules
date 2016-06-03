using OgilvyOne.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using umbraco.DataLayer;

namespace OgilvyOne.Report.DataHelper
{

    /// <summary>
    /// Map data from a source into a target object
    /// by copying public property values.
    /// </summary>
    /// <remarks></remarks>
    public static class DataMapper
    {
        /// <summary>
        /// Map data reader to objectMap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="objectMap"></param>
        public static void Mapper<T>(IDataReader reader, T objectMap)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetValue(i) != DBNull.Value)
                {
                    DataMapper.SetPropertyValue(objectMap, reader.GetName(i), reader.GetValue(i));
                }
            }
        }
        /// <summary>
        /// Map data reader to objectMap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="objectMap"></param>
        public static void Mapper<T>(IRecordsReader reader, T objectMap)
        {
            var type = typeof(T);
            var propreties = type.GetProperties();
            foreach (var proprety in propreties)
            {

                if (proprety.CanWrite && reader.ContainsField(proprety.Name))
                {
                    DataMapper.SetPropertyValue(objectMap, proprety, reader.GetObject(proprety.Name));
                }
            }
        }

        /// <summary>
        /// Sets an object's property with the specified value,
        /// coercing that value to the appropriate type if possible.
        /// </summary>
        /// <param name="target">Object containing the property to set.</param>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <param name="value">Value to set into the property.</param>
        public static void SetPropertyValue(
            object target, string propertyName, object value)
        {
            List<PropertyInfo> propertyInfos = GetPropertyByDataName(target, propertyName);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                SetPropertyValue(target, propertyInfo, value);
            }
        }

        /// <summary>
        /// Sets an object's property with the specified value,
        /// coercing that value to the appropriate type if possible.
        /// </summary>
        /// <param name="target">Object containing the property to set.</param>
        /// <param name="propertyInfo">The property to set.</param>
        /// <param name="value">Value to set into the property.</param>
        public static void SetPropertyValue(
            object target, PropertyInfo propertyInfo, object value)
        {
            if (propertyInfo.CanWrite == false)
                return;
            if (value == null)
                propertyInfo.SetValue(target, value, null);
            else
            {
                Type pType =
                    GetPropertyType(propertyInfo.PropertyType);
                Type vType =
                    GetPropertyType(value.GetType());
                if (pType.Equals(vType))
                {
                    // types match, just copy value
                    propertyInfo.SetValue(target, value, null);
                }
                else
                {
                    // types don't match, try to convert
                    if (pType.Equals(typeof(Guid)))
                        propertyInfo.SetValue(
                            target, new Guid(value.ToString()), null);
                    else if (pType.IsEnum)
                    {
                        propertyInfo.SetValue(target, Enum.Parse(pType, value.ToString()), null);
                    }
                    else if (pType.Equals(typeof(string)))
                        propertyInfo.SetValue(target, Convert.ToString(value), null);
                    else
                        propertyInfo.SetValue(
                            target, Convert.ChangeType(value, pType), null);
                }
            }
        }

        /// <summary>
        /// Gets an object's property with the specified value,
        /// coercing that value to the appropriate type if possible.
        /// </summary>
        /// <param name="target">Object containing the property to get.</param>
        /// <param name="propertyName">Name of the property to get.</param>
        /// <returns>Value to get into the property.</returns>
        public static object GetPropertyValue(object target, string propertyName)
        {
            Type type = target.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            return propertyInfo.GetValue(target, null);
        }



        private static List<PropertyInfo> GetPropertyByDataName(object target, string propertyName)
        {
            Type type = target.GetType();
            List<PropertyInfo> props = new List<PropertyInfo>(DataMapper.GetSourceProperties(type));
            return props.FindAll(delegate(PropertyInfo pi)
            {
                return DataMapper.MatchPropertyInfo(pi, propertyName);
            });
        }

        public static bool MatchPropertyInfo(PropertyInfo pi, string dataName)
        {
            DataFieldAttribute[] da = (DataFieldAttribute[])pi.GetCustomAttributes(typeof(DataFieldAttribute), true);
            if (da.Length == 0)
            {
                return pi.Name == dataName ? true : false;
            }
            else
            {
                if (string.IsNullOrEmpty(da[0].DataFieldName))
                {
                    return pi.Name == dataName ? true : false;
                }
                return da[0].DataFieldName == dataName ? true : false;
            }
        }

        public static PropertyInfo[] GetSourceProperties(Type sourceType)
        {
            //List<PropertyInfo> result = new List<PropertyInfo>();
            //PropertyDescriptorCollection props =
            //    TypeDescriptor.GetProperties(sourceType);
            //foreach (PropertyDescriptor item in props)
            //    if (item.IsBrowsable)
            //        result.Add(sourceType.GetProperty(item.Name));

            return sourceType.GetProperties();
        }

        /// <summary>
        /// Returns a property's type, dealing with
        /// Nullable(Of T) if necessary.
        /// </summary>
        /// <param name="propertyType">Type of the
        /// property as returned by reflection.</param>
        private static Type GetPropertyType(Type propertyType)
        {
            Type type = propertyType;
            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return Nullable.GetUnderlyingType(type);
            return type;
        }


    }

}