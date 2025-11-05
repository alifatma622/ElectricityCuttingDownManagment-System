using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Http.Json;

class Program
{
    private static readonly HttpClient _http = new HttpClient();
    private static string _connectionString = "Data Source=HABIBA\\SQLEXPRESS;Initial Catalog=Electricity_FTA;Integrated Security=True;Trust Server Certificate=True";
    private static string _apiUrl = "http://localhost:5032";
    private static int _cycleCount = 0;

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Electricity Incident Worker ===\n");

        _http.BaseAddress = new Uri(_apiUrl);
        _http.Timeout = TimeSpan.FromSeconds(30);

        // Test API connection first
        Console.WriteLine("Testing API connection...");
        try
        {
            var response = await _http.GetAsync("api/CuttingDownA/health");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ API connection successful");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ API returned error: {response.StatusCode}");
                Console.WriteLine("Make sure the API is running at: {_apiUrl}");
                Console.ResetColor();
                return;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Could not connect to API: {ex.Message}");
            Console.WriteLine($"Make sure the API is running at: {_apiUrl}");
            Console.WriteLine("\nTo start the API:");
            Console.WriteLine("1. Open another terminal");
            Console.WriteLine("2. Navigate to ElectricityCuttingDownManagment.SystemAPI directory");
            Console.WriteLine("3. Run: dotnet run");
            Console.ResetColor();
            return;
        }

        while (true)
        {
            _cycleCount++;
            Console.WriteLine($"\n━━━ Cycle #{_cycleCount} - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ━━━\n");

            try
            {
                // Phase 0: Build Hierarchy (first time only)
                if (_cycleCount == 1)
                {
                    await RunStoredProcedure("SP_BuildHierarchy", "Building hierarchy");
                    await Task.Delay(2000);
                }

                // Phase 1: Call API
                await CallApi("api/CuttingDownA/generate-test-data", "Source A (Cabins)", 10);
                await CallApi("api/CuttingDownB/generate-test-data", "Source B (Cables)", 10);

                // Phase 2: Execute SP_Create
                await RunStoredProcedure("SP_Create", "Creating incidents");

                // Phase 3: Execute SP_Close
                await RunStoredProcedure("SP_Close", "Closing incidents");

                Console.WriteLine("\n✅ Cycle completed successfully");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\n⏳ Waiting 10 seconds...");
            await Task.Delay(10000);
        }
    }

    static async Task CallApi(string endpoint, string name, int count)
    {
        try
        {
            var response = await _http.PostAsync($"{endpoint}?count={count}", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ [{name}] Success - {response.StatusCode}");
                Console.ResetColor();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠ [{name}] Failed - {response.StatusCode}: {error}");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ [{name}] Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    static async Task RunStoredProcedure(string spName, string description)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(spName, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandTimeout = 120;

            // Add return value parameter
            var returnParam = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;

            await cmd.ExecuteNonQueryAsync();

            // Get return value
            int returnValue = (int)(cmd.Parameters["@RETURN_VALUE"].Value ?? 0);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"✓ {description} - Records: {returnValue}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {spName} Error: {ex.Message}");
            Console.ResetColor();
        }
    }
}