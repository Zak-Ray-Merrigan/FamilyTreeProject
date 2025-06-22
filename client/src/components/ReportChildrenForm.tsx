import React, { FormEvent, useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import ReportChildrenRequest from "../models/ReportChildrenRequest";
import { FamilyElementContext } from "../models/FamilyElement";
import ReportActionsContext from "../models/ReportActionsContext";
import { StringDefault, reportChildren } from "../Utils";
import { PersonDefault } from "../models/PersonElement";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";

const ReportChildrenForm: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {setResponse} = useContext(ReportActionsContext);
    const [name, setName] = useState<string>(StringDefault);
    const [birthDate, setBirthDate] = useState<string>(StringDefault);
    const [deceasedDate, setDeceasedDate] = useState<string>(StringDefault);
    let navigate = useNavigate();

    const handleReportChildren = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const request: ReportChildrenRequest = {
            parent: selectedElement,
            child: {
                member: {
                    name: name,
                    birthDate: birthDate,
                    deceasedDate: deceasedDate
                },
                inLaw: PersonDefault,
                marriageDate: StringDefault
            }
        };
        const response: OutputResponse<MessageResponse> = await reportChildren(request);
        setResponse(response);
        if (response.output) {
            navigate('/family-tree');
        }
    };

    return (
        <form onSubmit={handleReportChildren}>
            <label>Child Name: <input type="text" value={name} onChange={(e) => setName(e.target.value)}/></label><br/>
            <label>Child Birth Date: <input type="text" value={birthDate} onChange={(e) => setBirthDate(e.target.value)}/></label><br/>
            <label>Child Name: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Children</button>
        </form>
    );
};

export default ReportChildrenForm;