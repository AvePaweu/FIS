import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OneRoundTeamResultsComponent } from './one-round-team-results.component';

describe('OneRoundTeamResultsComponent', () => {
  let component: OneRoundTeamResultsComponent;
  let fixture: ComponentFixture<OneRoundTeamResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OneRoundTeamResultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OneRoundTeamResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
