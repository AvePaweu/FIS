import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-two-rounds-team-results',
  templateUrl: './two-rounds-team-results.component.html',
  styleUrls: ['./two-rounds-team-results.component.css']
})
export class TwoRoundsTeamResultsComponent implements OnInit {

  @Input() results: any[];
  constructor() { }

  ngOnInit() {
  }

}
