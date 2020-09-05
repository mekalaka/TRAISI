import { Component, ViewEncapsulation, OnChanges, SimpleChanges, OnInit, Input, HostBinding } from '@angular/core';
import { CalendarWeekViewComponent, CalendarEvent } from 'angular-calendar';
import templateString from './travel-diary-event-display.component.html';
import styleString from './travel-diary-event-display.component.scss';
import { TravelDiaryEvent, TimelineLineResponseDisplayData } from 'travel-diary/models/consts';
import { TravelDiaryViewTimeEvent } from 'travel-diary/models/travel-diary-view-time-event.model';
import { Address } from 'traisi-question-sdk';
@Component({
	// tslint:disable-line max-classes-per-file
	selector: 'traisi-travel-diary-event-display',
	template: '' + templateString,
	styles: ['' + styleString],
	providers: [],
})
export class TravelDiaryEventDisplayComponent implements OnInit {
	@Input() public timeEvent: TravelDiaryViewTimeEvent;

	@HostBinding('class') public class = '';

	public get addressDisplay(): string {
		let address = this.timeEvent.event.meta.model?.address as Address;
		if (address) {
			return `${address.streetNumber} ${address.streetAddress}, ${address.city}`;
		} else {
			return '';
		}
	}

	public get model(): TimelineLineResponseDisplayData {
		return this.timeEvent.event.meta.model;
	}

	public get dateDisplay(): string {
		return this.model.timeA.toString();
	}

	public get isFirstEvent(): boolean {
		return this.timeEvent.event.start.getHours() === 0 && this.timeEvent.event.start.getMinutes() === 0;
	}

	public ngOnInit(): void {
		if (this.timeEvent.event.meta.homeAllDay) {
			this.class = 'event-home-all-day';
		}
	}
}