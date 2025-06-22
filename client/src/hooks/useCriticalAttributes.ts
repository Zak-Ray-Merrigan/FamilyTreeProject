import { useCallback, useEffect, useState } from "react";
import _ from "lodash";
import useLoadingContext from "./useLoadingContext";
import { getClientFamilyName, getClientFamilyTree, getClientSelectedPartnership, getClientTitle, setClientFamilyName, setClientFamilyTree, setClientSelectedPartnership, setClientTitle} from "../ApiCalls";
import { EmptyResponse, InitialTitle, Root } from "../Constants";
import { LoadingContext } from "../Enums";
import { FamilyTreeApiResponse, Partnership } from "../Types";
import { isProcessing, isSuccess } from "../Utils";

const useCriticalAttributes = () => {
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [familyName, setFamilyName] = useState<string | null>(null);
    const [familyNameGetter, setFamilyNameGetter] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [familyNameSetter, setFamilyNameSetter] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [familyTree, setFamilyTree] = useState<Partnership[]>([]);
    const [familyTreeGetter, setFamilyTreeGetter] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [familyTreeSetter, setFamilyTreeSetter] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [selectedPartnership, setSelectedPartnership] = useState<Partnership>(Root);
    const [selectedPartnershipGetter, setSelectedPartnershipGetter] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [selectedPartnershipSetter, setSelectedPartnershipSetter] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [title, setTitle] = useState<string>(InitialTitle);
    const [titleGetter, setTitleGetter] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [titleSetter, setTitleSetter] = useState<FamilyTreeApiResponse>(EmptyResponse);

    useEffect(() => {
        const fetchFamilyNameBackup = async() => {
            if (_.isNull(familyName)) {
                addLoadingContext(LoadingContext.RetrieveClientFamilyName);
                const response = await getClientFamilyName();
                if (!isProcessing(response)) {
                    removeLoadingContext(LoadingContext.RetrieveClientFamilyName);
                    if (isSuccess(response)) {
                        setFamilyName(response.result as string | null);
                    }
                }
                setFamilyNameGetter(response);
            }
        };
        const fetchFamilyTreeBackup = async() => {
            if (_.isEqual(familyTree, [])) {
                addLoadingContext(LoadingContext.RetrieveClientFamilyTree);
                const response = await getClientFamilyTree();
                if (!isProcessing(response)) {
                    removeLoadingContext(LoadingContext.RetrieveClientFamilyTree);
                    if (isSuccess(response)) {
                        setFamilyTree(response.result as Partnership[]);
                    }
                }
                setFamilyTreeGetter(response);
            }
        };
        const fetchSelectedPartnershipBackup = async() => {
            if (_.isEqual(selectedPartnership, Root)) {
                addLoadingContext(LoadingContext.RetrieveClientSelectedPartnership);
                const response = await getClientSelectedPartnership();
                if (!isProcessing(response)) {
                    removeLoadingContext(LoadingContext.RetrieveClientSelectedPartnership);
                    if (isSuccess(response)) {
                        setSelectedPartnership(response.result as Partnership);
                    }
                }
                setSelectedPartnershipGetter(response);
            }
        };
        const fetchTitleBackup = async() => {
            if (_.isEqual(title, InitialTitle)) {
                addLoadingContext(LoadingContext.RetrieveClientTitle);
                const response = await getClientTitle();
                if (!isProcessing(response)) {
                    removeLoadingContext(LoadingContext.RetrieveClientTitle);
                    if (isSuccess(response)) {
                        setTitle(response.result as string);
                    }
                }
                setTitleGetter(response);
            }
        };
        fetchFamilyNameBackup();
        fetchFamilyTreeBackup();
        fetchSelectedPartnershipBackup();
        fetchTitleBackup();
    }, [familyName, familyTree, selectedPartnership, title, addLoadingContext, removeLoadingContext]);

    const updateFamilyName = useCallback(async(clientFamilyName: string | null) => {
        setFamilyName(clientFamilyName);
        addLoadingContext(LoadingContext.UpdateClientFamilyName);
        const response = await setClientFamilyName(clientFamilyName);
        if (!isProcessing(response)) {
            removeLoadingContext(LoadingContext.UpdateClientFamilyName);
        }
        setFamilyNameSetter(response);
    }, [addLoadingContext, removeLoadingContext]);

    const updateFamilyTree = useCallback(async(clientFamilyTree: Partnership[]) => {
        setFamilyTree(clientFamilyTree);
        addLoadingContext(LoadingContext.UpdateClientFamilyTree);
        const response = await setClientFamilyTree(clientFamilyTree);
        if (!isProcessing(response)) {
            removeLoadingContext(LoadingContext.UpdateClientFamilyTree);
        }
        setFamilyTreeSetter(response);
    }, [addLoadingContext, removeLoadingContext]);
    
    const updateSelectedPartnership = useCallback(async(clientSelectedPartnership: Partnership) => {
        setSelectedPartnership(clientSelectedPartnership);
        addLoadingContext(LoadingContext.UpdateClientSelectedPartnership);
        const response = await setClientSelectedPartnership(clientSelectedPartnership);
        if (!isProcessing(response)) {
            removeLoadingContext(LoadingContext.UpdateClientSelectedPartnership);
        }
        setSelectedPartnershipSetter(response);
    }, [addLoadingContext, removeLoadingContext]);

    const updateTitle = useCallback(async(clientTitle: string) => {
        setTitle(clientTitle);
        addLoadingContext(LoadingContext.UpdateClientTitle);
        const response = await setClientTitle(clientTitle);
        if (!isProcessing(response)) {
            removeLoadingContext(LoadingContext.UpdateClientTitle);
        }
        setTitleSetter(response);
    }, [addLoadingContext, removeLoadingContext]);

    return {
        familyName,
        familyNameGetter,
        familyNameSetter,
        familyTree,
        familyTreeGetter,
        familyTreeSetter,
        selectedPartnership,
        selectedPartnershipGetter,
        selectedPartnershipSetter,
        title,
        titleGetter,
        titleSetter,
        updateFamilyName,
        updateFamilyTree,
        updateSelectedPartnership,
        updateTitle
    };
}

export default useCriticalAttributes;