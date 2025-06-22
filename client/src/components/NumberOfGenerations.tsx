import React, {useEffect, useState} from "react";
import ErrorDisplayComponent from "./ErrorDisplay";
import LoadingComponent from "./LoadingDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { getNumberOfGenerations } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { FamilyTreeApiResponse } from "../Types";
import { isProcessing, isSuccess } from "../Utils";

const NumberOfGenerations: React.FC = () => {
    const {familyName} = useCriticalAttributes();
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [numericOutput, setNumericOutput] = useState<FamilyTreeApiResponse>(EmptyResponse);
    useEffect(() => {
        const fetchNumberOfGenerations = async () => {
            addLoadingContext(LoadingContext.NumberOfGenerations);
            setNumericOutput(await getNumberOfGenerations());
            if (!isProcessing(numericOutput)) {
                removeLoadingContext(LoadingContext.NumberOfGenerations);
            }
        };
        fetchNumberOfGenerations();
    }, [familyName, addLoadingContext, removeLoadingContext, numericOutput]);
    return (
        <>
            <LoadingComponent context={LoadingContext.NumberOfGenerations} response={numericOutput} />
            <ErrorDisplayComponent response={numericOutput} />
            {isSuccess(numericOutput) && <p>There are {numericOutput.result as number} generations in the {familyName} family tree.</p>}
        </>
    )
}

export default NumberOfGenerations;