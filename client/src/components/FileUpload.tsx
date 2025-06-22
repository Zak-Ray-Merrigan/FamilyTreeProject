import React, { ChangeEvent, useContext } from "react";
import SelectedFileContext from "../models/SelectedFileContext";

const FileUpload: React.FC = () => {
    const {changeSelectedFile} = useContext(SelectedFileContext);
    
    const handleSelectFile = (e: ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (files) {
            changeSelectedFile(files[0]);
        }
    };

    return (
        <label>Choose A File:&nbsp;<input type="file" onChange={handleSelectFile}/></label>
    );
};

export default FileUpload;