using System;

namespace Tests {
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using DataAccessLibrary.Models;

    public class Program {
        public static async Task Main(string[] args) {
            while (true) {
                try {
                    Console.WriteLine("Press Enter To Start");
                    Console.ReadLine();
                    using (HttpClient client = new HttpClient()) {
                        Uri uri = new Uri("https://localhost:5001/api/FlightPlans/1");
                        var response1 = await client.GetAsync(uri);
                        Console.WriteLine($"response: {response1}");
                        var content = await response1.Content.ReadAsStringAsync();
                        Console.WriteLine($"content: {content}");
                        var flightPlan1 = JsonSerializer.Deserialize<FlightPlan>(await response1.Content.ReadAsStringAsync());
                        var flightPlan2 = JsonSerializer.Deserialize<FlightPlan>(await response1.Content.ReadAsStringAsync());
                        Console.WriteLine($"{flightPlan1.GetHashCode()}={flightPlan2.GetHashCode()}");
                        Console.WriteLine(flightPlan1.GetHashCode() == flightPlan2.GetHashCode());
                        Console.ReadLine();
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine();
                    Console.WriteLine();
                    continue;
                }
            }
        }
    }
}
