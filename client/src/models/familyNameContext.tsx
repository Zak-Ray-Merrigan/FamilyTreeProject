import React from "react";
interface FamilyNameContextType {
    familyName: string;
    setFamilyName: (name: string) => void;
}

const FamilyNameContext = React.createContext<FamilyNameContextType>({familyName: '', setFamilyName: () => {}});

export default FamilyNameContext;