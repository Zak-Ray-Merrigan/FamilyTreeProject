import React, { useEffect} from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import ChildrenOfSelectedPartnership from "../components/ChildrenOfSelectedPartnership";
import ErrorDisplay from "../components/ErrorDisplay";
import LoadingDisplay from "../components/LoadingDisplay";
import ParentOfSelectedPartnership from "../components/ParentOfSelectedPartnership";
import PartnershipInfo from "../components/PartnershipInfo";
import ReportActionsSection from "../components/ReportActionsSection";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import '../styles/FamilyProfilePage.css';
import { LoadingContext } from "../Enums";
import { createURL, isSuccess } from "../Utils";

const FamilyProfilePage: React.FC = () => {
    const {selectedPartnership, titleSetter, updateTitle} = useCriticalAttributes();
    let navigate = useNavigate();
    useEffect(() => {
        const handleTitle = async() => {
            if (!_.isNull(selectedPartnership.member) && !_.isNull(selectedPartnership.inLaw)) {
                await updateTitle(`This is the family of ${selectedPartnership.member.name} and ${selectedPartnership.inLaw.name}`);
            }
            else if (!_.isNull(selectedPartnership.member)) {
                await updateTitle(`This is the family of ${selectedPartnership.member.name}`);
            }
            else {
                navigate('/family-tree');
            }
        }
        handleTitle();
    }, [selectedPartnership, titleSetter, navigate, updateTitle]);

    const handleViewSubtree = async() => {
        if (!_.isNull(selectedPartnership.member) && !_.isNull(selectedPartnership.inLaw)) {
            await updateTitle(`This is the subtree of ${selectedPartnership.member.name} and ${selectedPartnership.inLaw.name}`);
            navigate(createURL('/sub-tree', {memberName: selectedPartnership.member.name, inLawName: selectedPartnership.inLaw.name}));
        }
        else if (!_.isNull(selectedPartnership.member)) {
            await updateTitle(`This is the subtree of ${selectedPartnership.member.name}`);
            navigate(createURL('/sub-tree', {memberName: selectedPartnership.member.name}));
        }
    }

    return (
        <>
            <LoadingDisplay response={titleSetter} context={LoadingContext.UpdateClientTitle} />
            <ErrorDisplay response={titleSetter} />
            {isSuccess(titleSetter) && (
                <div>
                    <ParentOfSelectedPartnership/>
                    <PartnershipInfo/>
                    <ReportActionsSection />
                    <ChildrenOfSelectedPartnership />
                    <div id="family-profile-controls">
                        <button type="button" className="family-profile-control" onClick={() => navigate('/family-tree')}>Back To Tree</button>
                        <button type="button" className="family-profile-control" onClick={handleViewSubtree}>View Subtree</button>
                    </div>
                </div>
            )}
        </>
    );
};

export default FamilyProfilePage;