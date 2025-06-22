import React from "react";
import './SuccessDisplayComponent.css';
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";

const SuccessDisplay: React.FC<{response: FamilyTreeApiResponse}> = (params) => {
    return (
        <p className="success">{params.response.message}</p>
    );
};

export default SuccessDisplay;