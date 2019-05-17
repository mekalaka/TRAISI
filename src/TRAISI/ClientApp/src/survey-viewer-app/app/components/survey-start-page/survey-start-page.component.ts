import {
	Component,
	OnInit,
	ViewChild,
	ViewContainerRef,
	Inject,
	ViewEncapsulation,
	ElementRef,
	ComponentFactoryResolver,
	ComponentRef,
	TemplateRef
} from '@angular/core';
import { SurveyViewerService } from '../../services/survey-viewer.service';
import { ActivatedRoute, Router, NavigationEnd, RouterOutlet, Params } from '@angular/router';
import { SurveyStart } from '../../models/survey-start.model';
import { User } from 'shared/models/user.model';
import { SurveyShortcodePageComponent } from '../survey-shortcode-page/survey-shortcode-page.component';
import { SurveyGroupcodePageComponent } from '../survey-groupcode-page/survey-groupcode-page.component';
import { SurveyShortcodeDisplayPageComponent } from '../survey-shortcode-display-page/survey-shortcode-display-page.component';

import { SurveyViewerSession } from 'app/services/survey-viewer-session.service';
import { SurveyViewerSessionData } from 'app/models/survey-viewer-session-data.model';
import { zip } from 'rxjs';

@Component({
	selector: 'traisi-survey-start-page',
	templateUrl: './survey-start-page.component.html',
	styleUrls: ['./survey-start-page.component.scss'],
	entryComponents: [SurveyShortcodePageComponent, SurveyGroupcodePageComponent, SurveyShortcodeDisplayPageComponent],
	encapsulation: ViewEncapsulation.None
})
export class SurveyStartPageComponent implements OnInit {
	public finishedLoading: boolean = false;
	public pageThemeInfo: any = {};
	public surveyName: string;
	public isLoading: boolean = false;
	public shortcode: string;
	public isAdmin: boolean = false;
	public surveyStartConfig: SurveyStart;
	public isError: boolean = false;
	public hasGroupcode: boolean;
	public groupcode: string;
	public isChildPage: boolean = true;
	public session: SurveyViewerSessionData;

	private _queryParams: Params;

	@ViewChild('codeComponent', { read: ViewContainerRef })
	public codeComponent: ViewContainerRef;

	@ViewChild('outlet', { read: RouterOutlet })
	public outlet: RouterOutlet;

	/**
	 *Creates an instance of SurveyStartPageComponent.
	 * @param {SurveyViewerService} _surveyViewerService
	 * @param {ActivatedRoute} _route
	 * @param {Router} _router
	 * @param {ElementRef} _elementRef
	 * @param {ComponentFactoryResolver} _componentFactoryResolver
	 * @param {SurveyViewerSession} _surveySession
	 * @memberof SurveyStartPageComponent
	 */
	constructor(
		@Inject('SurveyViewerService') private _surveyViewerService: SurveyViewerService,
		private _route: ActivatedRoute,
		private _router: Router,
		private _elementRef: ElementRef,
		private _componentFactoryResolver: ComponentFactoryResolver,
		private _surveySession: SurveyViewerSession
	) {}

	/**
	 *
	 *
	 * @memberof SurveyStartPageComponent
	 */
	public ngOnInit(): void {
		this.isAdmin = this._surveyViewerService.isAdminUser();
		zip(this._route.params, this._route.queryParams).subscribe(([routeParams, queryParams]: [Params, Params]) => {
			this._queryParams = queryParams;
		});

		this._surveyViewerService.welcomeModel.subscribe((surveyStartModel: SurveyStart) => {
			this.surveyStartConfig = surveyStartModel;

			this.evaluateSurveyType();
		});

		this._route.queryParams.subscribe((queryParms: Params) => {
			this._queryParams = queryParms;
		});

		if (this.outlet.component) {
			if (this.outlet.isActivated) {
				this.outlet.component['startPageComponent'] = this;
			}
		}

		this._router.events.subscribe((event) => {
			if (event instanceof NavigationEnd) {
				if (this.outlet.isActivated) {
					this.outlet.component['startPageComponent'] = this;
				}
			}
		});

		this._surveySession.data.subscribe((data) => {
			this.session = data;
		});
	}

	/**
	 * @private
	 * @memberof SurveyStartPageComponent
	 */
	private evaluateSurveyType(): void {
		if (!this.surveyStartConfig.hasGroupCodes) {
			// this.loadShortcodeComponent();
		} else {
			if (this._route.snapshot.children.length === 0) {
				this._router.navigate(['groupcode'], { relativeTo: this._route });
			} else if (this._route.snapshot.children.length === 1) {
				this._route.children[0].data.subscribe((data) => {
					if (data.shortcodePage) {
						this._router.navigate(['groupcode'], { relativeTo: this._route });
					}
				});
			}
		}
	}

	/**
	 * @memberof SurveyStartPageComponent
	 */
	public groupcodeStartSurvey(groupcode: string): void {
		const groupcodeMod: string = groupcode.trim();
		this._surveyViewerService.startSurveyWithGroupcode(this.surveyStartConfig.id, groupcodeMod, this._queryParams).subscribe(
			(result) => {
				if (result.success) {
					// this.loadShortcodeDisplayComponent(result.shortcode);
					this._surveySession.setGroupcode(groupcode);
					this._router.navigate(['shortcode'], {
						relativeTo: this._route,
						queryParams: {
							shortcode: result.shortcode
						}
					});
				}
			},
			(error) => {
				console.log(error);
			}
		);
	}

	/**
	 *
	 *
	 * @param {string} shortcode
	 * @memberof SurveyStartPageComponent
	 */
	public surveyLogin(shortcode: string, groupcode?: string): void {
		this._surveyViewerService.surveyLogin(this.surveyStartConfig.id, shortcode).subscribe((user: User) => {
			this._router.navigate([this.session.surveyCode, 'terms']);
		});
	}

	/**
	 * Starts the survey - this will authorize the current user with the associated
	 * short code. This will create a new survey user if one does not exist.
	 */
	public startSurvey(code: string): void {
		this.shortcode = code;
		this.isLoading = true;
		this.isError = false;
		code = code === undefined ? code : code.trim();
		this._surveyViewerService.surveyStart(this.surveyStartConfig.id, this.shortcode, this._queryParams).subscribe(
			(value) => {
				this.isLoading = false;
				if (!this.isAdmin) {
					this._surveyViewerService.surveyLogin(this.surveyStartConfig.id, this.shortcode).subscribe((user: User) => {
						this._router.navigate([this.session.surveyCode, 'terms']);
					});
				} else {
					this._router.navigate([this.session.surveyCode, 'terms']);
				}
			},
			(error) => {
				console.error(error);
				this.isLoading = false;
				this.isError = true;
			}
		);
	}
}
