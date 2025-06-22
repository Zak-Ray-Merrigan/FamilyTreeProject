import React, { useContext, useEffect} from "react";
import _ from "lodash";
import { FamilyElementContext } from "../models/FamilyElement";
import ParentOfSelectedElement from "../components/ParentOfSelectedElement";
import FamilyInfoElement from "../components/FamilyInfoElement";
import { StringDefault, createURL } from "../Utils";
import ReportActionsSection from "../components/ReportActionsSection";
import ChildrenOfSelectedElement from "../components/ChildrenOfSelectedElement";
import { useNavigate } from "react-router-dom";
import TitleContext from "../models/TitleContext";
import Title from "../components/Title";
import './FamilyProfilePage.css';

const FamilyProfilePage: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {setTitle} = useContext(TitleContext);
    let navigate = useNavigate();
    useEffect(() => {
        if (!_.isEqual(selectedElement.member.name, StringDefault) && !_.isEqual(selectedElement.inLaw.name, StringDefault)) {
            setTitle(`This is the family of ${selectedElement.member.name} and ${selectedElement.inLaw.name}`);
        }
        else if (!_.isEqual(selectedElement.member.name, StringDefault)) {
            setTitle(`This is the family of ${selectedElement.member.name}`);
        }
        else {
            setTitle('Unknown Family Element');
        }
    }, [selectedElement, setTitle]);

    const handleViewSubtree = () => {
        setTitle(`This is the subtree of ${selectedElement.member.name}`);
        navigate(createURL('/sub-tree', {memberName: selectedElement.member.name}));
    }

    return (
        <div>
            <Title />
            <ParentOfSelectedElement/>
            <FamilyInfoElement/>
            <ReportActionsSection />
            <ChildrenOfSelectedElement />
            <div id="family-profile-controls">
                <button type="button" className="family-profile-control" onClick={() => navigate('/family-tree')}>Back To Tree</button>
                <button type="button" className="family-profile-control" onClick={handleViewSubtree}>View Subtree</button>
            </div>
        </div>
    );
};

export default FamilyProfilePage;