import React, { useState } from "react";
import SelectedFileContext from "../contexts/SelectedFileContext";
import { ProviderProps } from "../Types";

const SelectedFileProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedFile, changeSelectedFile] = useState<File | undefined>(undefined);
    return (
        <SelectedFileContext.Provider value={{selectedFile, changeSelectedFile}}>
            {children}
        </SelectedFileContext.Provider>
    );
};

export default SelectedFileProvider;