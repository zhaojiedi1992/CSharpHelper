using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CSharpHelper
{
    public static class ListHelper
    {
        public static DataSet ToDataSet<T>(this List<T> list) where T : class, new()
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            if (list.Count > 0)
            {
                T t = list[0];
                PropertyInfo[] properties = t.GetType().GetProperties();
                PropertyInfo[] array = properties;
                for (int i = 0; i < array.Length; i++)
                {
                    PropertyInfo propertyInfo = array[i];
                    dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
                }
                for (int j = 0; j < list.Count; j++)
                {
                    ArrayList arrayList = new ArrayList();
                    array = properties;
                    for (int i = 0; i < array.Length; i++)
                    {
                        PropertyInfo propertyInfo = array[i];
                        object value = propertyInfo.GetValue(list[j], null);
                        arrayList.Add(value);
                    }
                    object[] values = arrayList.ToArray();
                    dataTable.LoadDataRow(values, true);
                }
            }
            dataSet.Tables.Add(dataTable);
            return dataSet;
        }
    }
}
