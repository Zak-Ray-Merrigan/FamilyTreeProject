import React, { useState } from "react";
import SelectedFileContext from "../models/SelectedFileContext";
import { ProviderProps } from "../models/providerProps";

const SelectedFileProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedFile, changeSelectedFile] = useState<File | undefined>(undefined);
    return (
        <SelectedFileContext.Provider value={{selectedFile: selectedFile, changeSelectedFile: changeSelectedFile}}>
            {children}
        </SelectedFileContext.Provider>
    );
};

export default SelectedFileProvider;