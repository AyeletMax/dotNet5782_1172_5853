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

    
    public static void SendEmail(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress("makethemhappy979@gmail.com", "Make Them Happy");
        var toAddress = new MailAddress(toEmail);

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("makethemhappy979@gmail.com", "qrkf psyd jjzs gsmq"),
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

    private static readonly string apiUrl = "https://geocode.maps.co/search?q={0}&api_key={1}";

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

   

    private static readonly string apiKey1 = "vaUo0LbTQF27M9LVCg8w2b35GKIAJJyl";

    public static (BO.DistanceType Type, double Distance) CalculateDistance(double latitudeV, double longitudeV, double latitudeC, double longitudeC)
    {
        double airDistance = HaversineDistance(latitudeV, longitudeV, latitudeC, longitudeC);

        double driveDistance = GetRouteDistance(latitudeV, longitudeV, latitudeC, longitudeC, "car");
        double walkDistance = GetRouteDistance(latitudeV, longitudeV, latitudeC, longitudeC, "pedestrian");

        // קביעת סוג המרחק המתאים ביותר
        if (driveDistance < airDistance && driveDistance <= walkDistance)
            return (BO.DistanceType.Drive, driveDistance);

        if (walkDistance < airDistance && walkDistance <= driveDistance)
            return (BO.DistanceType.Walk, walkDistance);
        
        return (BO.DistanceType.Air, airDistance);
    }

    private static double GetRouteDistance(double latitudeV, double longitudeV, double latitudeC, double longitudeC, string travelMode)
    {
        using HttpClient client = new HttpClient();
        string url = $"https://api.tomtom.com/routing/1/calculateRoute/{latitudeV},{longitudeV}:{latitudeC},{longitudeC}/json?key={apiKey1}&travelMode={travelMode}";

        try
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
                return double.MaxValue;

            string responseContent = response.Content.ReadAsStringAsync().Result;
            using JsonDocument doc = JsonDocument.Parse(responseContent);

            if (doc.RootElement.TryGetProperty("routes", out var routes) && routes.GetArrayLength() > 0)
            {
                var route = routes[0];
                if (route.TryGetProperty("summary", out var summary) && summary.TryGetProperty("lengthInMeters", out var length))
                    return length.GetDouble() / 1000.0; // המרה לקילומטרים
            }

            return double.MaxValue;
        }
        catch
        {
            return double.MaxValue;
        }
    }

    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

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




////private static readonly string apiKey = "6797d44fa1ea4701946207wxvc2aa5e";
//private static readonly string apiKey = "pk.75af8008d03ff3161df4583252c484f2";
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


//private static readonly string apiKey1 = "vaUo0LbTQF27M9LVCg8w2b35GKIAJJyl";

//public static double CalculateDistance(double latitudeV, double longitudeV, double latitudeC, double longitudeC, BO.DistanceType mode = BO.DistanceType.Air)
//{
//    if (mode == BO.DistanceType.Air)
//        return HaversineDistance(latitudeV, longitudeV, latitudeC, longitudeC);

//    using HttpClient client = new HttpClient();
//    string travelMode = mode == BO.DistanceType.Drive ? "car" : "pedestrian";
//    string url = $"https://api.tomtom.com/routing/1/calculateRoute/{latitudeV},{longitudeV}:{latitudeC},{longitudeC}/json?key={apiKey1}&travelMode={travelMode}";

//    try
//    {
//        HttpResponseMessage response = client.GetAsync(url).Result; // קריאה סינכרונית
//        if (!response.IsSuccessStatusCode)
//        {
//            Console.WriteLine($"API request failed: {response.StatusCode}");
//            return double.MaxValue; // להחזיר ערך גדול במקרה של כשל
//        }

//        string responseContent = response.Content.ReadAsStringAsync().Result;
//        using JsonDocument doc = JsonDocument.Parse(responseContent);

//        if (doc.RootElement.TryGetProperty("routes", out var routes) && routes.GetArrayLength() > 0)
//        {
//            var route = routes[0];
//            if (route.TryGetProperty("summary", out var summary) && summary.TryGetProperty("lengthInMeters", out var length))
//            {
//                return length.GetDouble() / 1000.0; // להמיר לקילומטרים
//            }
//        }

//        return double.MaxValue; // אם לא נמצא מידע, להחזיר ערך גדול
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Error fetching distance: {ex.Message}");
//        return double.MaxValue;
//    }
//}

//private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
//{
//    const double R = 6371; // רדיוס כדור הארץ בק"מ
//    double dLat = DegreesToRadians(lat2 - lat1);
//    double dLon = DegreesToRadians(lon2 - lon1);
//    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
//               Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
//               Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
//    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
//    return R * c;
//}

//private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;