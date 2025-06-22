import React, { FormEvent, useContext, useState } from "react";
import _ from "lodash";
import ReportActionsContext from "../contexts/ReportActionsContext";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { reportChildren } from "../ApiCalls";
import { FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { ReportChildrenRequest } from "../Types";
import { isProcessing } from "../Utils";

const ReportChildrenForm: React.FC = () => {
    const {response, isReportMade, setResponse} = useContext(ReportActionsContext);
    const {selectedPartnership} = useCriticalAttributes();
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [name, setName] = useState<string>('');
    const [birthDate, setBirthDate] = useState<string>('');

    const handleReportChildren = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isReportMade(true);
            if (_.isEqual(name, '')) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No child name provided.'});
            }
            else if (_.isEqual(birthDate, '')) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No child birth date provided.'});
            }
            else {
                const request: ReportChildrenRequest = {
                    parent: selectedPartnership,
                    child: {
                        member: {
                            name: name,
                            birthDate: birthDate,
                            deceasedDate: null
                        },
                        inLaw: null,
                        partnershipDate: null
                    }
                };
                addLoadingContext(LoadingContext.ReportChildren);
                setResponse(await reportChildren(request));
                if (!isProcessing(response)) {
                    removeLoadingContext(LoadingContext.ReportChildren);
                    setName('');
                    setBirthDate('');
                }
            }
            isReportMade(false);
        }
    };

    return (
        <form onSubmit={handleReportChildren}>
            <label>Child Name: <input type="text" value={name} onChange={(e) => setName(e.target.value)}/></label><br/>
            <label>Child Birth Date: <input type="text" value={birthDate} onChange={(e) => setBirthDate(e.target.value)}/></label><br/>
            <button type="submit">Report Child</button>
        </form>
    );
};

export default ReportChildrenForm;