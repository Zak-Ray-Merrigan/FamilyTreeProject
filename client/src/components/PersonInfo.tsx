import React, { useEffect, useState } from "react";
import _ from "lodash";
import ErrorDisplayComponent from "./ErrorDisplay";
import LoadingComponent from "./LoadingDisplay";
import useLoadingContext from "../hooks/useLoadingContext";
import '../styles/PersonInfo.css';
import { personToRepresentation } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext, PersonType } from "../Enums";
import { FamilyTreeApiResponse, Person, RepresentationElement } from "../Types";
import { isProcessing, isSuccess } from "../Utils";

/**
 * Represents input for person information, including their type (Member or InLaw).
 */
interface PersonInfoInput {
    type: PersonType;      // The type of the person (Member or InLaw)
    element: Person | null; // The person's data, or null if not available
}

const PersonInfo: React.FC<PersonInfoInput> = (input) => {
    const [personInfoResult, setPersonInfoResult] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [personInfoShown, showPersonInfo] = useState<boolean>(false);
    const handleChecked = () => {
        showPersonInfo(!personInfoShown);
    };
    useEffect(() => {
        const handleRender = async() => {
            if (!_.isNull(input.element)) {
                addLoadingContext(LoadingContext.PersonElementToRepresentation);
                setPersonInfoResult(await personToRepresentation(input.element));
                if (!isProcessing(personInfoResult)) {
                    removeLoadingContext(LoadingContext.PersonElementToRepresentation);
                }
            }
        };
        handleRender();
    }, [input, addLoadingContext, removeLoadingContext, personInfoResult]);
    
    return _.isNull(input.element) ? null : (
        <div>
            <h3>{input.type}:</h3>
            <LoadingComponent context={LoadingContext.PersonElementToRepresentation} response={personInfoResult} />
            <ErrorDisplayComponent response={personInfoResult} />
            {isSuccess(personInfoResult) && (
                <p>{(personInfoResult.result as RepresentationElement).representation}<span id="checkBoxPadder"><label><input type="checkbox" checked={personInfoShown} onChange={handleChecked}/>Show Person Info</label></span></p>
            )}
            {personInfoShown && (
                <div>
                    <p>Name: {input.element.name}</p>
                    <p>Birth Date: {input.element.birthDate}</p>
                    {input.element.deceasedDate && <p>Deceased Date: {input.element.deceasedDate}</p>}
                </div>
            )}
        </div>
    )
};

export default PersonInfo;