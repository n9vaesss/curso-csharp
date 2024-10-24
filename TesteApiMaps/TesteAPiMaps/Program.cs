using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class HelloWorld
{
    static async Task Main(string[] args)
    {
        string googleApiKey = "AIzaSyC3gsaIQeYaKMn8L-_77z6VTNKnktE6lEg";

        var calculator = new DistanceCalculator(googleApiKey);

        Console.WriteLine("Digite um endereço de origem (ou uma parte dele) para obter sugestões:");
        var originInfo = await GetAddressFromUserAsync(calculator);

        Console.WriteLine("Digite um endereço de destino (ou uma parte dele) para obter sugestões:");
        var destinationInfo = await GetAddressFromUserAsync(calculator);

        double distance = await calculator.GetDistanceBetweenCoordinatesAsync(originInfo.Latitude, originInfo.Longitude, destinationInfo.Latitude, destinationInfo.Longitude);

        if (distance >= 0)
        {
            Console.WriteLine($"A distância entre os endereços é de {distance} km.");
        }
        else
        {
            Console.WriteLine("Não foi possível calcular a distância.");
        }
    }

    static async Task<AddressInfo> GetAddressFromUserAsync(DistanceCalculator calculator)
    {
        string input;
        AddressInfo selectedAddressInfo = null;

        while (selectedAddressInfo == null)
        {
            input = Console.ReadLine();

            var suggestions = await calculator.GetAddressSuggestionsAsync(input);
            if (suggestions.Length > 0)
            {
                Console.WriteLine("Sugestões de endereços:");
                for (int i = 0; i < suggestions.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {suggestions[i].Address}");
                }

                Console.WriteLine("Selecione um endereço digitando o número correspondente ou continue digitando:");
                string choice = Console.ReadLine();

                if (int.TryParse(choice, out int index) && index > 0 && index <= suggestions.Length)
                {
                    selectedAddressInfo = suggestions[index - 1];
                }
                else
                {
                    Console.WriteLine("Digite mais detalhes do endereço ou escolha um número válido.");
                }
            }
            else
            {
                Console.WriteLine("Nenhuma sugestão encontrada. Digite mais detalhes do endereço:");
            }
        }

        return selectedAddressInfo;
    }
}

public class AddressInfo
{
    public string Address { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}

public class DistanceCalculator
{
    private static readonly HttpClient client = new HttpClient();
    private readonly string apiKey;

    public DistanceCalculator(string googleApiKey)
    {
        apiKey = googleApiKey;
    }

    public async Task<AddressInfo[]> GetAddressSuggestionsAsync(string input)
    {
        try
        {
            string url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(input)}&types=address&key={apiKey}";

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            JObject data = JObject.Parse(jsonResponse);

            var predictions = data["predictions"];
            if (predictions != null && predictions.HasValues)
            {

                var suggestions = new AddressInfo[predictions.Count()];
                for (int i = 0; i < predictions.Count(); i++)
                {
                    string description = predictions[i]["description"].ToString();
                    string placeId = predictions[i]["place_id"].ToString();

                    var location = await GetLocationFromPlaceIdAsync(placeId);
                    suggestions[i] = new AddressInfo
                    {
                        Address = description,
                        Latitude = location.Latitude.ToString(),
                        Longitude = location.Longitude.ToString()
                    };
                }

                return suggestions;
            }

            return new AddressInfo[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter sugestões de endereços: {ex.Message}");
            return new AddressInfo[0];
        }
    }

    private async Task<(string Latitude, string Longitude)> GetLocationFromPlaceIdAsync(string placeId)
    {
        try
        {

            string url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&key={apiKey}";

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(jsonResponse);

            var location = data["result"]["geometry"]["location"];
            string latitude = (string)location["lat"];
            string longitude = (string)location["lng"];

            return (latitude, longitude);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter a localização: {ex.Message}");
            return ("", "");
        }
    }

    public async Task<double> GetDistanceBetweenCoordinatesAsync(string originLat, string originLng, string destinationLat, string destinationLng)
    {
        try
        {
            Console.WriteLine(destinationLng);
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={originLat},{originLng}&destinations={destinationLat},{destinationLng.ToString()}&mode=driving&avoid=tolls&key={apiKey}";

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            JObject data = JObject.Parse(jsonResponse);

            var distanceElement = data["rows"]?[0]?["elements"]?[0];
            if (distanceElement?["status"]?.ToString() == "OK")
            {
                double distanceInMeters = (double)distanceElement["distance"]?["value"];
                return distanceInMeters / 1000.0;
            }

            throw new Exception("Não foi possível calcular a distância entre os endereços fornecidos.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            return -1;
        }
    }
}
