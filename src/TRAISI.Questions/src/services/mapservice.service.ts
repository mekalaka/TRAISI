import { HttpResponseBase, HttpResponse, HttpErrorResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { catchError } from 'rxjs/internal/operators/catchError';
import { Observable, throwError } from 'rxjs';
import { EndpointFactory } from '../../../TRAISI/ClientApp/src/app/services/endpoint-factory.service';

@Injectable()
export class MapEndpointService {
	private readonly _reverseGeocodeUrl: string = '/api/mapservice/reversegeo';

	/**
	 * @param  {HttpClient} http
	 */
	constructor(private http: HttpClient) {

	}

	public reverseGeocode(latitude: number, longitude: number): Observable<string> {
		const endpointUrl = `${this._reverseGeocodeUrl}/${latitude}/${longitude}`;

		return this.http.get<string>(endpointUrl, this.getRequestHeaders('text'));
	}

	protected getRequestHeaders(rType:any='json'): { headers: HttpHeaders | { [header: string]: string | string[]; }; responseType: any } {
		const headers = new HttpHeaders({
			'Authorization': 'Bearer ' + JSON.parse(sessionStorage.getItem('access_token')),
			'Content-Type': 'application/json',
			'Accept': `application/vnd.iman.v1+json, application/json, text/plain, */*`,
			'App-Version': '2.0.0'
		});

		return { headers: headers, responseType:rType };
	}

}