import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import { FamilyElementContext } from "../models/FamilyElement";
import PersonInfoElement from "./PersonInfoElementComponent";
import { PersonType } from "../models/personInfoInput";
import OutputResponse from "../models/outputResponse";
import { generationNumberOf } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";

const FamilyInfoElement: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const [generationNumberResponse, setGenerationNumberResponse] = useState<OutputResponse<number>>({});

    useEffect(() => {
        const handleGenerationNumberOf = async() => {
            const response: OutputResponse<number> = await generationNumberOf(selectedElement);
            setGenerationNumberResponse(response);
        };
        handleGenerationNumberOf();
    }, [selectedElement]);
    if (generationNumberResponse.problem) {
        return (
            <ErrorDisplayComponent message={generationNumberResponse.problem.message}/>
        );
    }
    else if (generationNumberResponse.output) {
        return (
            <div>
                <h2>Info:</h2>
                <h3>Generation Number: {generationNumberResponse.output}</h3>
                <PersonInfoElement type={PersonType.Member} element={selectedElement.member}/>
                <PersonInfoElement type={PersonType.InLaw} element={selectedElement.inLaw}/>
                <h3>Marriage Date: {_.isNull(selectedElement.marriageDate) ? 'unknown' : selectedElement.marriageDate}</h3>
            </div>
        );
    }
    return (
        <h2>Loading Element...</h2>
    )
};

export default FamilyInfoElement;