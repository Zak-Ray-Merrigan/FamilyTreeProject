import React, {useState} from "react";
import { ProviderProps } from "../models/providerProps";
import FamilyTreeContext from "../models/FamilyTreeSettingsContext";
import OutputResponse from "../models/outputResponse";
import FamilyElement from "../models/FamilyElement";

const FamilyTreeProvider: React.FC<ProviderProps> = ({children}) => {
    const [familyTreeResponse, setFamilyTreeResponse] = useState<OutputResponse<FamilyElement[]>>({});

    return (
        <FamilyTreeContext.Provider value={{familyTreeResponse: familyTreeResponse, setFamilyTreeResponse: setFamilyTreeResponse}}>
                {children}
        </FamilyTreeContext.Provider>
    )
};

export default FamilyTreeProvider;