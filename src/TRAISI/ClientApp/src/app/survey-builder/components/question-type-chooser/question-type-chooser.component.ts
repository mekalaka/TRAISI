import { Component, OnInit, ViewEncapsulation, ElementRef, Output, EventEmitter, HostListener, Input } from '@angular/core';
import { SurveyBuilderService } from '../../services/survey-builder.service';
import { QuestionTypeDefinition } from '../../models/question-type-definition';
import { AppConfig } from '../../../app.config';
import { QuestionIconType } from '../../models/question-icon-type.enum';

@Component({
	selector: 'traisi-question-type-chooser',
	templateUrl: './question-type-chooser.component.html',
	styleUrls: ['./question-type-chooser.component.scss'],
	encapsulation: ViewEncapsulation.None
})
export class QuestionTypeChooserComponent implements OnInit {
	public questionTypeDefinitions: QuestionTypeDefinition[];
	public config: any;
	public $el: any;
	public dragItemIndex: number = 0;
	public wasDragging: boolean = false;

	@Input() disabled: boolean = false;
	@Output() addQuestionType: EventEmitter<QuestionTypeDefinition> = new EventEmitter<QuestionTypeDefinition>();
	@Output() loadedQuestionTypes: EventEmitter<any> = new EventEmitter();

	constructor(private surveyBuilderService: SurveyBuilderService, config: AppConfig, el: ElementRef) {
		this.config = config.getConfig();
		this.$el = jQuery(el.nativeElement);
		this.getQuestionPayload = this.getQuestionPayload.bind(this);
	}

	/**
	 * Component initialization
	 */
	ngOnInit() {
		this.questionTypeDefinitions = [];

		// retrieve all question types from the server
		this.surveyBuilderService
			.getQuestionTypes()
			.subscribe((value: QuestionTypeDefinition[]) => {
				this.questionTypeDefinitions = value;
				this.loadedQuestionTypes.emit();
			});

		jQuery(window).on('sn:resize', this.initSidebarScroll.bind(this));
		this.initSidebarScroll();
	}

	initSidebarScroll(): void {
		const $sidebarContent = this.$el.find('.js-builder-sidebar-content');
		if (this.$el.find('.slimScrollDiv').length !== 0) {
			$sidebarContent.slimscroll({
				destroy: true
			});
		}
		$sidebarContent.slimscroll({
			height: window.innerHeight,
			size: '4px'
		});
	}

	toggleSidebarOverflow($event) {
		jQuery('#builderSidebar').css('z-index', $event ? '2' : '0');
		jQuery('.js-builder-sidebar-content, .slimScrollDiv').css('overflow', $event ? 'visible' : 'hidden');
	}

	addQuestionTypeToList(qType: QuestionTypeDefinition) {
		if (!this.wasDragging) {
			this.addQuestionType.emit(qType);
		}
	}

	addSectionToList() {
		if (!this.wasDragging) {
			let surveyPart: QuestionTypeDefinition = {
				typeName: 'Survey Part',
				icon: 'fa-archive',
				questionConfigurations: [],
				questionOptions: []
			};
			this.addQuestionType.emit(surveyPart);
		}
	}

	onDragStart(event: any) {
		$('.collapse.details').collapse('hide');
		/*if (event.payload.typeName !== 'Survey Part') {
			$('.collapse:not(.details)').collapse('show');
		}*/
		if (event.isSource) {
			this.wasDragging = true;
		}
	}

	onDragEnd(event: any) {
		if (event.isSource) {
			setTimeout(() => {
				this.wasDragging = false;
			}, 0);
		}
	}


	getQuestionPayload(index) {
		if (index === 0) {
			let surveyPart = {
				typeName: 'Survey Part',
				icon: 'fa-archive'
			};
			return surveyPart;
		} else {
			return this.questionTypeDefinitions[index - 1];
		}
	}
}

