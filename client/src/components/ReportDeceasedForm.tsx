import React, { ChangeEvent, FormEvent, useContext, useState } from "react";
import _ from 'lodash';
import ReportActionsContext from "../contexts/ReportActionsContext";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { reportDeceased } from "../ApiCalls";
import { Root } from "../Constants";
import { FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { Person, ReportDeceasedRequest } from "../Types";
import { isProcessing } from "../Utils";

const ReportDeceasedForm: React.FC = () => {
    const {response, isReportMade, setResponse} = useContext(ReportActionsContext);
    const {selectedPartnership} = useCriticalAttributes();
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [person,setPerson] = useState<Person | null>(selectedPartnership.member);
    const [deceasedDate, setDeceasedDate] = useState<string>('');

    const handleChosenPerson = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.checked) {
            const name = e.target.value;
            if (!_.isNull(selectedPartnership.member) && selectedPartnership.member.name === name) {
                setPerson(selectedPartnership.member);
            }
            else if (!_.isNull(selectedPartnership.inLaw) && selectedPartnership.inLaw.name === name) {
                setPerson(selectedPartnership.inLaw);
            }
        }
    };

    const handleReportDeceased = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            isReportMade(true);
            e.preventDefault();
            if (_.isNull(person)) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'This person doesn\'t exist.'});
            }
            else if (_.isEqual(deceasedDate, '')) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No deceased date was provided.'});
            }
            else {
                const request: ReportDeceasedRequest = {
                    person: person,
                    deceasedDate: deceasedDate
                };
                addLoadingContext(LoadingContext.ReportDeceased);
                setResponse(await reportDeceased(request));
                if (!isProcessing(response)) {
                    removeLoadingContext(LoadingContext.ReportDeceased);
                    setDeceasedDate('');
                }
            }
            isReportMade(false);
        }
    };

    return _.isEqual(selectedPartnership, Root) ? null : (
        <form onSubmit={handleReportDeceased}>
            <h3>Person To Report:</h3>
            {selectedPartnership.member && <><label><input type="radio" checked={_.isEqual(selectedPartnership.member, person)} value={selectedPartnership.member.name} onChange={handleChosenPerson}/>{selectedPartnership.member.name}</label><br/></>}
            {selectedPartnership.inLaw && <><label><input type="radio" checked={_.isEqual(selectedPartnership.inLaw, person)} value={selectedPartnership.inLaw.name} onChange={handleChosenPerson}/>{selectedPartnership.inLaw.name}</label><br/></>}
            <h3>Deceased Date To Report:</h3>
            <label>Deceased Date: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Deceased</button>
        </form>
    );
};

export default ReportDeceasedForm;