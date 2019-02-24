import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TwoRoundsTeamResultsComponent } from './two-rounds-team-results.component';

describe('TwoRoundsTeamResultsComponent', () => {
  let component: TwoRoundsTeamResultsComponent;
  let fixture: ComponentFixture<TwoRoundsTeamResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TwoRoundsTeamResultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TwoRoundsTeamResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
