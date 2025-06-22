import React from "react";
import "./ErrorDisplayComponent.css";

interface ErrorResponse {
    message: string;
}

const ErrorDisplayComponent: React.FC<ErrorResponse> = (response) => {
    return (
        <p className="error">{response.message}</p>
    );
};

export default ErrorDisplayComponent;