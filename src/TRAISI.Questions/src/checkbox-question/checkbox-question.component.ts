import { Component, EventEmitter, Inject, OnInit } from '@angular/core';
import {
	SurveyQuestion,
	ResponseTypes,
	SurveyResponder,
	QuestionConfiguration,
	SurveyViewer,
	OnSurveyQuestionInit,
	OnVisibilityChanged,
	OnSaveResponseStatus,
	StringResponseData,
	OnOptionsLoaded,
	QuestionOption
} from 'traisi-question-sdk';
@Component({
	selector: 'traisi-checkbox-question',
	template: require('./checkbox-question.component.html').toString(),
	styles: [require('./checkbox-question.component.scss').toString()]
})
export class CheckboxQuestionComponent extends SurveyQuestion<ResponseTypes.List>
	implements OnInit, OnOptionsLoaded {
	readonly QUESTION_TYPE_NAME: string = 'Checkbox Question';

	typeName: string;
	icon: string;

	options: QuestionOption[];

	/**
	 *
	 * @param surveyViewerService
	 */
	constructor(@Inject('SurveyViewerService') private surveyViewerService: SurveyViewer) {
		super();
		this.typeName = this.QUESTION_TYPE_NAME;
		this.icon = 'checkbox';
		this.options = [];

		this.surveyViewerService.configurationData.subscribe(this.loadConfigurationData);
	}

	/**
	 * Loads configuration data once it is available.
	 * @param data
	 */
	loadConfigurationData(data: QuestionConfiguration[]) {
		this.data = data;
	}

	ngOnInit() {
		console.log('init');
	}

	/**
	 *
	 * @param options
	 */
	onOptionsLoaded(options: QuestionOption[]): void {
		this.options = options;
	}
}
