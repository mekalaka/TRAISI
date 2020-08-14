import { Injectable } from '@angular/core';
import { CalendarEvent } from 'angular-calendar';
import { Subject, BehaviorSubject, Observable, concat, of } from 'rxjs';
import { TravelDiaryConfiguration } from './travel-diary-configuration.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, debounceTime, distinctUntilChanged, switchMap, tap, map } from 'rxjs/operators';
import { formatRelative } from 'date-fns';
@Injectable()
export class TravelDiaryService {
	public diaryEvents$: BehaviorSubject<CalendarEvent>;

	public configuration: TravelDiaryConfiguration = { purposes: [] };

	public addresses$: Observable<string[]>;

	public addressInput$: Subject<string> = new Subject<string>();

	public addressesLoading: boolean = false;

	public constructor(private _http: HttpClient) {}

	public initialize(configuration: any): void {
		let ps = configuration.purpose.split(' | ');
		console.log(ps);
		this.configuration.purposes = ps;
		this.loadAddresses();
	}

	public loadAddresses(): void {
		this.addresses$ = <any>concat(
			of(['']), // default items
			this.addressInput$.pipe(
                distinctUntilChanged(),
                debounceTime(500),
				tap(() => (this.addressesLoading = true)),
				switchMap((term) =>
					this.queryAddresses(term).pipe(
						map((v) => {
                            return v['features'];
						}),
						catchError(() => of([])), // empty list on error
						tap((v) => {
							this.addressesLoading = false;
						})
					)
				)
			)
		);
	}

	///geocoding/v5/{endpoint}/{search_text}.json
	private queryAddresses(input: string): Observable<Object> {
		const url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${input}.json`;
		const options = {
			params: new HttpParams().set(
				'access_token',
				'pk.eyJ1IjoiYnJlbmRhbmJlbnRpbmciLCJhIjoiY2s4Y3IwN3U3MG1obzNsczJjMGhoZWc4MiJ9.OCDfSypjueUF_gKejRr6Og'
			).set('country','ca'),
		};
		return this._http.get(url, options);
	}

	public get diaryEvents(): CalendarEvent[] {
		return undefined;
	}
}
