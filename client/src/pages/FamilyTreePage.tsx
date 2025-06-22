import React from "react";
//import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
//import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
import FamilyTreeProvider from "../providers/FamilyTreeProvider";
import FamilyTreeInput from "../components/FamilyTreeInput";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import SelectedFileProvider from "../providers/SelectedFileProvider";
import RevertTreeSection from "../components/RevertTreeSection";
import Title from "../components/Title";
const FamilyTreePage: React.FC = () => {
    return (
        <div>
            <Title />
            <SelectedFileProvider>
                <RevertTreeSection />
            </SelectedFileProvider>
            <FamilyTreeProvider>
                <FamilyTreeInput includesEntireTree={true}/>
                <FamilyTreeDisplay />
            </FamilyTreeProvider>
        </div>
    );
}

export default FamilyTreePage;