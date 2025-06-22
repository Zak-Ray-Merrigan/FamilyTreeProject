import React, { useLayoutEffect } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import NumberOfGenerations from "../components/NumberOfGenerations";
import NumberOfPartnerships from "../components/NumberOfPartnerships";
import FamilyTreeInput from "../components/FamilyTreeInput";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import SelectedFileProvider from "../providers/SelectedFileProvider";
import RevertTreeSection from "../components/RevertTreeSection";
import useCriticalAttributes from "../hooks/useCriticalAttributes";

const FamilyTreePage: React.FC = () => {
    const {familyName} = useCriticalAttributes();
    let navigate = useNavigate();

    useLayoutEffect(() => {
        if (_.isNull(familyName)) {
            navigate('/');
        }
    }, [familyName, navigate]);
    
    return (
        <div>
            <NumberOfPartnerships />
            <NumberOfGenerations />
            <SelectedFileProvider>
                <RevertTreeSection />
            </SelectedFileProvider>
            <FamilyTreeInput includesEntireTree={true}/>
            <FamilyTreeDisplay />
        </div>
    );
}

export default FamilyTreePage;