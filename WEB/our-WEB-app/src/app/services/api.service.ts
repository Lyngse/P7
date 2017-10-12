import { Injectable, Injector, Inject } from '@angular/core';
import { Http, Headers, RequestOptions } from "@angular/http";
import { Observable, BehaviorSubject } from 'rxjs/Rx';
import 'rxjs/Rx';

@Injectable()
export class APIService {

    private headers = new Headers({'Content-Type': 'application/json', 'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8'});
    options = new RequestOptions({ headers: this.headers });
    private url = 'https://api.etilbudsavis.dk/v2/';

    constructor( private http: Http){

    };

    getSessionToken():Promise<any>{
        return this.http.post(this.url + 'sessions', JSON.stringify({api_key: '00j8o9yy0mfq2h9li9ngs8xohxe5tuab'}), this.options)
        .toPromise()
        .then(res => res.json())
        .catch(this.handleError);
    };

    private handleError(error: any): Promise<any>{
        console.error('An error occurred', error); // for demo purposes only
        return Promise.reject(error || 'Server error')
    }
}