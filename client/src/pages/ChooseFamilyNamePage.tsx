import React, { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import ErrorDisplay from "../components/ErrorDisplay";
import LoadingDisplay from "../components/LoadingDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { initializeService } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import {FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { FamilyTreeApiResponse } from "../Types";
import { isProcessing, isSuccess } from "../Utils";


const ChooseFamilyNamePage: React.FC = () => {
    const {familyNameSetter, titleSetter, updateFamilyName, updateTitle} = useCriticalAttributes();
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [localFamilyName, setLocalFamilyName] = useState<string>('');
    let navigate = useNavigate();
    const [clicked, isClicked] = useState<boolean>(false);
    const [familyNameResponse, setFamilyNameResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const handleSubmitFamilyName = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isClicked(true);
            if (!_.isEqual(localFamilyName, '')) {
                addLoadingContext(LoadingContext.Default);
                setFamilyNameResponse (await initializeService(localFamilyName));
                if (!isProcessing(familyNameResponse)) {
                    removeLoadingContext(LoadingContext.Default);
                    if (isSuccess(familyNameResponse)) {
                        await updateFamilyName(localFamilyName);
                        await updateTitle(familyNameResponse.message);
                        if (isSuccess(familyNameSetter) && isSuccess(titleSetter)) {
                            navigate('/family-tree');
                        }
                    }
                }
            }
            else {
                setFamilyNameResponse({message: 'No name was provided.', status: FamilyTreeApiResponseStatus.Failure});
            }
            isClicked(false);
        }
    };
    return (
        <form onSubmit={handleSubmitFamilyName}>
            <label>Enter a Family Name:&nbsp;<input type="text" value={localFamilyName} onChange={(e) => setLocalFamilyName(e.target.value)}/></label><br/>
            <button type="submit">Go To Family Tree</button>
            {clicked && <LoadingDisplay context={LoadingContext.Default} response={familyNameResponse} />}
            <ErrorDisplay response={familyNameResponse} />
            {clicked && <LoadingDisplay context={LoadingContext.UpdateClientFamilyName} response={familyNameSetter} />}
            <ErrorDisplay response={familyNameSetter} />
            {clicked && <LoadingDisplay context={LoadingContext.UpdateClientTitle} response={titleSetter} />}
            <ErrorDisplay response={titleSetter} />
        </form>
    );
};
export default ChooseFamilyNamePage;