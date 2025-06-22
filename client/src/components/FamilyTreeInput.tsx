import React, {FormEvent, useState} from "react";
import ErrorDisplay from "./ErrorDisplay";
import LoadingDisplay from "./LoadingDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { getPartnerships, viewSubtree } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { FamilyTreeApiResponse, Partnership } from "../Types";
import { isProcessing, isSuccess } from "../Utils";

const FamilyTreeInput: React.FC<{includesEntireTree: boolean}> = (params) => {
    const orderOptions: string[] = ['parent first then children', 'ascending by name'];
    const {selectedPartnership, familyTreeSetter, updateFamilyTree} = useCriticalAttributes()
    const {addLoadingContext, removeLoadingContext, isLoading} = useLoadingContext();
    const [familyTreeResponse, setFamilyTreeResponse] = useState<FamilyTreeApiResponse>(EmptyResponse)
    const [orderOption, changeOrderOption] = useState<string>(orderOptions[0]);
    const [isClicked, setIsClicked] = useState<boolean>(false);
    const [memberName, setMemberName] = useState<string>('');

    const getFamilyTree = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            setIsClicked(true);
            setFamilyTreeResponse(EmptyResponse);
            e.preventDefault();
            if (params.includesEntireTree) {
                addLoadingContext(LoadingContext.RetrieveFamilyTree);
                setFamilyTreeResponse(await getPartnerships(orderOption, memberName));
                if (!isProcessing(familyTreeResponse)) {
                    removeLoadingContext(LoadingContext.ViewSubtree);
                    await updateFamilyTree(isSuccess(familyTreeResponse) ? familyTreeResponse.result as Partnership[] : []);
                    setIsClicked(false);
                }
            }
            else {
                addLoadingContext(LoadingContext.ViewSubtree);
                setFamilyTreeResponse(await viewSubtree(orderOption, memberName, selectedPartnership));
                if (!isProcessing(familyTreeResponse)) {
                    removeLoadingContext(LoadingContext.ViewSubtree);
                    await updateFamilyTree(isSuccess(familyTreeResponse) ? familyTreeResponse.result as Partnership[] : []);
                    setIsClicked(false);
                }
            }
        }
    }
    return (
        <form onSubmit={getFamilyTree}>
            <label>Choose Order Option:&nbsp;<select id="orderTypeSelector" value={orderOption} onChange={e => changeOrderOption(e.target.value)}>
                {orderOptions.map(option => (
                    <option key={option} value={option}>{option}</option>
                ))}
            </select></label>
            <label>Filter By Member Name:&nbsp;<input type="text" value={memberName} onChange={e => setMemberName(e.target.value)}/></label>
            <button type="submit">Get Family Tree</button>
            {isClicked && <LoadingDisplay context={params.includesEntireTree ? LoadingContext.RetrieveFamilyTree : LoadingContext.ViewSubtree} response={familyTreeResponse} />}
            <ErrorDisplay response={familyTreeResponse} />
            {isClicked && <LoadingDisplay context={LoadingContext.UpdateClientFamilyTree} response={familyTreeSetter} />}
            <ErrorDisplay response={familyTreeSetter} />
        </form>
    );
};

export default FamilyTreeInput;