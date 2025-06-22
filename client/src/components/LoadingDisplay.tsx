import React from "react";
import ReactLoading from 'react-loading';
import { LoadingContext } from "../Enums";
import { FamilyTreeApiResponse } from "../Types";
import { getLoadingText, isProcessing } from "../Utils";

const LoadingDisplay: React.FC<{context: LoadingContext, response: FamilyTreeApiResponse}> = (params) => {
    if (isProcessing(params.response)) {
        return (
            <p><ReactLoading type="spin" color="#000" height={100} width={100}/>&nbsp;&nbsp;{getLoadingText(params.context)}</p>
        )
    }
    return null;
};

export default LoadingDisplay;