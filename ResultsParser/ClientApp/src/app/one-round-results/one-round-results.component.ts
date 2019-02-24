import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-one-round-results',
  templateUrl: './one-round-results.component.html',
  styleUrls: ['./one-round-results.component.css']
})
export class OneRoundResultsComponent implements OnInit {

  @Input() results: any[];
  constructor() { }

  ngOnInit() {
  }

}
