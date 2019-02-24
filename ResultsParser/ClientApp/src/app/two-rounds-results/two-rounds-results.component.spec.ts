import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TwoRoundsResultsComponent } from './two-rounds-results.component';

describe('TwoRoundsResultsComponent', () => {
  let component: TwoRoundsResultsComponent;
  let fixture: ComponentFixture<TwoRoundsResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TwoRoundsResultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TwoRoundsResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
