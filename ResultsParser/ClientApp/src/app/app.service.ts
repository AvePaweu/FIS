import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AppService {
  constructor(private http: HttpClient) { }

  getParsedData(raceId: string, type: number) {
    return this.http.get(`/api/Parser/${raceId}/${type}`);
  }
}