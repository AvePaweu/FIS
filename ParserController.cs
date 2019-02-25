using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Net.Http;
using System.Globalization;

namespace ResultsParser.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ParserController : ControllerBase
  {
    [HttpGet("{raceId}/{type}")]
    public Task<List<Result>> GetData(string raceId, int type)
    {
      HtmlWeb web = new HtmlWeb();
      var htmlDoc = web.Load($"https://www.fis-ski.com/DB/general/results.html?sectorcode=JP&raceid={raceId}");
      var results = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='events-info-results']/div");
      var items = results.Elements("a");
      string competitionDate = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='content']/div[1]/div/div[1]/div/div/div/div/div/div[2]/time/span[1]")?.InnerText;
      DateTime.TryParseExact(competitionDate, "MMMM dd, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate);
      // System.Console.WriteLine(parsedDate.ToString("yyyy-MM-dd"));

      switch (type)
      {
        case 1: return ParseDataFor2RoundsCompetition(items, parsedDate);
        case 2: return ParseDataFor1RoundCompetition(items, parsedDate);
        case 3: return ParseDataForTeam2RoundsCompetition(items, parsedDate);
        case 4: return ParseDataForTeam1RoundCompetition(items, parsedDate);
        default: return ParseDataFor2RoundsCompetition(items, parsedDate);
      }

    }

    private Task<List<Result>> ParseDataForTeam1RoundCompetition(IEnumerable<HtmlNode> items, DateTime competitionDate)
    {
      throw new NotImplementedException();
    }

    private async Task<List<Result>> ParseDataForTeam2RoundsCompetition(IEnumerable<HtmlNode> items, DateTime competitionDate)
    {
      HttpClient client = new HttpClient();
            var response = new List<Result>();
            string currentNation = "";
            foreach (var item in items)
            {
                if (item.HasClass("table-row_theme_main"))
                {
                    currentNation = item.ChildNodes["div"].ChildNodes["div"].Elements("div").ToArray()[2]?.InnerText.Trim();
                }
                else if (item.HasClass("table-row_theme_additional"))
                {
                    var dataRow = item.ChildNodes["div"].ChildNodes["div"].Elements("div").ToList();
                    var result = new Result
                    {
                        Nation = currentNation,
                        Jumper = dataRow[2]?.InnerText.Trim() ?? "gówno",
                        FISCode = dataRow[1]?.InnerText ?? "gówno",
                        Jump1 = dataRow[6]?.InnerText.Replace(".", ",") ?? "0",
                        Jump2 = dataRow[8]?.InnerText.Replace(".", ",") ?? "0",
                    };
                    result.Sum = Convert.ToDouble(result.Jump1) + Convert.ToDouble(result.Jump2);

                    response.Add(result);
                }
                else continue;
            }
            response = response.OrderByDescending(a => a.Sum).ToList();
            foreach (var item in response)
            {
                item.Place = Convert.ToByte(response.Select(a => a.Sum).ToList().IndexOf(item.Sum) + 1);
                if (item.Place <= 5)
                {
                    string fisPoints = await client.GetStringAsync($"http://localhost:88/FIS/api.php/jumper/{item.FISCode}/{competitionDate.ToString("yyyy-MM-dd")}");
                    item.FISPoints = fisPoints.Replace(".", ",");
                }
            }

            return response;
    }

    private async Task<List<Result>> ParseDataFor1RoundCompetition(IEnumerable<HtmlNode> items, DateTime competitionDate)
    {
      HttpClient client = new HttpClient();
      var response = new List<Result>();
      foreach (var item in items)
      {
        var dataRow = item.ChildNodes["div"].ChildNodes["div"].Elements("div").ToList();
        var result = new Result
        {
          Place = Convert.ToByte(dataRow[0]?.InnerText ?? "0"),
          Jumper = dataRow[3]?.InnerText.Trim() ?? "gówno",
          FISCode = dataRow[2]?.InnerText ?? "gówno",
          Jump1 = dataRow[7]?.InnerText.Replace(".", ",") ?? "gówno",
          Sum = Convert.ToDouble(dataRow[8]?.InnerText.Replace(".", ",") ?? "0")
        };
        if (result.Place <= 5)
        {
          string FISPoints = await client.GetStringAsync($"http://localhost:88/FIS/api.php/jumper/{result.FISCode}/{competitionDate.ToString("yyyy-MM-dd")}");
          result.FISPoints = FISPoints.Replace(".", ",");
        }
        response.Add(result);
      }

      return response;
    }

    private async Task<List<Result>> ParseDataFor2RoundsCompetition(IEnumerable<HtmlNode> items, DateTime competitionDate)
    {
      HttpClient client = new HttpClient();
      var response = new List<Result>();
      foreach (var item in items)
      {
        var dataRow = item.ChildNodes["div"].ChildNodes["div"].Elements("div").ToList();
        var result = new Result
        {
          Place = Convert.ToByte(dataRow[0]?.InnerText ?? "0"),
          Jumper = dataRow[3]?.InnerText.Trim() ?? "gówno",
          FISCode = dataRow[2]?.InnerText ?? "gówno",
          Jump1 = dataRow[7]?.InnerText.Replace(".", ",") ?? "gówno",
          Jump2 = dataRow[9]?.InnerText.Replace(".", ",") ?? "gówno",
          Sum = Convert.ToDouble(dataRow[10]?.InnerText.Replace(".", ",") ?? "0")
        };
        if (result.Place <= 5)
        {
          string FISPoints = await client.GetStringAsync($"http://localhost:88/FIS/api.php/jumper/{result.FISCode}/{competitionDate.ToString("yyyy-MM-dd")}");
          result.FISPoints = FISPoints.Replace(".", ",");
        }
        response.Add(result);
      }

      return response;
    }

    public class Result
    {
      public byte Place { get; set; }
      public string Jumper { get; set; }
      public string Nation { get; set; }
      public string FISCode { get; set; }
      public string Jump1 { get; set; }
      public string Jump2 { get; set; }
      public double Sum { get; set; }
      public string FISPoints { get; set; }

      public override string ToString() => $"{Jumper} ({FISCode}) {Jump1} {Jump2} {Sum}";
    }
  }
}