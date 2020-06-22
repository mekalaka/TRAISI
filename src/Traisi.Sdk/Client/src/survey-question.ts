import { ResponseValidationState } from './question-response-state';
import { EventEmitter, Output, Inject, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { QuestionConfiguration } from './question-configuration';
import { BehaviorSubject, ReplaySubject, Observable } from 'rxjs';
import { QuestionOption } from './question-option';

/**
 * Base abstract class for Survey Questions available to TRAISI
 */
export abstract class SurveyQuestion<T extends ResponseTypes | ResponseTypes[]> {
	/**
	 * Output binding - question components embedded in other question types can subscribe
	 * to the questions response event to receive the generated "data" of that question type
	 * when it is completed by the user.
	 * @type {EventEmitter<ResponseData<T>>}
	 * @memberof SurveyQuestion
	 */
	@Output()
	public readonly response: EventEmitter<ResponseData<T>>;

	@Output()
	public readonly autoAdvance: EventEmitter<number>;

	@Output()
	public readonly validationState: EventEmitter<ResponseValidationState>;

	/**
	 * This value is id associated with the survey question. Each id will be unique.
	 */
	public questionId: number;

	/**
	 * The survey id
	 */
	public surveyId: number;

	/**
	 * The configuration for this question
	 */
	public configuration: QuestionConfiguration;

	/**
	 * The server configuration for this quesiton. This isn't specific to a survey, but related
	 * to the global server configuration, typically private configuration values.
	 * @type {{ [id: string] : any; }}
	 */
	public serverConfiguration: { [id: string]: any };

	/**
	 * The validity state of the question
	 */
	public isValid: boolean;

	/**
	 * The saved response - if any
	 */
	public savedResponse: ReplaySubject<ResponseData<T> | 'none'>;

	public isLoaded: BehaviorSubject<boolean>;

	public data: Array<any>;

	public pageIndex?: number;

	public displayClass: string = '';

	public isMultiPage: boolean = false;

	public respondent: GroupMember;

	public questionOptions: Observable<QuestionOption[]>;

	public configurations: Observable<QuestionConfiguration[]>;

	/**
	 *
	 *
	 * @private
	 * @param {QuestionConfiguration} configuration
	 * @memberof SurveyQuestion
	 */
	public loadConfiguration(configuration: QuestionConfiguration): void {
		this.configuration = configuration;
		this.isLoaded = new BehaviorSubject<boolean>(false);
	}

	/**
	 *
	 */
	constructor() {
		this.questionOptions = new ReplaySubject<QuestionOption[]>();
		this.configurations = new ReplaySubject<QuestionConfiguration[]>();
		this.response = new EventEmitter<ResponseData<T>>();
		this.autoAdvance = new EventEmitter<number>();
		this.validationState = new EventEmitter<ResponseValidationState>();
		this.savedResponse = new ReplaySubject<ResponseData<T> | 'none'>(1);
		this.questionId = -1;
		this.surveyId = -1;
		this.configuration = <QuestionConfiguration>{};
		this.isLoaded = new BehaviorSubject<boolean>(false);
		this.isValid = false;
		this.respondent = {
			relationship: '',
			id: -1,
			name: ''
		};
		this.data = [];
	}

	/**
	 * A question can override this method to signal to the survey viewer that
	 * the question implements special next functionality.
	 */
	public canNavigateInternalNext(): boolean {
		return false;
	}

	/**
	 * An override to signal to the viewer that this question has an internal navigation
	 * handler.
	 */
	public canNavigateInternalPrevious(): boolean {
		return false;
	}

	/**
	 * Called when the next is triggered in the survey viewer.
	 */
	public navigateInternalNext(): boolean {
		return true;
	}

	/**
	 * Called when previous is triggered in the survey viewer.
	 */
	public navigateInternalPrevious(): boolean {
		return true;
	}

	/**
	 *
	 *
	 * @memberof SurveyQuestion
	 */
	public traisiOnInit(isPreviousActionNext: boolean): void {}

	/**
	 * Called when the question has completed loading all of its data.
	 * This includes any saved response data and configuration data.
	 */
	public traisiOnLoaded(): void {}

	public traisiOnUnloaded(): void {}
}

/**
 *
 *
 * @export
 * @interface TraisiBuildable
 */
export interface TraisiBuildable {
	typeName: string;
	icon: string;
}

export abstract class ResponseData<T extends ResponseTypes | ResponseTypes[]> {}

export interface StringResponseData extends ResponseData<ResponseTypes.String> {
	value: string;
}

export interface DecimalResponseData extends ResponseData<ResponseTypes.Decminal> {
	value: number;
}

export interface NumberResponseData extends ResponseData<ResponseTypes.Number> {
	value: number;
}


export interface IntegerResponseData extends ResponseData<ResponseTypes.Integer> {
	value: number;
}

export interface TimeResponseData extends ResponseData<ResponseTypes.Time> {
	value: Date;
}

export interface DateResponseData extends ResponseData<ResponseTypes.Date> {
	value: Date;
}

export interface LocationResponseData extends ResponseData<ResponseTypes.Location> {
	latitude: number;
	longitude: number;
	address: string;
}

export interface TimelineResponseData extends ResponseData<ResponseTypes.Timeline> {
	latitude: number;
	longitude: number;
	address: string;
	timeA: Date;
	timeB: Date;
	name: string;
	order: number;
	purpose: string;
}

export interface RangeResponseData extends ResponseData<ResponseTypes.Range> {
	min: number;
	max: number;
}

export interface BooleanResponseData extends ResponseData<ResponseTypes.Boolean> {
	value: boolean;
}

export interface ListResponseData extends ResponseData<ResponseTypes.List> {
	values: Array<any>;
}

export interface OptionSelectResponseData extends ResponseData<ResponseTypes.OptionSelect> {
	value: any;
	name: any;
	code: string;
}

export interface RouteResponseData extends ResponseData<ResponseTypes.Path> {
	points: Array<any>;
}

/**
 * Wrapper interface format for save response returned from survey - responseValue in includes the dat
 */
export interface ResponseValue<T extends ResponseTypes> {
	responseValues: ResponseData<T>;
}

/**
 *
 *
 * @export
 * @enum {number}
 */
export enum ResponseTypes {
	Location = 'location',
	String = 'string',
	Integer = 'integer',
	Time = 'time',
	Date = 'date',
	Timeline = 'timeline',
	Decminal = 'decimal',
	Json = 'json',
	Path = 'path',
	Range = 'Range',
	List = 'List',
	Boolean = 'boolean',
	OptionSelect = 'option-select',
	Number = 'number',
	None = 'none'
}

export class GroupMember {
	id: number;
	name: string;
	relationship: string;
}