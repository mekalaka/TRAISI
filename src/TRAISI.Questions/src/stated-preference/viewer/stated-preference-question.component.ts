import { Component, Inject, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Observable, ReplaySubject, BehaviorSubject } from 'rxjs';
import {
	OnOptionsLoaded,
	OnSaveResponseStatus,
	OnVisibilityChanged,
	QuestionOption,
	ResponseTypes,
	SurveyQuestion,
	SurveyViewer
} from 'traisi-question-sdk';
import { StatedPreferenceConfig } from '../stated-preference-config.model';
import { FormArrayName, NgForm } from '@angular/forms';
import * as dot from 'dot';
/**
 * Base question component definition for the question type "Stated Preference"
 *
 * @export
 * @class StatedPreferenceQuestionComponent
 * @extends {SurveyQuestion<ResponseTypes.String>}
 * @implements {OnInit}
 * @implements {OnVisibilityChanged}
 * @implements {OnSaveResponseStatus}
 */
@Component({
	selector: 'traisi-stated-preference-question',
	template: require('./stated-preference-question.component.html'),
	styles: [require('./stated-preference-question.component.scss')]
})
export class StatedPreferenceQuestionComponent extends SurveyQuestion<ResponseTypes.OptionSelect[]>
	implements OnInit, OnVisibilityChanged, OnSaveResponseStatus, AfterViewInit {
	public options: QuestionOption[];
	public model: ReplaySubject<StatedPreferenceConfig>;
	public hasError: boolean = false;
	public displayModel: ReplaySubject<Array<any>>;

	public inputModel: { value?: string };

	@ViewChild('spForm')
	public spForm: NgForm;

	/**
	 *Creates an instance of StatedPreferenceQuestionComponent.
	 * @param {SurveyViewer} _surveyViewerService
	 * @memberof StatedPreferenceQuestionComponent
	 */
	constructor(@Inject('SurveyViewerService') private _surveyViewerService: SurveyViewer) {
		super();
		this.model = new ReplaySubject<StatedPreferenceConfig>(1);
		this.displayModel = new ReplaySubject<Array<any>>(1);
		this.inputModel = {};
	}

	public onQuestionShown(): void {}
	public onQuestionHidden(): void {}
	public onResponseSaved(result: any): void {}

	/**
	 * @private
	 * @param {QuestionOption} value
	 * @memberof StatedPreferenceQuestionComponent
	 */
	private parseSpModel(value: any): void {
		try {
			let spModel = JSON.parse(value.label);
			this.model.next(spModel);
			this.displayModel.next(this.transformToDisplayableData(spModel));
		} catch (exception) {
			console.error(exception);
			this.hasError = true;
		}
	}

	/**
	 * @memberof StatedPreferenceQuestionComponent
	 */
	public ngOnInit(): void {
		this.questionOptions.subscribe(options => {
			if (options.length > 0) {
				this.parseSpModel(options[0]);
			}
		});
	}

	/**
	 * Converts the SP question configuration into a displayable array consumable
	 * by the cdk-table component. This involves flattening the information into
	 * a rectangular array representing row/column cell data.
	 * @private
	 * @param {StatedPreferenceConfig} config
	 * @returns {Array<any>}
	 * @memberof StatedPreferenceQuestionComponent
	 */
	private transformToDisplayableData(config: StatedPreferenceConfig): Array<any> {
		console.log('in transform display');
		let spDataArray: Array<any> = [];
		for (let r = 0; r < config.headers.length; r++) {
			let spDataRow: {} = {};
			for (let c = 0; c < config.choices.length; c++) {
				spDataRow[c] = dot.template(config.choices[c].items[r].label)();
			}
			spDataArray.push(spDataRow);
		}
		console.log(spDataArray);
		return spDataArray;
	}

	/**
	 * @param {*} $event
	 * @param {*} col
	 * @memberof StatedPreferenceQuestionComponent
	 */
	public selectChoice($event, col): void {
		console.log(col);
		console.log(this.inputModel);
	}

	/**
	 * @memberof StatedPreferenceQuestionComponent
	 */
	public ngAfterViewInit(): void {
		console.log(this.spForm);
	}
}
