import { Component } from "@angular/core";
import { HttpClient } from "selenium-webdriver/http";
import { AppService } from "../app.service";

@Component({
  selector: 'app-parser',
  templateUrl: './parser.component.html'
})
export class ParserComponent {

  constructor(private api: AppService) { }

  raceId: string = '';
  competitionType: number = 1;
  results = null;

  parseResults(raceId: string, type: number) {
    this.api.getParsedData(raceId, type).subscribe(r => this.results = r);
  }
}