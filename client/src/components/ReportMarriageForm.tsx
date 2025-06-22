import React, {useContext, useState} from "react";
import { useNavigate } from "react-router-dom";
import {FamilyDefault, FamilyElementContext} from "../models/FamilyElement";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";
import { reportMarriage } from "../Utils";
import ReportActionsContext from "../models/ReportActionsContext";

function stringToBoolean(value: string): boolean {
    if (value.toLowerCase() === 'true') {
        return true;
    }
    else if (value.toLowerCase() === 'false') {
        return false;
    }
    throw new Error(`"${value}" can't be converted to a boolean.`);
}

const ReportMarriageForm: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {setResponse} = useContext(ReportActionsContext);
    const [memberName, setMemberName] = useState<string>(selectedElement.member.name);
    const [memberNameIsBeingCustomized, isMemberNameBeingCustomized] = useState<boolean>(false);
    const [inLawName, setInLawName] = useState<string>(FamilyDefault.inLaw.name);
    const [inLawBirthDate, setInLawBirthDate] = useState<string>(FamilyDefault.inLaw.birthDate);
    const [inLawDeceasedDate, setInLawDeceasedDate] = useState<string>(FamilyDefault.inLaw.deceasedDate);
    const [marriageDate, setMarriageDate] = useState<string>(FamilyDefault.marriageDate);
    let navigate = useNavigate();

    const handleMemberNameCustomizationOptions = (e: React.ChangeEvent<HTMLInputElement>) => {
        isMemberNameBeingCustomized(stringToBoolean(e.target.value));
    };

    const handleMemberNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setMemberName(e.target.value);
    };

    const handleInLawNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setInLawName(e.target.value);
    };

    const handleInLawBirthDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setInLawBirthDate(e.target.value);
    };

    const handleInLawDeceasedDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setInLawDeceasedDate(e.target.value);
    };

    const handleMarriageDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setMarriageDate(e.target.value);
    };

    const handleReportMarriage = async(e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const response: OutputResponse<MessageResponse> = await reportMarriage({
            member: {
                name: memberName,
                birthDate: selectedElement.member.birthDate,
                deceasedDate: selectedElement.member.deceasedDate
            },
            inLaw: {
                name: inLawName,
                birthDate: inLawBirthDate,
                deceasedDate: inLawDeceasedDate
            },
            marriageDate: marriageDate
        });
        setResponse(response);
        if (response.output) {
            navigate('/family-tree');
        }
    };

    return (
        <form onSubmit={handleReportMarriage}>
            <h3>Member:</h3>
            <label><input type="radio" checked={!memberNameIsBeingCustomized} value={String(false)} onChange={handleMemberNameCustomizationOptions}/>Use {selectedElement.member.name}</label><br/>
            <label><input type="radio" checked={memberNameIsBeingCustomized} value={String(true)} onChange={handleMemberNameCustomizationOptions}/><input type="text" disabled={!memberNameIsBeingCustomized} value={memberName} onChange={handleMemberNameChange}/></label><br/>
            <h3>InLaw:</h3>
            <label>Name: <input type="text" value={inLawName} onChange={handleInLawNameChange}/></label><br/>
            <label>Birth Date: <input type="text" value={inLawBirthDate} onChange={handleInLawBirthDateChange}/></label><br/>
            <label>{'Deceased Date (leave unknown if not deceased): '}<input type="text" value={inLawDeceasedDate} onChange={handleInLawDeceasedDateChange}/></label><br/>
            <h3>Marriage Date:</h3>
            <label>Marriage Date: <input type="text" value={marriageDate} onChange={handleMarriageDateChange}/></label><br/>
            <button type="submit">Report Marriage</button>
        </form>
    )
};

export default ReportMarriageForm;