import React, { FormEvent, useContext, useState } from "react";
import _ from "lodash";
import Arrow from "./Arrow";
import ErrorDisplay from "./ErrorDisplay";
import FileUpload from "./FileUpload";
import LoadingDisplay from "./LoadingDisplay";
import SuccessDisplay from "./SuccessDisplayComponent";
import SelectedFileContext from "../contexts/SelectedFileContext";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { revertTree } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { FamilyTreeApiResponse } from "../Types";
import { isProcessing, isSuccess } from "../Utils";
import '../styles/RevertTreeSection.css';

const RevertTreeSection: React.FC = () => {
    const {selectedFile} = useContext(SelectedFileContext);
    const {familyName} = useCriticalAttributes();
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [revertTreeResponse, setRevertTreeResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [clicked, isClicked] = useState<boolean>(false);
    const [isVisible, setIsVisible] = useState<boolean>(false);

    const handleRevertTree = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isClicked(true);
            if (selectedFile) {
                addLoadingContext(LoadingContext.RevertTree)
                setRevertTreeResponse(await revertTree(selectedFile));
                if (!isProcessing(revertTreeResponse)) {
                    removeLoadingContext(LoadingContext.RevertTree);
                }
            }
            else {
                setRevertTreeResponse({message: 'No file was selected.', status: FamilyTreeApiResponseStatus.Failure});
            }
            isClicked(false);
        }
    };

    const handleVisibility = () => {
        if (!isLoading()) {
            setIsVisible(!isVisible);
        }
    }

    return (
        <section id="revert-tree-section">
            <header id="revert-tree-header" onClick={handleVisibility}>
                <h2>Revert Tree&nbsp;&nbsp;<Arrow isVisible={isVisible}/></h2>
            </header>
            {isVisible && (
                <form onSubmit={handleRevertTree}>
                    <FileUpload /><br/>
                    {!_.isUndefined(selectedFile) && <><p>{selectedFile.name}</p><br/></>}
                    <button type="submit">Revert {familyName} Tree</button>
                    {clicked && <LoadingDisplay context={LoadingContext.RevertTree} response={revertTreeResponse}/>}
                    <ErrorDisplay response={revertTreeResponse}/>
                    {isSuccess(revertTreeResponse) && <SuccessDisplay response={revertTreeResponse} />}
                </form>
            )}
        </section>
    )
};

export default RevertTreeSection;