import React from "react";
import PersonElement, { PersonDefault } from "./PersonElement";
import { StringDefault } from "../Utils";
export default interface FamilyElement {
    member: PersonElement;
    inLaw: PersonElement;
    marriageDate: string;
}

export const FamilyDefault: FamilyElement = {
    member: PersonDefault,
    inLaw: PersonDefault,
    marriageDate: StringDefault
};

interface FamilyElementContextType {
    selectedElement: FamilyElement;
    changeSelectedElement: (selectedElement: FamilyElement) => void;
}

export const FamilyElementContext = React.createContext<FamilyElementContextType>({selectedElement: FamilyDefault, changeSelectedElement: () => {}});