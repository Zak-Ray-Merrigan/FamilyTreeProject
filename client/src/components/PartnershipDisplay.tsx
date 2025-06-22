import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import ErrorDisplay from "./ErrorDisplay";
import LoadingDisplay from "./LoadingDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { partnershipToRepresentation, representationToPartnership } from "../ApiCalls";
import { EmptyResponse, Root } from "../Constants";
import { FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { FamilyTreeApiResponse, Partnership, RepresentationElement } from "../Types";
import { createURL, isProcessing, isSuccess } from "../Utils";
import "../styles/PartnershipDisplay.css";

const PartnershipDisplay: React.FC<Partnership> = (partnership) => {
    const {selectedPartnership, selectedPartnershipSetter, updateSelectedPartnership} = useCriticalAttributes();
    const {addLoadingContext, removeLoadingContext, isLoading} = useLoadingContext();
    const [representationOutput, setRepresentationOutput] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [partnershipClicked, isPartnershipClicked] = useState<boolean>(false);
    let navigate = useNavigate();

    const handleClick = async(e: React.MouseEvent<HTMLParagraphElement>) => {
        if (!isLoading()) {
            isPartnershipClicked(true);
            addLoadingContext(LoadingContext.RepresentationToPartnership);
            const response : FamilyTreeApiResponse = _.isNull(e.currentTarget.textContent) ? {
                status: FamilyTreeApiResponseStatus.Success,
                message: 'The partnership doesn\'t exist.',
                result: Root
            } : await representationToPartnership({representation: e.currentTarget.textContent});
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.RepresentationToPartnership);
                if (isSuccess(response)) {
                    await updateSelectedPartnership(response.result as Partnership);
                    if (isSuccess(selectedPartnershipSetter) && !_.isNull(selectedPartnership.member) && _.isNull(selectedPartnership.inLaw)) {
                        navigate(createURL('/family-profile', {member: selectedPartnership.member.name}));
                    }
                    else if (isSuccess(selectedPartnershipSetter) && !(_.isNull(selectedPartnership.member) || _.isNull(selectedPartnership.inLaw))) {
                        navigate(createURL('/family-profile', {member: selectedPartnership.member.name, inLaw: selectedPartnership.inLaw.name}));
                    }
                }
            }
            isPartnershipClicked(false);
        }
    };

    useEffect(() => {
        const handleRender = async () => {
            addLoadingContext(LoadingContext.PartnershipToRepresentation);
            const response = await partnershipToRepresentation(partnership);
            setRepresentationOutput(response);
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.PartnershipToRepresentation);
            }
        };
        handleRender();
    }, [partnership, addLoadingContext, removeLoadingContext]);
    return (
        <>
            <LoadingDisplay context={LoadingContext.PartnershipToRepresentation} response={representationOutput}/>
            <ErrorDisplay response={representationOutput} />
            {partnershipClicked && <LoadingDisplay context={LoadingContext.UpdateClientSelectedPartnership} response={selectedPartnershipSetter} />}
            <ErrorDisplay response={selectedPartnershipSetter} />
            {isSuccess(representationOutput) && !partnershipClicked && <p className="familyElement" onClick={handleClick}>{(representationOutput.result as RepresentationElement).representation}</p>}
        </>
    )
};

export default PartnershipDisplay;