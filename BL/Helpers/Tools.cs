using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Helpers;

internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return "null";

        // StringBuilder לאיחסון התוצאה
        var result = new StringBuilder();

        // קבלת כל התכונות של הטיפוס T בעזרת Reflection
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // קבלת ערך התכונה בעזרת Reflection
            var value = property.GetValue(t);

            // אם התכונה היא אוסף או רשימה, נבצע חקירה לעומק
            if (value is IEnumerable enumerable && !(value is string))
            {
                result.Append($"{property.Name}: [");

                foreach (var item in enumerable)
                {
                    result.Append($"{item}, ");
                }

                // מסיימים את האוסף עם סוגריים
                result.Append("], ");
            }
            else
            {
                result.Append($"{property.Name}: {value}, ");
            }
        }

        // מחזירים את המחרוזת לאחר הסרת פסיקים מיותרים בסוף
        return result.ToString().TrimEnd(',', ' ');
    }
}
