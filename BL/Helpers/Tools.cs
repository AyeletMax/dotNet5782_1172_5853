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
    /// <summary>
    /// Converts an object's properties to a formatted string representation.
    /// </summary>
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return "null";

        var result = new StringBuilder();

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(t);

            if (value is IEnumerable enumerable && !(value is string))
            {
                result.Append($"{property.Name}: [");

                foreach (var item in enumerable)
                {
                    result.Append($"{item}, ");
                }

                result.Append("], ");
            }
            else
            {
                result.Append($"{property.Name}: {value}, ");
            }
        }

        return result.ToString().TrimEnd(',', ' ');
    }
    /// <summary>
    /// Calculates the status of an assignment based on its timing and risk threshold.
    /// </summary>
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

    /// <summary>
    /// Sends an email using SMTP with the specified recipient, subject, and body.
    /// </summary>
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
    /// <summary>
    /// Retrieves latitude and longitude coordinates for a given address using an API.
    /// </summary>
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
    /// <summary>
    /// Calculates the distance between two locations based on the selected distance type.
    /// </summary>
    public static double CalculateDistance(DistanceType type, double latitudeV, double longitudeV, double latitudeC, double longitudeC)
    {
        return type switch
        {
            DistanceType.Air => HaversineDistance(latitudeV, longitudeV, latitudeC, longitudeC),
            DistanceType.Walk => GetRouteDistance(latitudeV, longitudeV, latitudeC, longitudeC, "pedestrian"),
            DistanceType.Drive => GetRouteDistance(latitudeV, longitudeV, latitudeC, longitudeC, "car"),
            _ => throw new ArgumentException("Invalid distance type", nameof(type))
        };
    }
    /// <summary>
    /// Retrieves the route distance between two locations using an external API.
    /// </summary>
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
                    return length.GetDouble() / 1000.0; 
            }

            return double.MaxValue;
        }
        catch
        {
            return double.MaxValue;
        }
    }

    /// <summary>
    /// Calculates the great-circle distance between two points using the Haversine formula.
    /// </summary>
    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        lat1 = DegreesToRadians(lat1);
        lon1 = DegreesToRadians(lon1);
        lat2 = DegreesToRadians(lat2);
        lon2 = DegreesToRadians(lon2);

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }
    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}