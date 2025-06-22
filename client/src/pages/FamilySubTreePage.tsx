import React from "react";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import FamilyTreeInput from "../components/FamilyTreeInput";
import FamilyTreeProvider from "../providers/FamilyTreeSettingsProvider";
import { useNavigate } from "react-router-dom";
import Title from "../components/Title";
const FamilySubTreePage: React.FC = () => {
    let navigate = useNavigate();
    return (
        <div>
            <Title />
            <FamilyTreeProvider>
                <FamilyTreeInput includesEntireTree={false}/>
                <FamilyTreeDisplay />
            </FamilyTreeProvider>
            <button type="button" onClick={() => navigate('/family-tree')}>Back To Tree</button>
        </div>
    );
};

export default FamilySubTreePage;