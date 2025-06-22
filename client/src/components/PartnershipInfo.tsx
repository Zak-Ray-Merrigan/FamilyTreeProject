import React, { useEffect, useState } from "react";
import _ from "lodash";
import ErrorDisplay from "./ErrorDisplay";
import LoadingComponent from "./LoadingDisplay";
import PersonInfo from "./PersonInfo";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { generationNumberOf } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext, PersonType } from "../Enums";
import { FamilyTreeApiResponse } from "../Types";
import { isProcessing, isSuccess } from "../Utils";

const PartnershipInfo: React.FC = () => {
    const {selectedPartnership} = useCriticalAttributes();
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [generationNumberResponse, setGenerationNumberResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);

    useEffect(() => {
        const handleGenerationNumberOf = async() => {
            addLoadingContext(LoadingContext.GenerationNumber);
            const response = await generationNumberOf(selectedPartnership);
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.GenerationNumber);
            }
            setGenerationNumberResponse(response);
        };
        handleGenerationNumberOf();
    }, [selectedPartnership, addLoadingContext, removeLoadingContext]);
    return (
        <div>
            <h2>Info:</h2>
            <LoadingComponent context={LoadingContext.GenerationNumber} response={generationNumberResponse} />
            <ErrorDisplay response={generationNumberResponse}/>
            {isSuccess(generationNumberResponse) && (
                <>
                    <h3>Generation Number: {generationNumberResponse.result as number}</h3>
                    <PersonInfo type={PersonType.Member} element={selectedPartnership.member}/>
                    <PersonInfo type={PersonType.InLaw} element={selectedPartnership.inLaw}/>
                    {!_.isNull(selectedPartnership.partnershipDate) && <h3>Partnership Date: {selectedPartnership.partnershipDate}</h3>}
                </>
            )}
        </div>
    )
};

export default PartnershipInfo;
