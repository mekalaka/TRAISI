import { Injectable, Inject } from '@angular/core';
import { SurveyViewerState } from '../models/survey-viewer-state.model';
import { BehaviorSubject, ReplaySubject, Subject, Observable, Observer } from 'rxjs';
import { ResponseValidationState } from 'traisi-question-sdk';
import { SurveyViewGroupMember } from '../models/survey-view-group-member.model';
import { QuestionContainerComponent } from '../components/question-container/question-container.component';
import { SurveyViewQuestion } from '../models/survey-view-question.model';
import { SurveyViewerConditionalEvaluator } from './survey-viewer-conditional-evaluator.service';
import { EventEmitter } from 'events';
import { SurveyViewConditional } from 'app/models/survey-view-conditional.model';
import { SurveyResponderService } from './survey-responder.service';
@Injectable({
	providedIn: 'root'
})
export class SurveyViewerStateService {
	public static readonly SURVEY_QUESTIONS_CHANGED: string = 'SURVEY_QUESTIONS_CHANGED';

	public viewerState: SurveyViewerState;

	public surveyViewerState: ReplaySubject<SurveyViewerState>;

	public surveyQuestionsChanged: Subject<string>;

	/**
	 * Creates an instance of survey viewer state service.
	 * @param _conditionalEvaluator
	 * @param _responderService
	 */
	public constructor(
		private _conditionalEvaluator: SurveyViewerConditionalEvaluator,
		@Inject('SurveyResponderService') private _responderService: SurveyResponderService
	) {
		this.viewerState = {
			surveyPages: [],
			activeQuestion: undefined,
			activeSection: undefined,
			activePage: undefined,
			isSectionActive: false,
			surveyQuestions: [],
			surveyQuestionsFull: [],
			activeQuestionIndex: -1,
			activePageIndex: -1,
			groupMembers: [],
			activeGroupMemberIndex: -1,
			activeRepeatIndex: -1,
			primaryRespondent: undefined,
			activeGroupQuestions: [],
			isLoaded: false,
			questionMap: {}
		};

		this.surveyViewerState = new ReplaySubject<SurveyViewerState>();
		this.surveyQuestionsChanged = new Subject<string>();
	}

	/**
	 * Determines whether loaded is
	 * @param flag
	 */
	public isLoaded(flag: boolean): void {
		this.viewerState.isLoaded = flag;
	}

	/**
	 * Sets group member question validation state
	 * @param groupMember
	 * @param state
	 */
	public setGroupQuestionValidationState(memberIndex: number, state: ResponseValidationState): void {}

	/**
	 * Sets active question
	 * @param question
	 */
	public setActiveQuestion(question: SurveyViewQuestion): void {
		this.viewerState.activeQuestion = question;
		this.surveyViewerState.next(this.viewerState);
	}

	/**
	 * Updates state
	 * @param state
	 */
	public updateState(state: SurveyViewerState): void {
		this.viewerState = state;
		// this.surveyViewerState.next(this.viewerState);
	}

	/**
	 * Updates group question validation state
	 * @param question
	 * @param validationState
	 */
	public updateGroupQuestionValidationState(question: SurveyViewQuestion, validationState: ResponseValidationState): void {
		let index = this.viewerState.activeGroupQuestions.findIndex((f) => f.viewId === question.viewId);

		if (index >= 0) {
			this.viewerState.activeGroupQuestions[index].validationState = validationState;
		} else {
		}
		// this.surveyViewerState.next(this.viewerState);
	}

	/**
	 * Sets active group questions
	 * @param groupMembers
	 */
	public setActiveGroupQuestions(activeQuestion: SurveyViewQuestion, groupMembers: Array<SurveyViewGroupMember>): void {
		this.viewerState.activeGroupQuestions = [];
		groupMembers.forEach((member) => {
			let memberQuestion = Object.assign({}, activeQuestion);
			memberQuestion.viewId = Symbol();
			memberQuestion.parentMember = member;
			this.viewerState.activeGroupQuestions.push(memberQuestion);
		});

		// this.surveyViewerState.next(this.viewerState);
	}

	/**
	 * Evaluates repeat
	 * @param activeQuestion
	 */
	public evaluateRepeat(activeQuestion: SurveyViewQuestion, respondentId: number): Subject<void> {
		const subject: Subject<void> = new Subject<void>();

		if (activeQuestion.repeatTargets.length === 0) {
			setTimeout(() => {
				subject.next();
				subject.complete();
			});
			return subject;
		}

		this._responderService.readyCachedSavedResponses([activeQuestion.questionId], respondentId).subscribe((result) => {
			activeQuestion.repeatTargets.forEach((repeatTarget: number) => {
				const response: any = this._responderService.getCachedSavedResponse(activeQuestion.questionId, respondentId).value;

				// find index of repeat Target

				if (typeof response === 'number') {
					let targetQuestion: SurveyViewQuestion = this.viewerState.questionMap[repeatTarget];
					targetQuestion.repeatChildren = [];
					targetQuestion.repeatNumber = 0;
					for (let i: number = 0; i < response - 1; i++) {
						let duplicate: SurveyViewQuestion = Object.assign({}, targetQuestion);
						duplicate.repeatNumber = i + 1;
						targetQuestion.repeatChildren.push(duplicate);
					}
				}

				subject.next();
				subject.complete();
			});
		});

		return subject;
	}

	/**
	 * Updates active questions based on the last updated question id.
	 * @param updatedQuestionId
	 */
	public evaluateConditionals(updatedQuestionId: number, respondentId: number): Subject<void> {
		const subject = new Subject<void>();

		if (this.viewerState.questionMap[updatedQuestionId].sourceConditionals.length === 0) {
			setTimeout(() => {
				subject.next();
				subject.complete();
			});
			return subject;
		} else {
			this.viewerState.questionMap[updatedQuestionId].sourceConditionals.forEach((conditional) => {
				let targetQuestion = this.viewerState.questionMap[conditional.targetQuestionId];

				let sourceQuestionIds: number[] = [];

				targetQuestion.targetConditionals.forEach((targetConditional) => {
					sourceQuestionIds.push(targetConditional.sourceQuestionId);
				});

				this._responderService.readyCachedSavedResponses(sourceQuestionIds, respondentId).subscribe((value) => {
					let evalTrue: boolean = targetQuestion.targetConditionals.some((evalConditional) => {
						return this._conditionalEvaluator.evaluateConditional(
							evalConditional.conditionalType,
							this._responderService.getCachedSavedResponse(updatedQuestionId, respondentId),
							'',
							evalConditional.value
						);
					});

					const index: number = this.viewerState.surveyQuestions.findIndex(
						(sq) => sq.questionId === conditional.targetQuestionId
					);

					if (evalTrue) {
						if (index >= 0) {
							this.viewerState.surveyQuestions.splice(index, 1);
						}
					} else {
						if (index < 0) {
							// re add at the proper order

							for (let i = 0; i < this.viewerState.surveyQuestions.length - 1; i++) {
								if (
									targetQuestion.viewOrder > this.viewerState.surveyQuestions[i].viewOrder &&
									targetQuestion.viewOrder < this.viewerState.surveyQuestions[i + 1].viewOrder
								) {
									this.viewerState.surveyQuestions.splice(i + 1, 0, targetQuestion);
									break;
								}
							}
						}
					}

					this.surveyQuestionsChanged.next(SurveyViewerStateService.SURVEY_QUESTIONS_CHANGED);
					subject.next();
					subject.complete();
				});
			});

			return subject;
		}
	}
}