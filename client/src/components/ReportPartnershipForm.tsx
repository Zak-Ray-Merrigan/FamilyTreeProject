import React, {ChangeEvent, FormEvent, useContext, useState} from "react";
import _ from "lodash";
import ReportActionsContext from "../contexts/ReportActionsContext";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { reportMarriage } from "../ApiCalls";
import { FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { ReportPartnershipRequest } from "../Types";
import { isProcessing } from "../Utils";

function stringToBoolean(value: string) {
    if (value.toLowerCase() === 'true') {
        return true;
    }
    else if (value.toLowerCase() === 'false') {
        return false;
    }
    throw new Error(`"${value}" can't be converted to a boolean.`);
}

const ReportPartnershipForm: React.FC = () => {
    const {response, isReportMade, setResponse} = useContext(ReportActionsContext);
    const {selectedPartnership} = useCriticalAttributes();
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [memberName, setMemberName] = useState<string>(_.isNull(selectedPartnership.member) ? '' : selectedPartnership.member.name);
    const [memberNameIsBeingCustomized, isMemberNameBeingCustomized] = useState<boolean>(false);
    const [inLawName, setInLawName] = useState<string>('');
    const [inLawBirthDate, setInLawBirthDate] = useState<string>('');
    const [partnershipDate, setPartnershipDate] = useState<string>('');

    const handleMemberNameCustomizationOptions = (e: React.ChangeEvent<HTMLInputElement>) => {
        isMemberNameBeingCustomized(stringToBoolean(e.target.value));
    };

    const handleMemberNameChange = (e: ChangeEvent<HTMLInputElement>) => {
        setMemberName(e.target.value);
    };

    const handleInLawNameChange = (e: ChangeEvent<HTMLInputElement>) => {
        setInLawName(e.target.value);
    };

    const handleInLawBirthDateChange = (e: ChangeEvent<HTMLInputElement>) => {
        setInLawBirthDate(e.target.value);
    };

    const handlePartnershipDateChange = (e: ChangeEvent<HTMLInputElement>) => {
        setPartnershipDate(e.target.value);
    };

    const handleReportPartnership = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isReportMade(true);
            if (_.isNull(selectedPartnership.member)) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No member exists.'});
            }
            else if (_.isEqual(memberName, '')) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No family member was provided.'});
            }
            else if (_.isEqual(inLawName, '')) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No in-law name was provided.'});
            }
            else if (_.isEqual(inLawBirthDate, '')) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No in-law birth date was provided.'});
            }
            else if (_.isEqual(partnershipDate, '')) {
                setResponse({status: FamilyTreeApiResponseStatus.Failure, message: 'No partnership date was provided.'});
            }
            else {
                const request: ReportPartnershipRequest = {
                    initialMember: selectedPartnership.member,
                    newPartnership: {
                        member: {
                            name: memberName,
                            birthDate: selectedPartnership.member.birthDate,
                            deceasedDate: selectedPartnership.member.deceasedDate
                        },
                        inLaw: {
                            name: inLawName,
                            birthDate: inLawBirthDate,
                            deceasedDate: null
                        },
                        partnershipDate: partnershipDate
                    }
                };
                addLoadingContext(LoadingContext.ReportPartnership);
                setResponse(await reportMarriage(request));
                if (!isProcessing(response)) {
                    removeLoadingContext(LoadingContext.ReportPartnership);
                    setMemberName('');
                    setInLawName('');
                    setInLawBirthDate('');
                    setPartnershipDate('');
                }
            }
            isReportMade(false);
        }
    };

    return _.isNull(selectedPartnership.member) ? null : (
        <form onSubmit={handleReportPartnership}>
            <h3>Member:</h3>
            <label><input type="radio" checked={!memberNameIsBeingCustomized} value={String(false)} onChange={handleMemberNameCustomizationOptions}/>Use {selectedPartnership.member.name}</label><br/>
            <label><input type="radio" checked={memberNameIsBeingCustomized} value={String(true)} onChange={handleMemberNameCustomizationOptions}/><input type="text" disabled={!memberNameIsBeingCustomized} value={memberName} onChange={handleMemberNameChange}/></label><br/>
            <h3>InLaw:</h3>
            <label>Name: <input type="text" value={inLawName} onChange={handleInLawNameChange}/></label><br/>
            <label>Birth Date: <input type="text" value={inLawBirthDate} onChange={handleInLawBirthDateChange}/></label><br/>
            <h3>Partnership Date:</h3>
            <label>Partnership Date: <input type="text" value={partnershipDate} onChange={handlePartnershipDateChange}/></label><br/>
            <button type="submit">Report Partnership</button>
        </form>
    )
};

export default ReportPartnershipForm;