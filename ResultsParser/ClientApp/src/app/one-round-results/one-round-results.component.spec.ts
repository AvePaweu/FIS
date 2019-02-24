import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OneRoundResultsComponent } from './one-round-results.component';

describe('OneRoundResultsComponent', () => {
  let component: OneRoundResultsComponent;
  let fixture: ComponentFixture<OneRoundResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OneRoundResultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OneRoundResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
