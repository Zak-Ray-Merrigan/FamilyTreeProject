import React from "react";
import { useNavigate } from "react-router-dom";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import FamilyTreeInput from "../components/FamilyTreeInput";

const FamilySubTreePage: React.FC = () => {
    let navigate = useNavigate();
    return (
        <div>
            <FamilyTreeInput includesEntireTree={false}/>
            <FamilyTreeDisplay />
            <button type="button" onClick={() => navigate('/family-tree')}>Back To Tree</button>
        </div>
    );
};

export default FamilySubTreePage;