import React, { ChangeEvent, FormEvent, useContext, useState } from "react";
import ReportDeceasedRequest from "../models/ReportDeceasedRequest";
import { FamilyElementContext } from "../models/FamilyElement";
import ReportActionsContext from "../models/ReportActionsContext";
import PersonElement from "../models/PersonElement";
import { StringDefault, reportDeceased } from "../Utils";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";
import { useNavigate } from "react-router-dom";
import _ from "lodash";

const ReportDeceasedForm: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {setResponse} = useContext(ReportActionsContext);
    const [person,setPerson] = useState<PersonElement>(selectedElement.member);
    const [deceasedDate, setDeceasedDate] = useState<string>(StringDefault);
    let navigate = useNavigate();

    const handleChoosenPerson = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.checked) {
            const name = e.target.value;
            if (selectedElement.member.name === name) {
                setPerson(selectedElement.member);
            }
            else if (selectedElement.inLaw.name === name) {
                setPerson(selectedElement.inLaw);
            }
        }
    };

    const handleReportDeceased = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const request: ReportDeceasedRequest = {
            element: person,
            deceasedDate: deceasedDate
        };
        const response: OutputResponse<MessageResponse> = await reportDeceased(request);
        setResponse(response);
        if (response.output) {
            navigate('/family-tree');
        }
    };

    return (
        <form onSubmit={handleReportDeceased}>
            <h3>Person To Report:</h3>
            <label><input type="radio" checked={_.isEqual(selectedElement.member, person)} value={selectedElement.member.name} onChange={handleChoosenPerson}/>{selectedElement.member.name}</label><br/>
            <label><input type="radio" checked={_.isEqual(selectedElement.inLaw, person)} value={selectedElement.inLaw.name} onChange={handleChoosenPerson}/>{selectedElement.inLaw.name}</label><br/>
            <h3>Deceased Date To Report:</h3>
            <label>Deceased Date: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Deceased</button>
        </form>
    );
};

export default ReportDeceasedForm;