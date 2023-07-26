using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace CurrencyConverter.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly HttpClient _httpClient;

        public CurrencyController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                string apiUrl = "https://www.cbar.az/currencies/17.07.2023.xml";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string xmlData = await response.Content.ReadAsStringAsync();
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlData);

                    XmlNode usdNode = xmlDoc.SelectSingleNode("/ValCurs/ValType[Type='Xarici valyutalar'][1]/Valute[@Code='USD']");
                    XmlNode eurNode = xmlDoc.SelectSingleNode("/ValCurs/ValType[Type='Xarici valyutalar'][1]/Valute[@Code='EUR']");

                    string usdRate = usdNode.SelectSingleNode("Value")?.InnerText;
                    string eurRate = eurNode.SelectSingleNode("Value")?.InnerText;

                    ViewData["USD"] = usdRate;
                    ViewData["EUR"] = eurRate;
                }
                else
                {
                    ViewData["Error"] = "Произошла ошибка при получении данных о курсах валют.";
                }
            }
            catch (Exception ex)
            {
                ViewData["Error"] = $"Произошла ошибка: {ex.Message}";
            }

            return View();
        }
    }
}