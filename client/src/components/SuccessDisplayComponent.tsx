import React from "react";
import './SuccessDisplayComponent.css';

interface SuccessResponse {
    message: string;
}

const SuccessDisplay: React.FC<SuccessResponse> = (response) => {
    return (
        <p className="success">{response.message}</p>
    );
};

export default SuccessDisplay;