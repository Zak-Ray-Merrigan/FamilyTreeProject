import React, { useContext, useEffect, useState } from "react";
import FamilyElement, { FamilyElementContext } from "../models/FamilyElement";
import OutputResponse from "../models/outputResponse";
import { retrieveChildren } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyElementDisplay from "./FamilyElementDisplay";

const ChildrenOfSelectedElement: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const [childrenResponse, setChildrenResponse] = useState<OutputResponse<FamilyElement[]>>({});

    useEffect(() => {
        const fetchChildren = async() => {
            const response: OutputResponse<FamilyElement[]> = await retrieveChildren(selectedElement);
            setChildrenResponse(response);
        };
        fetchChildren();
    }, [selectedElement]);

    return (
        <div>
            <h2>Children:</h2>
            {childrenResponse.problem && <ErrorDisplayComponent message={childrenResponse.problem.message} />}
            {childrenResponse.output && (
                <>
                {childrenResponse.output.map((element: FamilyElement) => (
                    <FamilyElementDisplay member={element.member} inLaw={element.inLaw} marriageDate={element.marriageDate}/>
                ))}
                </>
            )}
            {(!childrenResponse.problem && !childrenResponse.output) && <h3>Loading Children...</h3>}
        </div>
    );
};

export default ChildrenOfSelectedElement;