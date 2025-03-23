using System.Collections;
using System.Reflection;
using System.Text;
using BO;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using DalApi;
using System.Text.Json;

namespace Helpers;

internal static class Tools
{
    public static double? DegreesToRadians(double? degrees)
    {
        return degrees is not null ? degrees * Math.PI / 180 : null;
    }
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

    //public static double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
    //{
    //    var lat1Value = lat1 ?? 0;
    //    var lon1Value = lon1 ?? 0;

    //    var r = 6371;
    //    var dLat = (lat2 - lat1Value) * Math.PI / 180;
    //    var dLon = (lon2 - lon1Value) * Math.PI / 180;
    //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
    //            Math.Cos(lat1Value * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
    //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
    //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    //    return r * c;
    //}
    public static double CalculateDistance(double? latitudeV, double? longitudeV, double latitudeC, double longitudeC, BO.DistanceType mode = BO.DistanceType.Air)
    {


        if (mode != BO.DistanceType.Air)
        {
            using (HttpClient client = new HttpClient())
            {
                // הכנת המידע לשאילתה ב-JSON
                var requestData = new
                {
                    coordinates = new[]
                    {
                        new double[] { (double)longitudeV!, (double)latitudeV! }, // תחילת מסלול
                        new double[] { longitudeC, latitudeC }  // סוף מסלול
                    }
                };

                // הגדרת כותרות הבקשה
                client.DefaultRequestHeaders.Add("Authorization", apiKey);
                string? modedistance = null;
                switch (mode)
                {
                    case BO.DistanceType.Drive:
                        modedistance = "driving-car";
                        break;
                    case BO.DistanceType.Walk:
                        modedistance = "walking";
                        break;


                }
                string url = string.Format(apiUrl, modedistance);


                HttpResponseMessage response = client.PostAsync(url,
                 new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();


                // קבלת התשובה
                string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent)!;


                // קבלת המרחק במטרים
                return jsonResponse.routes[0].segments[0].distance;

            }
        }
        else
        {
            const double EarthRadiusKm = 6371;

            double latVRad = (double)DegreesToRadians(latitudeV);
            double lonVRad = (double)DegreesToRadians(longitudeV);
            double latCRad = (double)DegreesToRadians(latitudeC);
            double lonCRad = (double)DegreesToRadians(longitudeC);

            double deltaLat = latCRad - latVRad;
            double deltaLon = lonCRad - lonVRad;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(latVRad) * Math.Cos(latCRad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = EarthRadiusKm * c;
            return distance;
        }
    }


    ////private static readonly string apiKey = "6797d44fa1ea4701946207wxvc2aa5e";
    //private static readonly string apiKey = "pk.75af8008d03ff3161df4583252c484f2";
    private static readonly string apiUrl = "https://geocode.maps.co/search?q={0}&api_key={1}";
    ///// <summary>
    ///// Retrieves coordinates (latitude and longitude) for a given address.
    ///// If the address is invalid or the API request fails, an appropriate exception is thrown.
    ///// </summary>
    ///// <param name="address">The address for which coordinates are requested.</param>
    ///// <returns>A tuple containing latitude and longitude of the address.</returns>
    ///// <exception cref="InvalidAddressException">Thrown when the address is invalid or cannot be processed.</exception>
    ///// <exception cref="ApiRequestException">Thrown when the API request fails.</exception>
    ///// <exception cref="GeolocationNotFoundException">Thrown when no geolocation is found for the address.</exception>
    //public static (double? Latitude, double? Longitude) GetCoordinatesFromAddress(string address)
    //{

    //    if (string.IsNullOrWhiteSpace(address))
    //    {
    //        throw new BlInvalidFormatException(address); // חריגה אם הכתובת לא תקינה
    //    }
    //    //Console.WriteLine("12 מרחק טעות");
    //    try
    //    {
    //        // יצירת ה-URL לפנייה ל-API
    //        string url = string.Format(apiUrl, Uri.EscapeDataString(address), apiKey);

    //        using (HttpClient client = new())
    //        {
    //            // בקשה סינכרונית ל-API
    //            HttpResponseMessage response = client.GetAsync(url).Result;

    //            // בדיקה אם הבקשה הצליחה
    //            if (response.IsSuccessStatusCode)
    //            {
    //                string jsonResponse = response.Content.ReadAsStringAsync().Result;

    //                // ניתוח התשובה כ-JSON
    //                JArray jsonArray = JArray.Parse(jsonResponse);

    //                // אם יש תוצאות, מחזירים את הקואורדינטות
    //                if (jsonArray.Count > 0)
    //                {
    //                    var firstResult = jsonArray[0];
    //                    double latitude = (double)firstResult["lat"];
    //                    double longitude = (double)firstResult["lon"];
    //                    return (latitude, longitude);
    //                }
    //                else
    //                {
    //                    throw new BlGeolocationNotFoundException(address); // חריגה אם לא נמצאה גיאוקולציה
    //                }
    //            }
    //            else
    //            {
    //                throw new BlApiRequestException($"API request failed with status code: {response.StatusCode}"); // חריגה אם הבקשה נכשלה
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // אם קרתה שגיאה כלשהי, זורקים חריגה עם פרטי השגיאה
    //        throw new BlApiRequestException($"Error occurred while fetching coordinates for the address. {ex.Message}");
    //    }
    //}
    public static void SendEmail(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress("makethemhappy979@gmail.com", "Make Them Happy");
        var toAddress = new MailAddress(toEmail);

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("makethemhappy979@gmail.com", "046150"),
            EnableSsl = true,
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
        })
        {
            smtpClient.Send(message);
        }
    }

    private static string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";


    public static (double, double) GetCoordinatesFromAddress(string address)
    {
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        var response = client.GetAsync(url).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid address or API error.");

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            throw new Exception("Address not found.");

        var root = doc.RootElement[0];

        if (!root.TryGetProperty("lat", out var latProperty) ||
            !root.TryGetProperty("lon", out var lonProperty))
        {
            throw new Exception("Missing latitude or longitude in response.");
        }

        if (!double.TryParse(latProperty.GetString(), out double latitude) ||
            !double.TryParse(lonProperty.GetString(), out double longitude))
        {
            throw new Exception("Invalid latitude or longitude format.");
        }

        return (latitude, longitude);
    }


}


