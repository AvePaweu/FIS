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

    private Task<List<Result>> ParseDataForTeam2RoundsCompetition(IEnumerable<HtmlNode> items, DateTime competitionDate)
    {
      throw new NotImplementedException();
    }

    private async Task<List<Result>> ParseDataFor1RoundCompetition(IEnumerable<HtmlNode> items, DateTime competitionDate)
    {
      HttpClient client = new HttpClient();
      var response = new List<Result>();
      foreach (var item in items)
      {
        var costam = item.ChildNodes["div"].ChildNodes["div"].Elements("div").ToList();
        var result = new Result
        {
          Place = Convert.ToByte(costam[0]?.InnerText ?? "0"),
          Jumper = costam[3]?.InnerText.Trim() ?? "gówno",
          FISCode = costam[2]?.InnerText ?? "gówno",
          Jump1 = costam[7]?.InnerText.Replace(".", ",") ?? "gówno",
          Sum = costam[8]?.InnerText.Replace(".", ",") ?? "gówno"
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
        var costam = item.ChildNodes["div"].ChildNodes["div"].Elements("div").ToList();
        var result = new Result
        {
          Place = Convert.ToByte(costam[0]?.InnerText ?? "0"),
          Jumper = costam[3]?.InnerText.Trim() ?? "gówno",
          FISCode = costam[2]?.InnerText ?? "gówno",
          Jump1 = costam[7]?.InnerText.Replace(".", ",") ?? "gówno",
          Jump2 = costam[9]?.InnerText.Replace(".", ",") ?? "gówno",
          Sum = costam[10]?.InnerText.Replace(".", ",") ?? "gówno"
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
      public string Sum { get; set; }
      public string FISPoints { get; set; }

      public override string ToString() => $"{Jumper} ({FISCode}) {Jump1} {Jump2} {Sum}";
    }
  }
}