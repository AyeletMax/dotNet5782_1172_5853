using System;
using System.Collections;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    public static Status CalculateStatus(DO.Assignment assignment, DO.Call call, int riskThreshold = 30)
    {
        if (assignment.ExitTime == null)
        {
            if (call.MaxFinishTime.HasValue)
            {
                var remainingTime = call.MaxFinishTime.Value - DateTime.Now;

                if (remainingTime.TotalMinutes <= riskThreshold)
                {
                    return Status.InProgressAtRisk; 
                }
                else
                {
                    return Status.InProgress; 
                }
            }
            else
            {
                return Status.InProgress;
            }
        }
        return Status.Closed;
    }

    public static double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
    {
        var lat1Value = lat1 ?? 0; 
        var lon1Value = lon1 ?? 0; 

        var r = 6371;
        var dLat = (lat2 - lat1Value) * Math.PI / 180;
        var dLon = (lon2 - lon1Value) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1Value * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }


    //private static readonly string apiKey = "6797d44fa1ea4701946207wxvc2aa5e";
    private static readonly string apiKey = "pk.75af8008d03ff3161df4583252c484f2";
    private static readonly string apiUrl = "https://geocode.maps.co/search?q={0}&api_key={1}";
    /// <summary>
    /// Retrieves coordinates (latitude and longitude) for a given address.
    /// If the address is invalid or the API request fails, an appropriate exception is thrown.
    /// </summary>
    /// <param name="address">The address for which coordinates are requested.</param>
    /// <returns>A tuple containing latitude and longitude of the address.</returns>
    /// <exception cref="InvalidAddressException">Thrown when the address is invalid or cannot be processed.</exception>
    /// <exception cref="ApiRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="GeolocationNotFoundException">Thrown when no geolocation is found for the address.</exception>
    public static (double? Latitude, double? Longitude) GetCoordinatesFromAddress(string address)
    {
        
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new BlInvalidFormatException(address); // חריגה אם הכתובת לא תקינה
        }
        //Console.WriteLine("12 מרחק טעות");
        try
        {
            // יצירת ה-URL לפנייה ל-API
            string url = string.Format(apiUrl, Uri.EscapeDataString(address), apiKey);

            using (HttpClient client = new())
            {
                // בקשה סינכרונית ל-API
                HttpResponseMessage response = client.GetAsync(url).Result;

                // בדיקה אם הבקשה הצליחה
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    // ניתוח התשובה כ-JSON
                    JArray jsonArray = JArray.Parse(jsonResponse);

                    // אם יש תוצאות, מחזירים את הקואורדינטות
                    if (jsonArray.Count > 0)
                    {
                        var firstResult = jsonArray[0];
                        double latitude = (double)firstResult["lat"];
                        double longitude = (double)firstResult["lon"];
                        return (latitude, longitude);
                    }
                    else
                    {
                        throw new BlGeolocationNotFoundException(address); // חריגה אם לא נמצאה גיאוקולציה
                    }
                }
                else
                {
                    throw new BlApiRequestException($"API request failed with status code: {response.StatusCode}"); // חריגה אם הבקשה נכשלה
                }
            }
        }
        catch (Exception ex)
        {
            // אם קרתה שגיאה כלשהי, זורקים חריגה עם פרטי השגיאה
            throw new BlApiRequestException($"Error occurred while fetching coordinates for the address. {ex.Message}");
        }
    }
}



//private static readonly string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
//private static readonly string apiKey = "6797d44fa1ea4701946207wxvc2aa5e";

//private static readonly string apiUrl = "https://us1.locationiq.com/v1/search.php?key={0}&q={1}&format=json";

//public static (double? Latitude, double? Longitude) GetCoordinatesFromAddress(string? address = null)
//{
//    try
//    {
//        if (string.IsNullOrWhiteSpace(address))
//        {
//            return (null, null);

//        }

//        // Create URL for the API request
//        string url = string.Format(apiUrl, apiKey, Uri.EscapeDataString(address));

//        using (HttpClient client = new HttpClient())
//        {
//            // Make the synchronous API request
//            HttpResponseMessage response = client.GetAsync(url).Result;
//            // Check if the API request was successful
//            if (response.IsSuccessStatusCode)
//            {
//                string jsonResponse = response.Content.ReadAsStringAsync().Result;
//                // Parse the JSON response
//                JArray jsonArray = JArray.Parse(jsonResponse);
//                // If there are results, return the coordinates
//                if (jsonArray.Count > 0)
//                {
//                    var firstResult = jsonArray[0];
//                    double latitude = (double)firstResult["lat"];
//                    double longitude = (double)firstResult["lon"];
//                    return (latitude, longitude);
//                }
//                else
//                {
//                    throw new GeolocationNotFoundException(address);
//                }
//            }
//            else
//            {
//                throw new Exception(response.StatusCode.ToString());
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        throw new Exception("Error occurred while fetching coordinates for the address. ", ex);
//    }

//}
