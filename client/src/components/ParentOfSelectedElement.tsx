import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import FamilyElement, { FamilyElementContext } from "../models/FamilyElement";
import OutputResponse from "../models/outputResponse";
import { retrieveParent } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyElementDisplay from "./FamilyElementDisplay";

const ParentOfSelectedElement: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const [parentResult,setParentResult] = useState<OutputResponse<FamilyElement>>({});
    useEffect(() => {
        const getParentElement = async () => {
            const response: OutputResponse<FamilyElement> = await retrieveParent(selectedElement);
            setParentResult(response);
        }
        getParentElement();
    }, [selectedElement]);

    return (
        <div>
            <h2>Parent:</h2>
            {!_.isUndefined(parentResult.problem) && <ErrorDisplayComponent message={parentResult.problem.message}/>}
            {!_.isUndefined(parentResult.output) && <FamilyElementDisplay member={parentResult.output.member} inLaw={parentResult.output.inLaw} marriageDate={parentResult.output.marriageDate}/>}
        </div>
    );
};

export default ParentOfSelectedElement;