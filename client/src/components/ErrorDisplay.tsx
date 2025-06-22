import React from "react";
import { FamilyTreeApiResponse } from "../Types";
import { isProcessing, isSuccess } from "../Utils";
import "../styles/ErrorDisplay.css";

// ErrorDisplay component: Displays error messages from API responses
const ErrorDisplay: React.FC<{response: FamilyTreeApiResponse}> = (params) => {
    // Only render the error message if the response is not processing and not successful
    return isProcessing(params.response) || isSuccess(params.response) ? null : (
        <p id="error">{params.response.message}</p>
    );
};

export default ErrorDisplay;