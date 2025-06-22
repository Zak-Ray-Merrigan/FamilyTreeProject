import React from "react";
import _ from "lodash";
import PartnershipDisplay from "./PartnershipDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import { Root } from "../Constants";

const FamilyTreeDisplay: React.FC = () => {
    const {familyTree, selectedPartnership} = useCriticalAttributes();
    return _.isEqual(selectedPartnership, Root) ? ( <div>
            {familyTree.map((partnership) => (
                <PartnershipDisplay member={partnership.member} inLaw={partnership.inLaw} partnershipDate={partnership.partnershipDate} />
            ))}
        </div>) : <PartnershipDisplay member={selectedPartnership.member} inLaw={selectedPartnership.inLaw} partnershipDate={selectedPartnership.partnershipDate} />
};

export default FamilyTreeDisplay;