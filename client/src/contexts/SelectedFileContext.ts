import React from "react";

interface SelectedFileContextType {
    selectedFile?: File;
    changeSelectedFile: (selectedFile?: File) => void;
};

const SelectedFileContext = React.createContext<SelectedFileContextType>({changeSelectedFile: () => {}});

export default SelectedFileContext;