import React, { useEffect, useState } from "react";
import ErrorDisplayComponent from "./ErrorDisplay";
import LoadingComponent from "./LoadingDisplay";
import PartnershipDisplay from "./PartnershipDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { FamilyTreeApiResponse, Partnership } from "../Types";
import { retrieveParent } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";

const ParentOfSelectedPartnership: React.FC = () => {
    const {selectedPartnership} = useCriticalAttributes();
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [parentResult, setParentResult] = useState<FamilyTreeApiResponse>(EmptyResponse);
    useEffect(() => {
        const getParentOfPartnership = async () => {
            addLoadingContext(LoadingContext.RetrieveParent);
            const response = await retrieveParent(selectedPartnership);
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.RetrieveParent);
            }
            setParentResult(response);
        }
        getParentOfPartnership();
    }, [selectedPartnership, addLoadingContext, removeLoadingContext]);

    return (
        <div>
            <h2>Parent:</h2>
            <LoadingComponent context={LoadingContext.RetrieveParent} response={parentResult} />
            <ErrorDisplayComponent response={parentResult} />
            {isSuccess(parentResult) && <PartnershipDisplay member={(parentResult.result as Partnership).member} inLaw={(parentResult.result as Partnership).inLaw} partnershipDate={(parentResult.result as Partnership).partnershipDate}/>}
        </div>
    );
};

export default ParentOfSelectedPartnership;