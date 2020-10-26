import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
  })
export class DataService {

  private message = new BehaviorSubject('false');
  sharedMessage = this.message.asObservable();

  constructor() { }

  nextMessage(message: string) {
    this.message.next(message)
  }
  
}