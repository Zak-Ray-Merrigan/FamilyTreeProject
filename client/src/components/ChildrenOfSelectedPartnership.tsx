import React, { useEffect, useState } from "react";
import ErrorDisplay from "./ErrorDisplay";
import FamilyElementDisplay from "./PartnershipDisplay";
import LoadingComponent from "./LoadingDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { retrieveChildren } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { FamilyTreeApiResponse, Partnership } from "../Types";
import { isProcessing, isSuccess } from "../Utils";

const ChildrenOfSelectedPartnership: React.FC = () => {
    const {selectedPartnership} = useCriticalAttributes();
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [childrenResponse, setChildrenResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);

    useEffect(() => {
        const fetchChildren = async() => {
            addLoadingContext(LoadingContext.RetrieveChildren);
            const response = await retrieveChildren(selectedPartnership);
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.ReportChildren);
            }
            setChildrenResponse(response);
        };
        fetchChildren();
    }, [selectedPartnership, addLoadingContext, removeLoadingContext]);
    return (
        <div>
            <h2>Children:</h2>
            <LoadingComponent context={LoadingContext.RetrieveChildren} response={childrenResponse} />
            <ErrorDisplay response={childrenResponse} />
            {isSuccess(childrenResponse) && (
                <>
                {(childrenResponse.result as Partnership[]).map((element: Partnership) => (
                    <FamilyElementDisplay member={element.member} inLaw={element.inLaw} partnershipDate={element.partnershipDate}/>
                ))}
                </>
            )}
        </div>
    )
};

export default ChildrenOfSelectedPartnership;