import React from "react";
import OutputResponse from "./outputResponse";
import FamilyElement from "./FamilyElement";

interface FamilyTreeContextType {
    familyTreeResponse: OutputResponse<FamilyElement[]>;
    setFamilyTreeResponse: (familyTreeResponse: OutputResponse<FamilyElement[]>) => void;
}

const FamilyTreeSettingsContext = React.createContext<FamilyTreeContextType>({familyTreeResponse: {}, setFamilyTreeResponse: () => {}});

export default FamilyTreeSettingsContext;