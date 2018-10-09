import { Observable } from "rxjs";

export interface SurveyResponder {
        id: number;
        
        addSurveyGroupMember(respondent: SurveyRespondent): Observable<{}>;
        getSurveyGroupMembers(): Observable<{}>;
        removeSurveyGroupMember(respondent: SurveyRespondent): Observable<{}>;
};

export interface SurveyRespondent {
	firstName: string;
	lastName: string;
	id: number;
};



/*
        String,
        Boolean,
        Integer,
        Decimal,
        Location,
        Json,
        OptionList,
        DateTime
 */
