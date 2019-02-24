import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-two-rounds-results',
  templateUrl: './two-rounds-results.component.html',
  styleUrls: ['./two-rounds-results.component.css']
})
export class TwoRoundsResultsComponent implements OnInit {

  @Input() results: any[];
  constructor() { }

  ngOnInit() {
  }

}
