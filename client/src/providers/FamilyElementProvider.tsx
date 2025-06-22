import React, { useState } from "react";
import FamilyElement, { FamilyDefault, FamilyElementContext } from "../models/FamilyElement";
import { ProviderProps } from "../models/providerProps";

const FamilyElementProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedElement, changeSelectedElement] = useState<FamilyElement>(FamilyDefault);
    return (
        <FamilyElementContext.Provider value={{selectedElement, changeSelectedElement}}>
            {children}
        </FamilyElementContext.Provider>
    )
}

export default FamilyElementProvider;