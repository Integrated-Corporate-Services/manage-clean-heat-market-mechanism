import { Injectable } from '@angular/core';

@Injectable()
export class SessionStorageService {
  constructor() {}

  getObject<T>(key: string): T | null {
    const objectJson = sessionStorage.getItem(key);
    let object: T | null = null;
    if (objectJson !== null) {
      object = JSON.parse(objectJson);
    }
    return object;
  }

  setObject<T>(key: string, value: T) {
    const objectJson = JSON.stringify(value);
    sessionStorage.setItem(key, objectJson);
  }

  remove(key: string) {
    sessionStorage.removeItem(key);
  }
}
