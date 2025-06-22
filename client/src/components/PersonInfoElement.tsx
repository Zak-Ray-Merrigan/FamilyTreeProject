import React, { useEffect, useState } from "react";
import _ from "lodash";
import OutputResponse from "../models/outputResponse";
import { personElementToRepresentation } from "../Utils";
import FamilyRepresentationElement from "../models/RepresentationElement";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import PersonInfoInput from "../models/personInfoInput";
import './PersonInfoElement.css';


const PersonInfoElement: React.FC<PersonInfoInput> = (input) => {
    const [personInfoResult, setPersonInfoResult] = useState<OutputResponse<FamilyRepresentationElement>>({});
    const [personInfoShown, showPersonInfo] = useState<boolean>(false);
    const handleChecked = () => {
        showPersonInfo(!personInfoShown);
    };
    useEffect(() => {
        const handleRender = async() => {
            const response: OutputResponse<FamilyRepresentationElement> = await personElementToRepresentation(input.element);
            setPersonInfoResult(response);
        };
        handleRender();
    }, [input]);
    return (
        <div>
            <h3>{input.type}:</h3>
            {!_.isUndefined(personInfoResult.problem) && <ErrorDisplayComponent message={personInfoResult.problem.message}/>}
            {!_.isUndefined(personInfoResult.output) && (
                <p>{personInfoResult.output.representation}<span id="checkBoxPadder"><label><input type="checkbox" checked={personInfoShown} onChange={handleChecked}/>Show Person Info</label></span></p>
            )}
            {personInfoShown && (
                <div>
                    <p>Name: {input.element.name}</p>
                    <p>Birth Date: {input.element.birthDate}</p>
                    <p>Deceased Date: {input.element.deceasedDate}</p>
                </div>
            )}
        </div>
    )
};

export default PersonInfoElement;